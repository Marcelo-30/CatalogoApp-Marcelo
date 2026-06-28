using CatalogoRopaMVC.Data;
using CatalogoRopaMVC.Models;
using CatalogoRopaMVC.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CatalogoRopaMVC.Services;

public class ProductoCatalogoService : IProductoCatalogoService
{
    private readonly ApplicationDbContext _context;

    public ProductoCatalogoService(ApplicationDbContext context)
    {
        _context = context;
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

        if (!string.IsNullOrWhiteSpace(filtro.Buscar))
        {
            productos = productos.Where(p =>
                p.Nombre.Contains(filtro.Buscar) ||
                (p.Descripcion != null && p.Descripcion.Contains(filtro.Buscar)) ||
                (p.Color != null && p.Color.Nombre.Contains(filtro.Buscar)) ||
                (p.Talla != null && p.Talla.Nombre.Contains(filtro.Buscar)));
        }

        if (filtro.CategoriaId.HasValue && filtro.CategoriaId.Value > 0)
        {
            productos = productos.Where(p => p.CategoriaId == filtro.CategoriaId.Value);
        }

        if (filtro.SoloDisponibles)
        {
            productos = productos.Where(p => p.Disponible && p.Stock > 0);
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
