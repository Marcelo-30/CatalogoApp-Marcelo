using CatalogoRopaMVC.Data;
using CatalogoRopaMVC.DTOs;
using CatalogoRopaMVC.Models;
using CatalogoRopaMVC.Services.Factories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogoRopaMVC.Controllers.Api;

[ApiController]
[Route("api/productos")]
public class ProductosApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IProductoDtoFactory _productoDtoFactory;

    public ProductosApiController(ApplicationDbContext context, IProductoDtoFactory productoDtoFactory)
    {
        _context = context;
        _productoDtoFactory = productoDtoFactory;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductoDto>>> GetProductos()
    {
        var productos = await _context.Productos
            .AsNoTracking()
            .Include(p => p.Categoria)
            .Include(p => p.Talla)
            .Include(p => p.Color)
            .Include(p => p.Imagenes)
            .OrderBy(p => p.Nombre)
            .ToListAsync();

        return Ok(_productoDtoFactory.CrearColeccion(productos));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductoDto>> GetProducto(int id)
    {
        var producto = await ObtenerProductoAsync(id);
        if (producto == null)
        {
            return NotFound();
        }

        return Ok(_productoDtoFactory.Crear(producto));
    }

    [Authorize(Roles = "Vendedor")]
    [HttpPost]
    [ProducesResponseType(typeof(ProductoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ProductoDto>> PostProducto(ProductoUpsertDto dto)
    {
        var errores = await ValidarReferenciasAsync(dto);
        if (errores.Count > 0)
        {
            return BadRequest(errores);
        }

        var producto = new Producto
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            Precio = dto.Precio,
            Stock = dto.Stock,
            Disponible = dto.Disponible,
            FechaRegistro = DateTime.Now,
            CategoriaId = dto.CategoriaId,
            TallaId = dto.TallaId,
            ColorId = dto.ColorId
        };

        ActualizarImagenPrincipal(producto, dto.ImagenUrl);

        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();

        var productoCreado = await ObtenerProductoAsync(producto.Id);

        return CreatedAtAction(
            nameof(GetProducto),
            new { id = producto.Id },
            _productoDtoFactory.Crear(productoCreado!));
    }

    [Authorize(Roles = "Vendedor")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutProducto(int id, ProductoUpsertDto dto)
    {
        var producto = await ObtenerProductoAsync(id, tracked: true);
        if (producto == null)
        {
            return NotFound();
        }

        var errores = await ValidarReferenciasAsync(dto);
        if (errores.Count > 0)
        {
            return BadRequest(errores);
        }

        producto.Nombre = dto.Nombre;
        producto.Descripcion = dto.Descripcion;
        producto.Precio = dto.Precio;
        producto.Stock = dto.Stock;
        producto.Disponible = dto.Disponible;
        producto.CategoriaId = dto.CategoriaId;
        producto.TallaId = dto.TallaId;
        producto.ColorId = dto.ColorId;

        ActualizarImagenPrincipal(producto, dto.ImagenUrl);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = "Vendedor")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProducto(int id)
    {
        var producto = await ObtenerProductoAsync(id, tracked: true);
        if (producto == null)
        {
            return NotFound();
        }

        _context.Productos.Remove(producto);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<Producto?> ObtenerProductoAsync(int id, bool tracked = false)
    {
        var query = _context.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Talla)
            .Include(p => p.Color)
            .Include(p => p.Imagenes)
            .AsQueryable();

        if (!tracked)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(p => p.Id == id);
    }

    private async Task<Dictionary<string, string[]>> ValidarReferenciasAsync(ProductoUpsertDto dto)
    {
        var errores = new Dictionary<string, string[]>();

        if (!await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaId))
        {
            errores[nameof(dto.CategoriaId)] = new[] { "La categoria indicada no existe." };
        }

        if (!await _context.Tallas.AnyAsync(t => t.Id == dto.TallaId))
        {
            errores[nameof(dto.TallaId)] = new[] { "La talla indicada no existe." };
        }

        if (!await _context.Colores.AnyAsync(c => c.Id == dto.ColorId))
        {
            errores[nameof(dto.ColorId)] = new[] { "El color indicado no existe." };
        }

        return errores;
    }

    private static void ActualizarImagenPrincipal(Producto producto, string? imagenUrl)
    {
        var imagenPrincipal = producto.Imagenes
            .OrderByDescending(i => i.EsPrincipal)
            .ThenBy(i => i.Id)
            .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(imagenUrl))
        {
            if (imagenPrincipal != null)
            {
                producto.Imagenes.Remove(imagenPrincipal);
            }

            return;
        }

        if (imagenPrincipal == null)
        {
            producto.Imagenes.Add(new ImagenProducto
            {
                Url = imagenUrl,
                TextoAlternativo = producto.Nombre,
                EsPrincipal = true
            });
        }
        else
        {
            imagenPrincipal.Url = imagenUrl;
            imagenPrincipal.TextoAlternativo = producto.Nombre;
            imagenPrincipal.EsPrincipal = true;
        }
    }
}
