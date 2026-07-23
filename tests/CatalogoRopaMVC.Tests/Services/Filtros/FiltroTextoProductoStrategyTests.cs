using CatalogoRopaMVC.Models;
using CatalogoRopaMVC.Services.Filtros;
using CatalogoRopaMVC.ViewModels;
using Xunit;

namespace CatalogoRopaMVC.Tests.Services.Filtros;

public class FiltroTextoProductoStrategyTests
{
    [Fact]
    public void Aplicar_BusquedaVacia_ConservaTodosLosProductos()
    {
        // Arrange
        var productos = CrearProductos().AsQueryable();
        var filtro = new ProductoFiltroViewModel { Buscar = "   " };
        var sut = new FiltroTextoProductoStrategy();

        // Act
        var resultado = sut.Aplicar(productos, filtro).ToList();

        // Assert
        Assert.Equal(2, resultado.Count);
    }

    [Theory]
    [InlineData("Playera")]
    [InlineData("Algodon")]
    [InlineData("Rojo")]
    [InlineData("Mediana")]
    public void Aplicar_TextoCoincideEnCampoDelProducto_DevuelveProductoEsperado(string busqueda)
    {
        // Arrange
        var productos = CrearProductos().AsQueryable();
        var filtro = new ProductoFiltroViewModel { Buscar = busqueda };
        var sut = new FiltroTextoProductoStrategy();

        // Act
        var resultado = sut.Aplicar(productos, filtro).ToList();

        // Assert
        var producto = Assert.Single(resultado);
        Assert.Equal(1, producto.Id);
    }

    [Fact]
    public void Aplicar_TextoSinCoincidencias_DevuelveColeccionVacia()
    {
        // Arrange
        var productos = CrearProductos().AsQueryable();
        var filtro = new ProductoFiltroViewModel { Buscar = "Sombrero" };
        var sut = new FiltroTextoProductoStrategy();

        // Act
        var resultado = sut.Aplicar(productos, filtro).ToList();

        // Assert
        Assert.Empty(resultado);
    }

    private static List<Producto> CrearProductos()
    {
        return
        [
            new Producto
            {
                Id = 1,
                Nombre = "Playera urbana",
                Descripcion = "Algodon suave",
                Color = new Color { Nombre = "Rojo" },
                Talla = new Talla { Nombre = "Mediana" }
            },
            new Producto
            {
                Id = 2,
                Nombre = "Jeans clasicos",
                Descripcion = "Mezclilla resistente",
                Color = new Color { Nombre = "Azul" },
                Talla = new Talla { Nombre = "Grande" }
            }
        ];
    }
}
