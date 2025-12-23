using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using PersonalizacionProyectoGradoWASM.Helpers;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace PersonalizacionProyectoGradoWASM.Servicios
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _cliente;
        private readonly ILocalStorageService _localStorageService;

        public AuthStateProvider(HttpClient cliente, ILocalStorageService localStorageService)
        {
            _cliente = cliente;
            _localStorageService = localStorageService;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorageService.GetItemAsync<string>(Inicializar.Token_Local);
            if (string.IsNullOrEmpty(token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var claims = JwtParser.ParseClaimsFromJwt(token).ToList();

            // Añade el rol a las claims si no está ya incluido
            var userRole = await _localStorageService.GetItemAsync<string>(Inicializar.Rol_Usuario_Local);
            if (!string.IsNullOrEmpty(userRole) && !claims.Any(c => c.Type == ClaimTypes.Role))
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            // Añade el nombre de usuario a las claims si no está ya incluido
            var nombreUsuario = await _localStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
            if (!string.IsNullOrEmpty(nombreUsuario) && !claims.Any(c => c.Type == ClaimTypes.Name))
            {
                claims.Add(new Claim(ClaimTypes.Name, nombreUsuario));
            }

            // Añade el ID del usuario a las claims si no está ya incluido
            var userId = await _localStorageService.GetItemAsync<string>(Inicializar.Id_Usuario_Local);
            if (!string.IsNullOrEmpty(userId) && !claims.Any(c => c.Type == ClaimTypes.NameIdentifier))
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
            }

            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);
            var state = new AuthenticationState(user);

            return state;
        }
        public async Task NotificarUsuarioLogueado(string token)
        {
            var claims = JwtParser.ParseClaimsFromJwt(token).ToList();

            var userRole = await _localStorageService.GetItemAsync<string>(Inicializar.Rol_Usuario_Local);
            var nombreUsuario = await _localStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
            var userId = await _localStorageService.GetItemAsync<string>(Inicializar.Id_Usuario_Local);

            if (!string.IsNullOrEmpty(userRole) && !claims.Any(c => c.Type == ClaimTypes.Role))
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            if (!string.IsNullOrEmpty(nombreUsuario) && !claims.Any(c => c.Type == ClaimTypes.Name))
            {
                claims.Add(new Claim(ClaimTypes.Name, nombreUsuario));
            }
            if (!string.IsNullOrEmpty(userId) && !claims.Any(c => c.Type == ClaimTypes.NameIdentifier))
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
            }

            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);
            var authState = Task.FromResult(new AuthenticationState(user));
            NotifyAuthenticationStateChanged(authState);

            await _localStorageService.SetItemAsync(Inicializar.Token_Local, token);
        }
        public void NotificarUsuarioSalir()
        {
            var authState = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
            NotifyAuthenticationStateChanged(authState);
        }
    }
}
