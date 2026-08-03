using System.Security.Claims;
using CatalogoRopaMVC.Models;
using CatalogoRopaMVC.Services;
using CatalogoRopaMVC.ViewModels;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoRopaMVC.Controllers.Api;

[ApiController]
[Route("api/auth")]
public class AuthApiController : ControllerBase
{
    private readonly IVendedorAuthService _vendedorAuthService;
    private readonly IAntiforgery _antiforgery;

    public AuthApiController(IVendedorAuthService vendedorAuthService, IAntiforgery antiforgery)
    {
        _vendedorAuthService = vendedorAuthService;
        _antiforgery = antiforgery;
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetStatus()
    {
        return Ok(new
        {
            IsAuthenticated = User.Identity?.IsAuthenticated == true,
            UserName = User.Identity?.Name,
            Role = User.IsInRole("Vendedor") ? "Vendedor" : null,
            CanRegisterSeller = !await _vendedorAuthService.ExisteVendedorAsync()
        });
    }

    [HttpGet("antiforgery")]
    public IActionResult GetAntiforgeryToken()
    {
        var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
        return Ok(new { Token = tokens.RequestToken });
    }

    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var vendedor = await _vendedorAuthService.ValidarCredencialesAsync(model);
        if (vendedor == null)
        {
            return Unauthorized(new { Message = "El correo o la contraseña son incorrectos." });
        }

        await SignInAsync(vendedor);
        return Ok(new
        {
            IsAuthenticated = true,
            UserName = vendedor.Nombre,
            Role = "Vendedor",
            CanRegisterSeller = false
        });
    }

    [HttpPost("register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegistroViewModel model)
    {
        var vendedor = await _vendedorAuthService.RegistrarPrimerVendedorAsync(model);
        if (vendedor == null)
        {
            return Conflict(new { Message = "Ya existe un vendedor registrado." });
        }

        await SignInAsync(vendedor);
        return Ok(new
        {
            IsAuthenticated = true,
            UserName = vendedor.Nombre,
            Role = "Vendedor",
            CanRegisterSeller = false
        });
    }

    [Authorize(Roles = "Vendedor")]
    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return NoContent();
    }

    private async Task SignInAsync(Vendedor vendedor)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, vendedor.Id.ToString()),
            new(ClaimTypes.Name, vendedor.Nombre),
            new(ClaimTypes.Email, vendedor.Email),
            new(ClaimTypes.Role, "Vendedor")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));
    }
}

