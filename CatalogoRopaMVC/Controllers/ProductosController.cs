using CatalogoRopaMVC.Data;
using CatalogoRopaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CatalogoRopaMVC.Controllers;

public class ProductosController : Controller
{
    private readonly CatalogoRopaContext _context;

    public ProductosController(CatalogoRopaContext context)
    {
        _context = context;
    }

    // GET: Productos
    public async Task<IActionResult> Index(string? buscar, int? categoriaId, bool soloDisponibles = false)
    {
        var productos = _context.Productos
            .Include(p => p.Categoria)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            productos = productos.Where(p =>
                p.Nombre.Contains(buscar) ||
                (p.Descripcion != null && p.Descripcion.Contains(buscar)) ||
                p.Color.Contains(buscar) ||
                p.Talla.Contains(buscar));
        }

        if (categoriaId.HasValue && categoriaId.Value > 0)
        {
            productos = productos.Where(p => p.CategoriaId == categoriaId.Value);
        }

        if (soloDisponibles)
        {
            productos = productos.Where(p => p.Disponible && p.Stock > 0);
        }

        ViewData["Buscar"] = buscar;
        ViewData["SoloDisponibles"] = soloDisponibles;
        ViewData["CategoriaId"] = categoriaId;
        ViewBag.Categorias = new SelectList(await _context.Categorias.OrderBy(c => c.Nombre).ToListAsync(), "Id", "Nombre", categoriaId);

        return View(await productos.OrderBy(p => p.Nombre).ToListAsync());
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
            .FirstOrDefaultAsync(m => m.Id == id);

        if (producto == null)
        {
            return NotFound();
        }

        return View(producto);
    }

    // GET: Productos/Create
    public async Task<IActionResult> Create()
    {
        await CargarCategoriasAsync();
        return View(new Producto());
    }

    // POST: Productos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nombre,Descripcion,Precio,Talla,Color,Stock,ImagenUrl,Disponible,CategoriaId")] Producto producto)
    {
        producto.FechaRegistro = DateTime.Now;

        if (ModelState.IsValid)
        {
            _context.Add(producto);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Producto registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        await CargarCategoriasAsync(producto.CategoriaId);
        return View(producto);
    }

    // GET: Productos/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var producto = await _context.Productos.FindAsync(id);
        if (producto == null)
        {
            return NotFound();
        }

        await CargarCategoriasAsync(producto.CategoriaId);
        return View(producto);
    }

    // POST: Productos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion,Precio,Talla,Color,Stock,ImagenUrl,Disponible,FechaRegistro,CategoriaId")] Producto producto)
    {
        if (id != producto.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(producto);
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

        await CargarCategoriasAsync(producto.CategoriaId);
        return View(producto);
    }

    // GET: Productos/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var producto = await _context.Productos
            .Include(p => p.Categoria)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (producto == null)
        {
            return NotFound();
        }

        return View(producto);
    }

    // POST: Productos/Delete/5
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

    private async Task CargarCategoriasAsync(int? categoriaSeleccionada = null)
    {
        ViewBag.Categorias = new SelectList(
            await _context.Categorias.OrderBy(c => c.Nombre).ToListAsync(),
            "Id",
            "Nombre",
            categoriaSeleccionada);
    }
}
