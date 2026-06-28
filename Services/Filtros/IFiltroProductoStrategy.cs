using CatalogoRopaMVC.Models;
using CatalogoRopaMVC.ViewModels;

namespace CatalogoRopaMVC.Services.Filtros;

public interface IFiltroProductoStrategy
{
    IQueryable<Producto> Aplicar(IQueryable<Producto> productos, ProductoFiltroViewModel filtro);
}
