namespace SistemaTGU.Entities
{
    public class DetPedidos
    {
        public int Id { get; set; }
        public string CodPrenda { get; set; }     
        public int Cantidad { get; set; }
        public int PrecioUnitario { get; set; }
        public int Total { get; set; }  = 0;
        //public string? FuncionarioId { get; set; }
        public int CabPedidosId { get; set; }
        public CabPedidos CabPedidos { get; set; }

    }
}
