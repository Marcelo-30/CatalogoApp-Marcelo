using CatalogoRopaMVC.Data;
using CatalogoRopaMVC.DTOs;
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
}
