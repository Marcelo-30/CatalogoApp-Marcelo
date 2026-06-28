using CatalogoRopaMVC.Data;
using CatalogoRopaMVC.Models;
using CatalogoRopaMVC.Services;
using CatalogoRopaMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CatalogoRopaMVC.Controllers;

public class ProductosController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IProductoCatalogoService _catalogoService;

    public ProductosController(ApplicationDbContext context, IProductoCatalogoService catalogoService)
    {
        _context = context;
        _catalogoService = catalogoService;
    }

    // GET: Productos
    public async Task<IActionResult> Index([FromQuery] ProductoFiltroViewModel filtro)
    {
        var productos = await _catalogoService.ObtenerCatalogoAsync(filtro);

        ViewData["Buscar"] = filtro.Buscar;
        ViewData["SoloDisponibles"] = filtro.SoloDisponibles;
        ViewData["CategoriaId"] = filtro.CategoriaId;
        ViewBag.Categorias = new SelectList(await _catalogoService.ObtenerCategoriasAsync(), "Id", "Nombre", filtro.CategoriaId);

        return View(productos);
    }

    // GET: Productos/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var producto = await _context.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Talla)
            .Include(p => p.Color)
            .Include(p => p.Imagenes)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (producto == null)
        {
            return NotFound();
        }

        return View(producto);
    }

    // GET: Productos/Create
    [Authorize(Roles = "Vendedor")]
    public async Task<IActionResult> Create()
    {
        await PrepararFormularioAsync();
        return View(new Producto { Disponible = true });
    }

    // POST: Productos/Create
    [Authorize(Roles = "Vendedor")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nombre,Descripcion,Precio,Stock,ImagenUrl,Disponible,CategoriaId,TallaId,ColorId")] Producto producto)
    {
        producto.FechaRegistro = DateTime.Now;

        if (ModelState.IsValid)
        {
            if (!string.IsNullOrWhiteSpace(producto.ImagenUrl))
            {
                producto.Imagenes.Add(new ImagenProducto
                {
                    Url = producto.ImagenUrl,
                    TextoAlternativo = producto.Nombre,
                    EsPrincipal = true
                });
            }

            _context.Add(producto);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Producto registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        await PrepararFormularioAsync(producto.CategoriaId, producto.TallaId, producto.ColorId);
        return View(producto);
    }

    // GET: Productos/Edit/5
    [Authorize(Roles = "Vendedor")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var producto = await _context.Productos
            .Include(p => p.Imagenes)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (producto == null)
        {
            return NotFound();
        }

        producto.ImagenUrl = producto.ImagenPrincipalUrl;
        await PrepararFormularioAsync(producto.CategoriaId, producto.TallaId, producto.ColorId);
        return View(producto);
    }

    // POST: Productos/Edit/5
    [Authorize(Roles = "Vendedor")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion,Precio,Stock,ImagenUrl,Disponible,FechaRegistro,CategoriaId,TallaId,ColorId")] Producto producto)
    {
        if (id != producto.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var productoDb = await _context.Productos
                .Include(p => p.Imagenes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (productoDb == null)
            {
                return NotFound();
            }

            productoDb.Nombre = producto.Nombre;
            productoDb.Descripcion = producto.Descripcion;
            productoDb.Precio = producto.Precio;
            productoDb.Stock = producto.Stock;
            productoDb.Disponible = producto.Disponible;
            productoDb.FechaRegistro = producto.FechaRegistro;
            productoDb.CategoriaId = producto.CategoriaId;
            productoDb.TallaId = producto.TallaId;
            productoDb.ColorId = producto.ColorId;

            ActualizarImagenPrincipal(productoDb, producto.ImagenUrl);

            try
            {
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Producto actualizado correctamente.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(producto.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        await PrepararFormularioAsync(producto.CategoriaId, producto.TallaId, producto.ColorId);
        return View(producto);
    }

    // GET: Productos/Delete/5
    [Authorize(Roles = "Vendedor")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var producto = await _context.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Talla)
            .Include(p => p.Color)
            .Include(p => p.Imagenes)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (producto == null)
        {
            return NotFound();
        }

        return View(producto);
    }

    // POST: Productos/Delete/5
    [Authorize(Roles = "Vendedor")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto != null)
        {
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Producto eliminado correctamente.";
        }

        return RedirectToAction(nameof(Index));
    }

    private bool ProductoExists(int id)
    {
        return _context.Productos.Any(e => e.Id == id);
    }

    private async Task PrepararFormularioAsync(int? categoriaSeleccionada = null, int? tallaSeleccionada = null, int? colorSeleccionado = null)
    {
        ViewBag.Categorias = new SelectList(
            await _catalogoService.ObtenerCategoriasAsync(),
            "Id",
            "Nombre",
            categoriaSeleccionada);

        ViewBag.Tallas = new SelectList(
            await _catalogoService.ObtenerTallasAsync(),
            "Id",
            "Nombre",
            tallaSeleccionada);

        ViewBag.Colores = new SelectList(
            await _catalogoService.ObtenerColoresAsync(),
            "Id",
            "Nombre",
            colorSeleccionado);
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
