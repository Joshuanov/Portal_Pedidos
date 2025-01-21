using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SistemaTGU.Entities;

namespace SistemaTGU.Models
{
    public class SolicitudViewModel
    {
       
        public int Id { get; set; }
        public string Solicitud { get; set; }
        public DateTime FechaSolicitud { get; set; } = DateTime.Now;
        public int EstadoId { get; set; }
        public Estado Estado { get; set; }
        public string? SolicitanteId { get; set; }
        public Usuario? Solicitante { get; set; }
        public List<Estado> Estados { get; set; } = new List<Estado>();

        public CabPedidos cabPedidos { get; set; }
        public List<PrendasViewModel>? Prendas { get; set; }
        public HashSet<DetPedidos> DetPedidos { get; set; }


    }
}
