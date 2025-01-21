using SistemaTGU.Entities;

namespace SistemaTGU.Models
{
    public class CabeceraDetalleViewModel
    {
        public int Id { get; set; }       
        public DateTime FechaSolicitud { get; set; }
        public string EstadoId { get; set; }
        public Estado Estado { get; set; }
        public string Solicitante { get; set; }
        //public HashSet<DetPedidos> DetPedidos { get; set; }
        public string DenominacionCeco { get; set; }
        public string IdCeco { get; set; }
        public string NombreEmpresa { get; set; }
        //public string Nombre { get; set; } = string.Empty;
        //public string Direccion { get; set; } = string.Empty;
        //public string Responsable { get; set; } = string.Empty;
        public List<PedidoDetalleViewModel> Detalles { get; set; }
    }
}
