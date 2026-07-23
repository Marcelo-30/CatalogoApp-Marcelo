using CatalogoRopaMVC.Models;
using CatalogoRopaMVC.Services.Filtros;
using CatalogoRopaMVC.ViewModels;
using Xunit;

namespace CatalogoRopaMVC.Tests.Services.Filtros;

public class FiltroCategoriaProductoStrategyTests
{
    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    [InlineData(-1)]
    public void Aplicar_CategoriaNoValida_ConservaTodosLosProductos(int? categoriaId)
    {
        // Arrange
        var productos = CrearProductos().AsQueryable();
        var filtro = new ProductoFiltroViewModel { CategoriaId = categoriaId };
        var sut = new FiltroCategoriaProductoStrategy();

        // Act
        var resultado = sut.Aplicar(productos, filtro).ToList();

        // Assert
        Assert.Equal(3, resultado.Count);
    }

    [Fact]
    public void Aplicar_CategoriaExistente_DevuelveSoloProductosDeLaCategoria()
    {
        // Arrange
        var productos = CrearProductos().AsQueryable();
        var filtro = new ProductoFiltroViewModel { CategoriaId = 2 };
        var sut = new FiltroCategoriaProductoStrategy();

        // Act
        var resultado = sut.Aplicar(productos, filtro).ToList();

        // Assert
        Assert.Collection(
            resultado,
            producto => Assert.Equal(2, producto.Id),
            producto => Assert.Equal(3, producto.Id));
    }

    [Fact]
    public void Aplicar_CategoriaInexistente_DevuelveColeccionVacia()
    {
        // Arrange
        var productos = CrearProductos().AsQueryable();
        var filtro = new ProductoFiltroViewModel { CategoriaId = 99 };
        var sut = new FiltroCategoriaProductoStrategy();

        // Act
        var resultado = sut.Aplicar(productos, filtro).ToList();

        // Assert
        Assert.Empty(resultado);
    }

    private static List<Producto> CrearProductos()
    {
        return
        [
            new Producto { Id = 1, Nombre = "Playera", CategoriaId = 1 },
            new Producto { Id = 2, Nombre = "Jeans", CategoriaId = 2 },
            new Producto { Id = 3, Nombre = "Pantalon", CategoriaId = 2 }
        ];
    }
}
