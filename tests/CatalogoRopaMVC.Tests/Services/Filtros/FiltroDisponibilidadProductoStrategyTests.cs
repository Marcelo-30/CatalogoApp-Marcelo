using CatalogoRopaMVC.Models;
using CatalogoRopaMVC.Services.Filtros;
using CatalogoRopaMVC.ViewModels;
using Xunit;

namespace CatalogoRopaMVC.Tests.Services.Filtros;

public class FiltroDisponibilidadProductoStrategyTests
{
    [Fact]
    public void Aplicar_FiltroDesactivado_ConservaTodosLosProductos()
    {
        // Arrange
        var productos = CrearProductos().AsQueryable();
        var filtro = new ProductoFiltroViewModel { SoloDisponibles = false };
        var sut = new FiltroDisponibilidadProductoStrategy();

        // Act
        var resultado = sut.Aplicar(productos, filtro).ToList();

        // Assert
        Assert.Equal(3, resultado.Count);
    }

    [Fact]
    public void Aplicar_FiltroActivado_ExigeMarcaDisponibleYStockPositivo()
    {
        // Arrange
        var productos = CrearProductos().AsQueryable();
        var filtro = new ProductoFiltroViewModel { SoloDisponibles = true };
        var sut = new FiltroDisponibilidadProductoStrategy();

        // Act
        var resultado = sut.Aplicar(productos, filtro).ToList();

        // Assert
        var producto = Assert.Single(resultado);
        Assert.Equal(1, producto.Id);
    }

    private static List<Producto> CrearProductos()
    {
        return
        [
            new Producto { Id = 1, Nombre = "Disponible", Disponible = true, Stock = 4 },
            new Producto { Id = 2, Nombre = "Sin stock", Disponible = true, Stock = 0 },
            new Producto { Id = 3, Nombre = "Oculto", Disponible = false, Stock = 8 }
        ];
    }
}
