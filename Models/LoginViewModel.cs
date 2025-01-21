using System.ComponentModel.DataAnnotations;

namespace SistemaTGU.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Ingrese un correo válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Recuérdame")] // Este será el texto mostrado al cliente
        public bool RememberMe { get; set; }

    }
}
