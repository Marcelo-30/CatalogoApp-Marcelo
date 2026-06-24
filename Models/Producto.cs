using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CatalogoRopaMVC.Models;

public class Producto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
    [StringLength(80, ErrorMessage = "El nombre no puede superar los 80 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(300, ErrorMessage = "La descripción no puede superar los 300 caracteres.")]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "El precio es obligatorio.")]
    [Range(0.01, 999999, ErrorMessage = "El precio debe ser mayor a 0.")]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Precio { get; set; }

    [Required(ErrorMessage = "La talla es obligatoria.")]
    [StringLength(20, ErrorMessage = "La talla no puede superar los 20 caracteres.")]
    public string Talla { get; set; } = string.Empty;

    [Required(ErrorMessage = "El color es obligatorio.")]
    [StringLength(40, ErrorMessage = "El color no puede superar los 40 caracteres.")]
    public string Color { get; set; } = string.Empty;

    [Range(0, 9999, ErrorMessage = "El stock no puede ser negativo.")]
    public int Stock { get; set; }

    [Display(Name = "Imagen URL")]
    [StringLength(500, ErrorMessage = "La URL de la imagen no puede superar los 500 caracteres.")]
    public string? ImagenUrl { get; set; }

    [Display(Name = "Disponible")]
    public bool Disponible { get; set; } = true;

    [Display(Name = "Fecha de registro")]
    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "La categoría es obligatoria.")]
    [Display(Name = "Categoría")]
    public int CategoriaId { get; set; }

    public Categoria? Categoria { get; set; }
}
