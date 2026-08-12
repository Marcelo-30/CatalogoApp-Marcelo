using CatalogoRopaMVC.Models;
using CatalogoRopaMVC.ViewModels;

namespace CatalogoRopaMVC.Services.Filtros;

public class FiltroTextoProductoStrategy : IFiltroProductoStrategy
{
    public IQueryable<Producto> Aplicar(IQueryable<Producto> productos, ProductoFiltroViewModel filtro)
    {
        if (string.IsNullOrWhiteSpace(filtro.Buscar))
        {
            return productos;
        }

        return productos.Where(p =>
            p.Nombre.Contains(filtro.Buscar) ||
            (p.Descripcion != null && p.Descripcion.Contains(filtro.Buscar)) ||
            (p.Color != null && p.Color.Nombre.Contains(filtro.Buscar)) ||
            (p.Talla != null && p.Talla.Nombre.Contains(filtro.Buscar)));
    }
}
