using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PersonalizacionProyectoGradoWASM.Helpers;
using PersonalizacionProyectoGradoWASM.Modelos;
using System.Text;
using PersonalizacionProyectoGradoWASM.Servicios.IServicios;

namespace PersonalizacionProyectoGradoWASM.Servicios
{
    public class ServicioAutenticacion : IServicioAutenticacion
    {
        private readonly HttpClient _cliente;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _estadoProveedorAutenticacion;

        public ServicioAutenticacion(HttpClient cliente,
            ILocalStorageService localStorage,
            AuthenticationStateProvider estadoProveedorAutenticacion)
        {
            _cliente = cliente;
            _localStorage = localStorage;
            _estadoProveedorAutenticacion = estadoProveedorAutenticacion;
        }
        public async Task<RespuestaAutenticacion> Acceder(UsuarioAutenticacion usuarioDesdeAutenticacion)
        {
            try
            {
                var content = JsonConvert.SerializeObject(usuarioDesdeAutenticacion);
                var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
                var response = await _cliente.PostAsync($"{Inicializar.UrlBaseApi}api/usuarios/login", bodyContent);
                var contentTemp = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var resultado = JObject.Parse(contentTemp);

                    var token = resultado["result"]?["token"]?.Value<string>();
                    if (string.IsNullOrEmpty(token))
                    {
                        return new RespuestaAutenticacion { IsSuccess = false, ErrorMessage = "Token no recibido" };
                    }

                    var nombreUsuario = resultado["result"]?["usuario"]?["nombreUsuario"]?.Value<string>();
                    var rol = resultado["result"]?["usuario"]?["rol"]?.Value<string>();
                    var idUsuario = resultado["result"]?["usuario"]?["id"]?.Value<string>();

                    if (string.IsNullOrEmpty(nombreUsuario) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(idUsuario))
                    {
                        return new RespuestaAutenticacion { IsSuccess = false, ErrorMessage = "Datos de usuario, rol o ID no recibidos" };
                    }

                    await _localStorage.SetItemAsync(Inicializar.Token_Local, token);
                    await _localStorage.SetItemAsync(Inicializar.Datos_Usuario_Local, nombreUsuario);
                    await _localStorage.SetItemAsync(Inicializar.Rol_Usuario_Local, rol);
                    await _localStorage.SetItemAsync(Inicializar.Id_Usuario_Local, idUsuario);

                    await ((AuthStateProvider)_estadoProveedorAutenticacion).NotificarUsuarioLogueado(token);

                    return new RespuestaAutenticacion
                    {
                        IsSuccess = true,
                        Token = token,
                        Usuario = new Usuario
                        {
                            Id = int.Parse(idUsuario),
                            NombreUsuario = nombreUsuario,
                            Rol = Enum.TryParse<RolEnum>(rol, true, out var parsedRole) ? parsedRole : RolEnum.cliente
                        }
                    };
                }
                else
                {
                    return new RespuestaAutenticacion { IsSuccess = false, ErrorMessage = "Error en la autenticación" };
                }
            }
            catch (JsonException jsonEx)
            {
                return new RespuestaAutenticacion
                {
                    IsSuccess = false,
                    ErrorMessage = $"Error procesando respuesta JSON: {jsonEx.Message}"
                };
            }
            catch (Exception ex)
            {
                return new RespuestaAutenticacion
                {
                    IsSuccess = false,
                    ErrorMessage = $"Error inesperado: {ex.Message}"
                };
            }
        }
        public async Task<RespuestaRegistro> RegistrarUsuario(UsuarioRegistro usuarioParaRegistro)
        {
            var content = JsonConvert.SerializeObject(usuarioParaRegistro);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await _cliente.PostAsync($"{Inicializar.UrlBaseApi}api/usuarios/registro", bodyContent);
            var contentTemp = await response.Content.ReadAsStringAsync();
            var resultado = JsonConvert.DeserializeObject<RespuestaRegistro>(contentTemp);

            if (response.IsSuccessStatusCode)
            {
                return new RespuestaRegistro { registroCorrecto = true };
            }
            else
            {
                return resultado;
            }
        }

        public async Task Salir()
        {
            await _localStorage.RemoveItemAsync(Inicializar.Token_Local);
            await _localStorage.RemoveItemAsync(Inicializar.Datos_Usuario_Local);
            await _localStorage.RemoveItemAsync(Inicializar.Rol_Usuario_Local);
            await _localStorage.RemoveItemAsync(Inicializar.Id_Usuario_Local);
            ((AuthStateProvider)_estadoProveedorAutenticacion).NotificarUsuarioSalir();
            _cliente.DefaultRequestHeaders.Authorization = null;
        }
    }
}
