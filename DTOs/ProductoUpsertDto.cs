using System.ComponentModel.DataAnnotations;

namespace CatalogoRopaMVC.DTOs;

public class ProductoUpsertDto
{
    [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
    [StringLength(80, ErrorMessage = "El nombre no puede superar los 80 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(300, ErrorMessage = "La descripcion no puede superar los 300 caracteres.")]
    public string? Descripcion { get; set; }

    [Range(0.01, 999999, ErrorMessage = "El precio debe ser mayor a 0.")]
    public decimal Precio { get; set; }

    [Range(0, 9999, ErrorMessage = "El stock no puede ser negativo.")]
    public int Stock { get; set; }

    public bool Disponible { get; set; } = true;

    [Required(ErrorMessage = "La categoria es obligatoria.")]
    public int CategoriaId { get; set; }

    [Required(ErrorMessage = "La talla es obligatoria.")]
    public int TallaId { get; set; }

    [Required(ErrorMessage = "El color es obligatorio.")]
    public int ColorId { get; set; }

    [StringLength(500, ErrorMessage = "La URL de la imagen no puede superar los 500 caracteres.")]
    public string? ImagenUrl { get; set; }
}
