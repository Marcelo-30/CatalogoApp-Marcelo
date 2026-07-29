using CatalogoRopaMVC.Data;
using CatalogoRopaMVC.Infrastructure;
using CatalogoRopaMVC.Services;
using CatalogoRopaMVC.Services.Factories;
using CatalogoRopaMVC.Services.Filtros;
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

// Servicios del patrón MVC
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IProductoCatalogoService, ProductoCatalogoService>();
builder.Services.AddScoped<IVendedorAuthService, VendedorAuthService>();
builder.Services.AddScoped<Microsoft.AspNetCore.Identity.IPasswordHasher<CatalogoRopaMVC.Models.Vendedor>, Microsoft.AspNetCore.Identity.PasswordHasher<CatalogoRopaMVC.Models.Vendedor>>();
builder.Services.AddScoped<IProductoDtoFactory, ProductoDtoFactory>();
builder.Services.AddScoped<IFiltroProductoStrategy, FiltroTextoProductoStrategy>();
builder.Services.AddScoped<IFiltroProductoStrategy, FiltroCategoriaProductoStrategy>();
builder.Services.AddScoped<IFiltroProductoStrategy, FiltroDisponibilidadProductoStrategy>();

// Autenticación por cookies para diferenciar Cliente y Vendedor.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Cuenta/Login";
        options.AccessDeniedPath = "/Cuenta/AccesoDenegado";
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
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
