using System.Security.Claims;
using CatalogoRopaMVC.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoRopaMVC.Controllers;

public class CuentaController : Controller
{
    // GET: Cuenta/Login
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    // POST: Cuenta/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var rol = NormalizarRol(model.Rol);
        if (rol == null)
        {
            ModelState.AddModelError(nameof(model.Rol), "Selecciona Cliente o Vendedor.");
            return View(model);
        }

        await IniciarSesionAsync(model.Email, rol);
        TempData["Mensaje"] = rol == "Vendedor"
            ? "Sesión iniciada como vendedor. Puedes administrar productos."
            : "Sesión iniciada como cliente. Puedes consultar el catálogo.";

        return RedirectToAction("Index", "Productos");
    }

    // GET: Cuenta/Registro
    public IActionResult Registro()
    {
        return View(new RegistroViewModel());
    }

    // POST: Cuenta/Registro
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registro(RegistroViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var rol = NormalizarRol(model.Rol);
        if (rol == null)
        {
            ModelState.AddModelError(nameof(model.Rol), "Selecciona Cliente o Vendedor.");
            return View(model);
        }

        // Versión inicial: registra la sesión en modo demostración.
        // Más adelante se puede reemplazar por ASP.NET Core Identity y guardar usuarios en la base de datos.
        await IniciarSesionAsync(model.Email, rol, model.Nombre);
        TempData["Mensaje"] = rol == "Vendedor"
            ? "Registro demo completado como vendedor. Puedes administrar productos."
            : "Registro demo completado como cliente. Puedes consultar el catálogo.";

        return RedirectToAction("Index", "Productos");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["Mensaje"] = "Sesión cerrada correctamente.";
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccesoDenegado()
    {
        return View();
    }

    private static string? NormalizarRol(string? rol)
    {
        if (string.Equals(rol, "Vendedor", StringComparison.OrdinalIgnoreCase))
        {
            return "Vendedor";
        }

        if (string.Equals(rol, "Cliente", StringComparison.OrdinalIgnoreCase))
        {
            return "Cliente";
        }

        return null;
    }

    private async Task IniciarSesionAsync(string email, string rol, string? nombre = null)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, string.IsNullOrWhiteSpace(nombre) ? email : nombre),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, rol)
        };

        var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identidad);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }
}
