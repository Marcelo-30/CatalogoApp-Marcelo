namespace CatalogoRopaMVC.DTOs;

public class ProductoDto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public string Categoria { get; set; } = string.Empty;

    public string Talla { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;

    public decimal Precio { get; set; }

    public int Stock { get; set; }

    public bool Disponible { get; set; }

    public string? ImagenUrl { get; set; }
}
