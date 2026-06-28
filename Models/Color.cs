using System.ComponentModel.DataAnnotations;

namespace CatalogoRopaMVC.Models;

public class Color
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del color es obligatorio.")]
    [StringLength(40, ErrorMessage = "El color no puede superar los 40 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Display(Name = "Código hexadecimal")]
    [StringLength(7, ErrorMessage = "El código hexadecimal no puede superar los 7 caracteres.")]
    public string? CodigoHex { get; set; }

    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
