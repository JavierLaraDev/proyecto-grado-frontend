using Newtonsoft.Json;
using PersonalizacionProyectoGradoWASM.Helpers;
using PersonalizacionProyectoGradoWASM.Modelos;
using PersonalizacionProyectoGradoWASM.Servicios.IServicios;
using System.Text;

namespace PersonalizacionProyectoGradoWASM.Servicios
{
    public class PedidosServicio : IPedidosServicio
    {
        private readonly HttpClient _cliente;
        public PedidosServicio(HttpClient cliente)
        {
            _cliente = cliente;
        }

        public async Task<PedidosComprasDto> AgregarPedido(PedidosComprasDto pedido)
        {
            try
            {
                Console.WriteLine($"Iniciando AgregarPedido para usuario {pedido.UsuarioId}");
                var content = JsonConvert.SerializeObject(pedido);
                Console.WriteLine($"Pedido serializado: {content}");

                var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
                var response = await _cliente.PostAsync($"{Inicializar.UrlBaseApi}api/pedido", bodyContent);

                Console.WriteLine($"Respuesta del servidor: StatusCode={response.StatusCode}");
                var contentTemp = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Contenido de la respuesta: {contentTemp}");

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<PedidosComprasDto>(contentTemp);
                    Console.WriteLine($"Pedido agregado con éxito. ID: {result.Id}");
                    return result;
                }
                else
                {
                    var errorModel = JsonConvert.DeserializeObject<ModeloError>(contentTemp);
                    Console.WriteLine($"Error al agregar pedido: {errorModel.ErrorMessage}");
                    throw new Exception($"Error al agregar pedido: {errorModel.ErrorMessage}");
                }
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"Error al deserializar la respuesta: {jsonEx.Message}");
                throw new Exception("Error al procesar la respuesta del servidor", jsonEx);
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Error de red al agregar pedido: {httpEx.Message}");
                throw new Exception("Error de conexión al servidor", httpEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado al agregar pedido: {ex.Message}");
                throw new Exception("Error inesperado al procesar el pedido", ex);
            }

        }

        public async Task<PedidosComprasDto> GetPedido(int pedidoId)
        {
            try
            {
                var response = await _cliente.GetAsync($"{Inicializar.UrlBaseApi}api/pedido/{pedidoId}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var pedido = JsonConvert.DeserializeObject<PedidosComprasDto>(content);
                    Console.WriteLine($"Pedido recuperado: ID={pedido.Id}, Items={pedido.Items?.Count ?? 0}");
                    return pedido;
                }
                else
                {
                    Console.WriteLine($"Error al obtener el pedido: Status Code={response.StatusCode}, Content={content}");
                    throw new Exception($"Error al obtener el pedido: {response.StatusCode} - {content}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error de red al obtener el pedido: {ex.Message}");
                throw new Exception($"Error de red al obtener el pedido: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado al obtener el pedido: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> EliminarItemDelPedido(int pedidoId, int accesorioId)
        {
            var response = await _cliente.DeleteAsync($"{Inicializar.UrlBaseApi}api/pedido/{pedidoId}/items/{accesorioId}");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                var errorModel = JsonConvert.DeserializeObject<ModeloError>(content);
                throw new Exception(errorModel.ErrorMessage);
            }
        }

        public async Task<bool> VaciarPedido(int pedidoId)
        {
            var response = await _cliente.DeleteAsync($"{Inicializar.UrlBaseApi}api/pedido/{pedidoId}");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                var errorModel = JsonConvert.DeserializeObject<ModeloError>(content);
                throw new Exception(errorModel.ErrorMessage);
            }
        }

        public async Task<List<PedidosComprasDto>> GetPedidos()
        {
            var response = await _cliente.GetAsync($"{Inicializar.UrlBaseApi}api/pedido/pedidos");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var pedidos = JsonConvert.DeserializeObject<List<PedidosComprasDto>>(content);
                Console.WriteLine($"Pedidos recuperados: {pedidos.Count}");
                foreach (var pedido in pedidos)
                {
                    Console.WriteLine($"Pedido ID={pedido.Id}, Items={pedido.Items?.Count ?? 0}");
                }
                return pedidos;
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error al obtener los pedidos: {content}");
                throw new Exception($"Error al obtener los pedidos: {content}");
            }
        }

        public async Task<bool> ActualizarEstadoPedido(int pedidoId, EstadoPedido nuevoEstado)
        {
            try
            {
                var estadoString = nuevoEstado.ToString();
                var content = JsonConvert.SerializeObject(estadoString);
                var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

                var response = await _cliente.PatchAsync($"{Inicializar.UrlBaseApi}api/pedido/{pedidoId}/estado", bodyContent);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var contentTemp = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error al actualizar el estado del pedido. Respuesta del servidor: {contentTemp}");
                    throw new Exception($"Error al actualizar el estado del pedido: {contentTemp}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al actualizar el estado del pedido: {ex}");
                throw;
            }
        }
        public async Task<List<PedidosComprasDto>> GetPedidosUsuario(int userId)
        {
            try
            {
                var response = await _cliente.GetAsync($"{Inicializar.UrlBaseApi}api/pedido/usuario/{userId}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var pedidos = JsonConvert.DeserializeObject<List<PedidosComprasDto>>(content);
                    Console.WriteLine($"Pedidos del usuario recuperados: {pedidos.Count}");
                    return pedidos;
                }
                else
                {
                    Console.WriteLine($"Error al obtener los pedidos del usuario: Status Code={response.StatusCode}, Content={content}");
                    throw new Exception($"Error al obtener los pedidos del usuario: {response.StatusCode} - {content}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado al obtener los pedidos del usuario: {ex.Message}");
                throw;
            }
        }
    }
}
