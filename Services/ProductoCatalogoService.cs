using CatalogoRopaMVC.Data;
using CatalogoRopaMVC.Models;
using CatalogoRopaMVC.Services.Filtros;
using CatalogoRopaMVC.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CatalogoRopaMVC.Services;

public class ProductoCatalogoService : IProductoCatalogoService
{
    private readonly ApplicationDbContext _context;
    private readonly IEnumerable<IFiltroProductoStrategy> _filtros;

    public ProductoCatalogoService(ApplicationDbContext context, IEnumerable<IFiltroProductoStrategy> filtros)
    {
        _context = context;
        _filtros = filtros;
    }

    public async Task<IReadOnlyList<Producto>> ObtenerCatalogoAsync(ProductoFiltroViewModel filtro)
    {
        var productos = _context.Productos
            .AsNoTracking()
            .Include(p => p.Categoria)
            .Include(p => p.Talla)
            .Include(p => p.Color)
            .Include(p => p.Imagenes)
            .AsQueryable();

        foreach (var estrategia in _filtros)
        {
            productos = estrategia.Aplicar(productos, filtro);
        }

        return await productos.OrderBy(p => p.Nombre).ToListAsync();
    }

    public async Task<IReadOnlyList<Categoria>> ObtenerCategoriasAsync()
    {
        return await _context.Categorias
            .AsNoTracking()
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Talla>> ObtenerTallasAsync()
    {
        return await _context.Tallas
            .AsNoTracking()
            .OrderBy(t => t.Nombre)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Color>> ObtenerColoresAsync()
    {
        return await _context.Colores
            .AsNoTracking()
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }
}
