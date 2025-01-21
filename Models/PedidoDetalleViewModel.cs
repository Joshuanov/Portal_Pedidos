

using System.Data;

namespace SistemaTGU.Models
{
    public class PedidoDetalleViewModel
    {
       
        public int PedidoId { get; set; }
        public string CodPrenda { get; set; }
        public string DescripcionPrenda { get; set; }
        public int Cantidad { get; set; }
   


        //public int PrecioUnitario { get; set; }
        //public int Total { get; set; }

    }
}
// Obtener datos de las filas visibles
//dataTable.rows({ search: 'none', page: 'all' }).every(function() {
//    const row = [];
//                $(this.node()).find('td').each(function() {
//        row.push($(this).text());
//    });
//    tableData.push(row);
//});