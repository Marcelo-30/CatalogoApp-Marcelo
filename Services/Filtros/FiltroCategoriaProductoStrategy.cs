using CatalogoRopaMVC.Models;
using CatalogoRopaMVC.ViewModels;

namespace CatalogoRopaMVC.Services.Filtros;

public class FiltroCategoriaProductoStrategy : IFiltroProductoStrategy
{
    public IQueryable<Producto> Aplicar(IQueryable<Producto> productos, ProductoFiltroViewModel filtro)
    {
        if (!filtro.CategoriaId.HasValue || filtro.CategoriaId.Value <= 0)
        {
            return productos;
        }

        return productos.Where(p => p.CategoriaId == filtro.CategoriaId.Value);
    }
}
