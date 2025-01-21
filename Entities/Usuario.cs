using Microsoft.AspNetCore.Identity;

namespace SistemaTGU.Entities
{
    public class Usuario : IdentityUser
    {
        public string Nombre { get; set; }
    }
}
