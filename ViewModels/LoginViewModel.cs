using System.ComponentModel.DataAnnotations;

namespace CatalogoRopaMVC.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Escribe un correo válido.")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecciona el tipo de usuario.")]
    [Display(Name = "Tipo de usuario")]
    public string Rol { get; set; } = "Cliente";
}
