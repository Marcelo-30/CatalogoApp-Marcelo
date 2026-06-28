using CatalogoRopaMVC.Models;
using CatalogoRopaMVC.ViewModels;

namespace CatalogoRopaMVC.Services.Filtros;

public class FiltroDisponibilidadProductoStrategy : IFiltroProductoStrategy
{
    public IQueryable<Producto> Aplicar(IQueryable<Producto> productos, ProductoFiltroViewModel filtro)
    {
        if (!filtro.SoloDisponibles)
        {
            return productos;
        }

        return productos.Where(p => p.Disponible && p.Stock > 0);
    }
}
