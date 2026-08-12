using CatalogoRopaMVC.DTOs;
using CatalogoRopaMVC.Models;

namespace CatalogoRopaMVC.Services.Factories;

public class ProductoDtoFactory : IProductoDtoFactory
{
    public ProductoDto Crear(Producto producto)
    {
        return new ProductoDto
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            Categoria = producto.Categoria?.Nombre ?? string.Empty,
            Talla = producto.Talla?.Nombre ?? string.Empty,
            Color = producto.Color?.Nombre ?? string.Empty,
            Precio = producto.Precio,
            Stock = producto.Stock,
            Disponible = producto.Disponible,
            ImagenUrl = producto.ImagenPrincipalUrl
        };
    }

    public IEnumerable<ProductoDto> CrearColeccion(IEnumerable<Producto> productos)
    {
        return productos.Select(Crear);
    }
}
