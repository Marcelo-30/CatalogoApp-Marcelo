using System.Security.Claims;
using CatalogoRopaMVC.Models;
using CatalogoRopaMVC.Services;
using CatalogoRopaMVC.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoRopaMVC.Controllers;

public class CuentaController : Controller
{
    private readonly IVendedorAuthService _vendedorAuthService;

    public CuentaController(IVendedorAuthService vendedorAuthService)
    {
        _vendedorAuthService = vendedorAuthService;
    }

    // GET: Cuenta/Login
    public async Task<IActionResult> Login()
    {
        ViewBag.PuedeRegistrarVendedor = !await _vendedorAuthService.ExisteVendedorAsync();
        return View(new LoginViewModel());
    }

    // POST: Cuenta/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.PuedeRegistrarVendedor = !await _vendedorAuthService.ExisteVendedorAsync();
            return View(model);
        }

        var vendedor = await _vendedorAuthService.ValidarCredencialesAsync(model);
        if (vendedor == null)
        {
            ViewBag.PuedeRegistrarVendedor = !await _vendedorAuthService.ExisteVendedorAsync();
            ModelState.AddModelError(string.Empty, "Correo o contrasena incorrectos.");
            return View(model);
        }

        await IniciarSesionAsync(vendedor);
        TempData["Mensaje"] = "Sesion iniciada como vendedor. Puedes administrar productos.";

        return RedirectToAction("Index", "Productos");
    }

    // GET: Cuenta/Registro
    public async Task<IActionResult> Registro()
    {
        if (await _vendedorAuthService.ExisteVendedorAsync())
        {
            TempData["Error"] = "Ya existe un vendedor dueno registrado. Usa sus credenciales para entrar.";
            return RedirectToAction(nameof(Login));
        }

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

        var vendedor = await _vendedorAuthService.RegistrarPrimerVendedorAsync(model);
        if (vendedor == null)
        {
            ModelState.AddModelError(string.Empty, "Ya existe un vendedor dueno registrado.");
            return View(model);
        }

        await IniciarSesionAsync(vendedor);
        TempData["Mensaje"] = "Vendedor dueno registrado correctamente. Ya puedes administrar productos.";

        return RedirectToAction("Index", "Productos");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["Mensaje"] = "Sesion cerrada correctamente.";
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccesoDenegado()
    {
        return View();
    }

    private async Task IniciarSesionAsync(Vendedor vendedor)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, vendedor.Id.ToString()),
            new Claim(ClaimTypes.Name, vendedor.Nombre),
            new Claim(ClaimTypes.Email, vendedor.Email),
            new Claim(ClaimTypes.Role, "Vendedor")
        };

        var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identidad);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }
}
