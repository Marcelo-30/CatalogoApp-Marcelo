using System.ComponentModel.DataAnnotations;

namespace CatalogoRopaMVC.Models;

public class Talla
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre de la talla es obligatorio.")]
    [StringLength(20, ErrorMessage = "La talla no puede superar los 20 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
