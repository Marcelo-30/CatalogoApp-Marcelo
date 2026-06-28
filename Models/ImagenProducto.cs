using System.ComponentModel.DataAnnotations;

namespace CatalogoRopaMVC.Models;

public class ImagenProducto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La URL de la imagen es obligatoria.")]
    [StringLength(500, ErrorMessage = "La URL de la imagen no puede superar los 500 caracteres.")]
    public string Url { get; set; } = string.Empty;

    [Display(Name = "Texto alternativo")]
    [StringLength(120, ErrorMessage = "El texto alternativo no puede superar los 120 caracteres.")]
    public string? TextoAlternativo { get; set; }

    [Display(Name = "Imagen principal")]
    public bool EsPrincipal { get; set; } = true;

    [Required]
    public int ProductoId { get; set; }

    public Producto? Producto { get; set; }
}
