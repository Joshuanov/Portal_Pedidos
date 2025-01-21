using SistemaTGU.Entities;

namespace SistemaTGU.Models
{
    public class PrendasViewModel
    {
        //public IEnumerable<Usuario> Nombres { get; set; }
        public int Id { get; set; }
        public string CodigoUsr { get; set; } //cod prenda
        public string DescripcionPrenda { get; set; }    // Nombre prenda   
        public int CantidadIngresada { get; set; } // Cantidad de prendas ingresadas por usuario
    }
}
