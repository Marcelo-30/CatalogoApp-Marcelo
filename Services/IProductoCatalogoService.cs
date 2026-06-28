using CatalogoRopaMVC.Models;
using CatalogoRopaMVC.ViewModels;

namespace CatalogoRopaMVC.Services;

public interface IProductoCatalogoService
{
    Task<IReadOnlyList<Producto>> ObtenerCatalogoAsync(ProductoFiltroViewModel filtro);

    Task<IReadOnlyList<Categoria>> ObtenerCategoriasAsync();

    Task<IReadOnlyList<Talla>> ObtenerTallasAsync();

    Task<IReadOnlyList<Color>> ObtenerColoresAsync();
}
