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

        // Servicios/PedidosServicio.cs - Método AgregarPedido
        public async Task<PedidosComprasDto> AgregarPedido(PedidoCrearDto pedido)
        {
            try
            {
                Console.WriteLine("=".PadRight(60, '='));
                Console.WriteLine("📤 DATOS QUE SE ENVIARÁN AL SERVIDOR:");
                Console.WriteLine("=".PadRight(60, '='));
                Console.WriteLine($"   UsuarioId: {pedido.UsuarioId}");
                Console.WriteLine($"   PrecioTotal: {pedido.PrecioTotal}");
                Console.WriteLine($"   ColorBicicleta: {pedido.ColorBicicleta}");
                Console.WriteLine($"   Total Items: {pedido.Items?.Count ?? 0}");

                if (pedido.Items != null)
                {
                    for (int i = 0; i < pedido.Items.Count; i++)
                    {
                        var item = pedido.Items[i];
                        Console.WriteLine($"   Item {i}: AccesorioId={item.AccesorioId}, Cantidad={item.Cantidad}");
                    }
                }

                var json = JsonConvert.SerializeObject(pedido, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented
                });

                Console.WriteLine("📄 JSON QUE SE ENVIARÁ:");
                Console.WriteLine(json);
                Console.WriteLine("=".PadRight(60, '='));

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _cliente.PostAsync(
                    $"{Inicializar.UrlBaseApi}api/pedido",
                    content
                );

                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"📥 RESPUESTA: Status {(int)response.StatusCode}");
                Console.WriteLine(responseContent);

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<PedidosComprasDto>(responseContent);
                }

                try
                {
                    var error = JsonConvert.DeserializeObject<ModeloError>(responseContent);
                    throw new Exception(error?.ErrorMessage ?? "Error desconocido del servidor");
                }
                catch (JsonException)
                {
                    throw new Exception("Error del servidor: " + responseContent);
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"❌ Error de conexión: {ex.Message}");
                throw new Exception("No se pudo conectar con el servidor");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                throw;
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