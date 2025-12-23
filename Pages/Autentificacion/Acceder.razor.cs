using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using PersonalizacionProyectoGradoWASM.Modelos;
using PersonalizacionProyectoGradoWASM.Servicios.IServicios;
using PersonalizacionProyectoGradoWASM.Servicios;
using Blazored.LocalStorage;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using PersonalizacionProyectoGradoWASM.Helpers;

namespace PersonalizacionProyectoGradoWASM.Pages.Autentificacion
{
    public partial class Acceder
    {
        private UsuarioAutenticacion usuarioAutenticacion = new UsuarioAutenticacion();
        public bool EstaProcesando { get; set; } = false;
        public bool MostrarErroresAutenticacion { get; set; }
        public string UrlRetorno { get; set; }
        public string Errores { get; set; }

        [Inject] public IServicioAutenticacion servicioAutenticacion { get; set; }
        [Inject] public NavigationManager navigationManager { get; set; }
        [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject] private ILocalStorageService _localStorageService { get; set; }

        private async Task AccesoUsuario()
        {
            try
            {
                MostrarErroresAutenticacion = false;
                EstaProcesando = true;
                var result = await servicioAutenticacion.Acceder(usuarioAutenticacion);
                if (result.IsSuccess)
                {
                    EstaProcesando = false;

                    // El ID del usuario y el rol ya deberían estar almacenados en el localStorage por el ServicioAutenticacion
                    var userRole = await _localStorageService.GetItemAsync<string>(Inicializar.Rol_Usuario_Local);
                    var userId = await _localStorageService.GetItemAsync<string>(Inicializar.Id_Usuario_Local);

                    if (string.IsNullOrEmpty(userId))
                    {
                        Console.WriteLine("No se pudo obtener el ID del usuario del almacenamiento local.");
                    }

                    // Forzar una actualización del estado de autenticación
                    await ((AuthStateProvider)AuthenticationStateProvider).NotificarUsuarioLogueado(result.Token);

                    // Navegar según el rol del usuario
                    switch (userRole)
                    {
                        case "0" :
                            navigationManager.NavigateTo("/usuarios");
                            break;
                        case "1" :
                            navigationManager.NavigateTo("/bicicleta-personalizada");
                            break;
                        default:
                            navigationManager.NavigateTo("/");
                            break;
                    }
                }
                else
                {
                    EstaProcesando = false;
                    MostrarErroresAutenticacion = true;
                    Errores = result.ErrorMessage ?? "Usuario y/o contraseña son incorrectos";
                }
            }
            catch (Exception ex)
            {
                EstaProcesando = false;
                MostrarErroresAutenticacion = true;
                Errores = $"Error inesperado: {ex.Message}";
            }
        }
    }
}
