using CatalogoRopaMVC.Models;
using CatalogoRopaMVC.ViewModels;

namespace CatalogoRopaMVC.Services;

public interface IVendedorAuthService
{
    Task<bool> ExisteVendedorAsync();

    Task<Vendedor?> RegistrarPrimerVendedorAsync(RegistroViewModel model);

    Task<Vendedor?> ValidarCredencialesAsync(LoginViewModel model);
}
