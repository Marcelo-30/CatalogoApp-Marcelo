using CatalogoRopaMVC.DTOs;
using CatalogoRopaMVC.Models;

namespace CatalogoRopaMVC.Services.Factories;

public interface IProductoDtoFactory
{
    ProductoDto Crear(Producto producto);

    IEnumerable<ProductoDto> CrearColeccion(IEnumerable<Producto> productos);
}
