using CatalogoRopaMVC.Data;
using CatalogoRopaMVC.DTOs;
using CatalogoRopaMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogoRopaMVC.Controllers.Api;

[ApiController]
[Route("api")]
public class CatalogosApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CatalogosApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("categorias")]
    [ProducesResponseType(typeof(IEnumerable<CategoriaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategorias()
    {
        var categorias = await _context.Categorias
            .AsNoTracking()
            .OrderBy(c => c.Nombre)
            .Select(c => new CategoriaDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion
            })
            .ToListAsync();

        return Ok(categorias);
    }

    [HttpGet("categorias/{id:int}")]
    [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoriaDto>> GetCategoria(int id)
    {
        var categoria = await _context.Categorias
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CategoriaDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion
            })
            .FirstOrDefaultAsync();

        return categoria == null ? NotFound() : Ok(categoria);
    }

    [Authorize(Roles = "Vendedor")]
    [HttpPost("categorias")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult<CategoriaDto>> PostCategoria(CategoriaUpsertDto dto)
    {
        var nombre = dto.Nombre.Trim();
        if (await _context.Categorias.AnyAsync(c => c.Nombre == nombre))
        {
            return Conflict(new { Message = "Ya existe una categoría con ese nombre." });
        }

        var categoria = new Categoria
        {
            Nombre = nombre,
            Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion) ? null : dto.Descripcion.Trim()
        };

        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();

        var result = new CategoriaDto
        {
            Id = categoria.Id,
            Nombre = categoria.Nombre,
            Descripcion = categoria.Descripcion
        };

        return Created($"/api/categorias/{categoria.Id}", result);
    }

    [Authorize(Roles = "Vendedor")]
    [HttpPut("categorias/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PutCategoria(int id, CategoriaUpsertDto dto)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria == null)
        {
            return NotFound();
        }

        var nombre = dto.Nombre.Trim();
        if (await _context.Categorias.AnyAsync(c => c.Id != id && c.Nombre == nombre))
        {
            return Conflict(new { Message = "Ya existe una categoría con ese nombre." });
        }

        categoria.Nombre = nombre;
        categoria.Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion) ? null : dto.Descripcion.Trim();
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = "Vendedor")]
    [HttpDelete("categorias/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategoria(int id)
    {
        var categoria = await _context.Categorias
            .Include(c => c.Productos)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (categoria == null)
        {
            return NotFound();
        }

        if (categoria.Productos.Count > 0)
        {
            return Conflict(new { Message = "No puedes eliminar una categoría que tiene productos." });
        }

        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("tallas")]
    [ProducesResponseType(typeof(IEnumerable<TallaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TallaDto>>> GetTallas()
    {
        var tallas = await _context.Tallas
            .AsNoTracking()
            .OrderBy(t => t.Nombre)
            .Select(t => new TallaDto
            {
                Id = t.Id,
                Nombre = t.Nombre
            })
            .ToListAsync();

        return Ok(tallas);
    }

    [HttpGet("colores")]
    [ProducesResponseType(typeof(IEnumerable<ColorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ColorDto>>> GetColores()
    {
        var colores = await _context.Colores
            .AsNoTracking()
            .OrderBy(c => c.Nombre)
            .Select(c => new ColorDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                CodigoHex = c.CodigoHex
            })
            .ToListAsync();

        return Ok(colores);
    }
}
