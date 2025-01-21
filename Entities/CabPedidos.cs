namespace SistemaTGU.Entities
{
    public class CabPedidos
    {
        public int Id { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public int EstadoId { get; set; }
        public Estado Estado { get; set; }
        public string SolicitanteId { get; set; }
        public Usuario Solicitante { get; set; }
        public HashSet<DetPedidos> DetPedidos { get; set; }
        public string IdCeco { get; set; } = "0";
        public int CecoEmpresaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Responsable { get; set; } = string.Empty;
    }
}
