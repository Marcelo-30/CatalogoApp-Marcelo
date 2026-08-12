using CatalogoRopaMVC.Data;
using CatalogoRopaMVC.Infrastructure;
using CatalogoRopaMVC.Services;
using CatalogoRopaMVC.Services.Factories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(defaultConnection))
{
    throw new InvalidOperationException(
        "Missing required connection string 'DefaultConnection'. " +
        "Configure ConnectionStrings__DefaultConnection for the current environment.");
}

// Los servicios de ViewFeatures son necesarios para ValidateAntiForgeryToken.
// No se mapean rutas MVC ni existen Razor Views: toda la interfaz es React.
builder.Services.AddControllersWithViews();
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.Name = "CatalogoRopa.Antiforgery";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.SameAsRequest
        : CookieSecurePolicy.Always;
});
builder.Services.AddScoped<IVendedorAuthService, VendedorAuthService>();
builder.Services.AddScoped<Microsoft.AspNetCore.Identity.IPasswordHasher<CatalogoRopaMVC.Models.Vendedor>, Microsoft.AspNetCore.Identity.PasswordHasher<CatalogoRopaMVC.Models.Vendedor>>();
builder.Services.AddScoped<IProductoDtoFactory, ProductoDtoFactory>();

// Autenticación por cookies para diferenciar Cliente y Vendedor.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/vendedor/login";
        options.AccessDeniedPath = "/vendedor/login?denegado=1";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                }

                context.Response.Redirect(context.RedirectUri);
                return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = context =>
            {
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                }

                context.Response.Redirect(context.RedirectUri);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// Conexión a SQL Server usando Entity Framework Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        defaultConnection,
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null)));

builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database");

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;
    options.ForwardLimit = 1;

    // El proxy administrado de Azure termina TLS. La aplicación solo confía
    // en el primer salto y usa los encabezados para reconstruir el esquema HTTPS.
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new
            {
                Title = "Ocurrió un error inesperado.",
                Status = StatusCodes.Status500InternalServerError
            });
        });
    });
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

if (builder.Configuration.GetValue<bool>("Database:ApplyMigrations"))
{
    await using var scope = app.Services.CreateAsyncScope();
    var logger = scope.ServiceProvider
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("DatabaseMigration");
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    logger.LogInformation("Applying pending Entity Framework Core migrations.");
    await dbContext.Database.MigrateAsync();
    logger.LogInformation("Database migrations completed.");
}

app.MapHealthChecks("/health");

app.MapControllers();

app.MapFallback(async context =>
{
    var path = context.Request.Path;
    var isReservedPath =
        path.StartsWithSegments("/api") ||
        path.StartsWithSegments("/health");

    var indexPath = Path.Combine(app.Environment.WebRootPath, "index.html");
    if (isReservedPath || !File.Exists(indexPath))
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        return;
    }

    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.SendFileAsync(indexPath);
});

app.Run();
