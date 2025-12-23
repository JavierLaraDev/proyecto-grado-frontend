using PersonalizacionProyectoGradoWASM.Modelos;

namespace PersonalizacionProyectoGradoWASM.Servicios.IServicios
{
    public interface IPedidosServicio
    {
        Task<PedidosComprasDto> AgregarPedido(PedidosComprasDto carrito);
        Task<PedidosComprasDto> GetPedido(int userId);
        Task<List<PedidosComprasDto>> GetPedidos();
        Task<bool> ActualizarEstadoPedido(int pedidoId, EstadoPedido nuevoEstado);
        Task<bool> VaciarPedido(int carritoId);
        Task<bool> EliminarItemDelPedido(int carritoId, int accesorioId);
        Task<List<PedidosComprasDto>> GetPedidosUsuario(int userId);
    }
}
