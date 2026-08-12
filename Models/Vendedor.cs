using System.ComponentModel.DataAnnotations;

namespace CatalogoRopaMVC.Models;

public class Vendedor
{
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(120)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string EmailNormalizado { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [StringLength(40)]
    public string LlaveUnica { get; set; } = "VENDEDOR_DUENO";

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public DateTime? UltimoAcceso { get; set; }
}
