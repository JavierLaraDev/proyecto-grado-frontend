using PersonalizacionProyectoGradoWASM.Modelos;

namespace PersonalizacionProyectoGradoWASM.Pages.Admin
{
    public partial class Reporte
    {
        public double TotalVentas { get; set; }
        public Dictionary<string, int> ProductosVendidos { get; set; }
        public Dictionary<int, double> VentasPorCliente { get; set; }
        public List<PedidosComprasDto> Pedidos { get; set; }
    }
}
