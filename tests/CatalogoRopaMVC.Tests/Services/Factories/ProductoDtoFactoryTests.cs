using CatalogoRopaMVC.Models;
using CatalogoRopaMVC.Services.Factories;
using Xunit;

namespace CatalogoRopaMVC.Tests.Services.Factories;

public class ProductoDtoFactoryTests
{
    [Fact]
    public void Crear_ProductoCompleto_MapeaPropiedadesEImagenPrincipal()
    {
        // Arrange
        var producto = new Producto
        {
            Id = 7,
            Nombre = "Chamarra",
            Descripcion = "Chamarra ligera",
            Categoria = new Categoria { Nombre = "Abrigos" },
            Talla = new Talla { Nombre = "L" },
            Color = new Color { Nombre = "Negro" },
            Precio = 899.90m,
            Stock = 5,
            Disponible = true,
            Imagenes =
            [
                new ImagenProducto { Id = 1, Url = "secundaria.jpg", EsPrincipal = false },
                new ImagenProducto { Id = 2, Url = "principal.jpg", EsPrincipal = true }
            ]
        };
        var sut = new ProductoDtoFactory();

        // Act
        var resultado = sut.Crear(producto);

        // Assert
        Assert.Equal(producto.Id, resultado.Id);
        Assert.Equal(producto.Nombre, resultado.Nombre);
        Assert.Equal(producto.Descripcion, resultado.Descripcion);
        Assert.Equal("Abrigos", resultado.Categoria);
        Assert.Equal("L", resultado.Talla);
        Assert.Equal("Negro", resultado.Color);
        Assert.Equal(producto.Precio, resultado.Precio);
        Assert.Equal(producto.Stock, resultado.Stock);
        Assert.Equal(producto.Disponible, resultado.Disponible);
        Assert.Equal("principal.jpg", resultado.ImagenUrl);
    }

    [Fact]
    public void Crear_NavegacionesAusentes_UsaTextosVaciosEImagenNula()
    {
        // Arrange
        var producto = new Producto { Id = 8, Nombre = "Producto basico" };
        var sut = new ProductoDtoFactory();

        // Act
        var resultado = sut.Crear(producto);

        // Assert
        Assert.Equal(string.Empty, resultado.Categoria);
        Assert.Equal(string.Empty, resultado.Talla);
        Assert.Equal(string.Empty, resultado.Color);
        Assert.Null(resultado.ImagenUrl);
    }

    [Fact]
    public void CrearColeccion_VariosProductos_MapeaCadaElementoEnOrden()
    {
        // Arrange
        var productos = new[]
        {
            new Producto { Id = 1, Nombre = "Primero" },
            new Producto { Id = 2, Nombre = "Segundo" }
        };
        var sut = new ProductoDtoFactory();

        // Act
        var resultado = sut.CrearColeccion(productos).ToList();

        // Assert
        Assert.Collection(
            resultado,
            dto =>
            {
                Assert.Equal(1, dto.Id);
                Assert.Equal("Primero", dto.Nombre);
            },
            dto =>
            {
                Assert.Equal(2, dto.Id);
                Assert.Equal("Segundo", dto.Nombre);
            });
    }
}
