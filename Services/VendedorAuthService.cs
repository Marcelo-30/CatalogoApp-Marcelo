using CatalogoRopaMVC.Data;
using CatalogoRopaMVC.Models;
using CatalogoRopaMVC.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CatalogoRopaMVC.Services;

public class VendedorAuthService : IVendedorAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<Vendedor> _passwordHasher;

    public VendedorAuthService(ApplicationDbContext context, IPasswordHasher<Vendedor> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public Task<bool> ExisteVendedorAsync()
    {
        return _context.Vendedores.AnyAsync();
    }

    public async Task<Vendedor?> RegistrarPrimerVendedorAsync(RegistroViewModel model)
    {
        if (await ExisteVendedorAsync())
        {
            return null;
        }

        var vendedor = new Vendedor
        {
            Nombre = model.Nombre.Trim(),
            Email = model.Email.Trim(),
            EmailNormalizado = NormalizarEmail(model.Email),
            FechaRegistro = DateTime.Now
        };

        vendedor.PasswordHash = _passwordHasher.HashPassword(vendedor, model.Password);

        _context.Vendedores.Add(vendedor);
        await _context.SaveChangesAsync();

        return vendedor;
    }

    public async Task<Vendedor?> ValidarCredencialesAsync(LoginViewModel model)
    {
        var emailNormalizado = NormalizarEmail(model.Email);
        var vendedor = await _context.Vendedores
            .FirstOrDefaultAsync(v => v.EmailNormalizado == emailNormalizado);

        if (vendedor == null)
        {
            return null;
        }

        var resultado = _passwordHasher.VerifyHashedPassword(vendedor, vendedor.PasswordHash, model.Password);
        if (resultado == PasswordVerificationResult.Failed)
        {
            return null;
        }

        if (resultado == PasswordVerificationResult.SuccessRehashNeeded)
        {
            vendedor.PasswordHash = _passwordHasher.HashPassword(vendedor, model.Password);
        }

        vendedor.UltimoAcceso = DateTime.Now;
        await _context.SaveChangesAsync();

        return vendedor;
    }

    private static string NormalizarEmail(string email)
    {
        return email.Trim().ToUpperInvariant();
    }
}
