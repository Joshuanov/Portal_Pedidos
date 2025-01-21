namespace SistemaTGU.Models
{
    public class PedidoViewModel
    {
        public IEnumerable<EmpresaViewModel>? Empresa { get; set; }
        public int EmpresaId { get; set; }
        public string CecoId { get; set; }
        public IEnumerable<PrendasViewModel> Prendas { get; set; }

        public string EmpresaNombre { get; set; }

        public string CecoNombre { get; set; }
    }
}
