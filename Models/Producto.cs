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

    [Range(0, 9999, ErrorMessage = "El stock no puede ser negativo.")]
    public int Stock { get; set; }

    [Display(Name = "Disponible")]
    public bool Disponible { get; set; } = true;

    [Display(Name = "Fecha de registro")]
    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "La categoría es obligatoria.")]
    [Display(Name = "Categoría")]
    public int CategoriaId { get; set; }

    [Required(ErrorMessage = "La talla es obligatoria.")]
    [Display(Name = "Talla")]
    public int TallaId { get; set; }

    [Required(ErrorMessage = "El color es obligatorio.")]
    [Display(Name = "Color")]
    public int ColorId { get; set; }

    public Categoria? Categoria { get; set; }
    public Talla? Talla { get; set; }
    public Color? Color { get; set; }

    public ICollection<ImagenProducto> Imagenes { get; set; } = new List<ImagenProducto>();

    // Campo auxiliar para capturar la URL principal desde el formulario.
    // No se guarda en la tabla Productos; se guarda en ImagenProducto.
    [NotMapped]
    [Display(Name = "Imagen URL")]
    [StringLength(500, ErrorMessage = "La URL de la imagen no puede superar los 500 caracteres.")]
    public string? ImagenUrl { get; set; }

    [NotMapped]
    public string? ImagenPrincipalUrl =>
        !string.IsNullOrWhiteSpace(ImagenUrl)
            ? ImagenUrl
            : Imagenes
                .OrderByDescending(i => i.EsPrincipal)
                .ThenBy(i => i.Id)
                .FirstOrDefault()?.Url;
}
