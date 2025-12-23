using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PersonalizacionProyectoGradoWASM.Helpers;
using PersonalizacionProyectoGradoWASM.Modelos;
using PersonalizacionProyectoGradoWASM.Servicios.IServicios;

namespace PersonalizacionProyectoGradoWASM.Servicios
{
    public class UsuariosServicio : IUsuariosServicio
    {
        private readonly HttpClient _cliente;

        public UsuariosServicio(HttpClient cliente)
        {
            _cliente = cliente;
        }

        public async Task<UsuarioGestion> ActualizarUsuario(int usuarioId, UsuarioGestion usuario)
        {
            var content = JsonConvert.SerializeObject(usuario);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await _cliente.PutAsync($"{Inicializar.UrlBaseApi}api/usuarios/{usuarioId}", bodyContent);

            if (response.IsSuccessStatusCode)
            {
                // Si la actualización fue exitosa, obtén el usuario actualizado
                return await GetUsuario(usuarioId);
            }
            else
            {
                var contentTemp = await response.Content.ReadAsStringAsync();
                var errorModel = JsonConvert.DeserializeObject<ModeloError>(contentTemp);
                throw new Exception(errorModel.ErrorMessage);
            }
        }

        public async Task<bool> EliminarUsuario(int usuarioId)
        {
            var response = await _cliente.DeleteAsync($"{Inicializar.UrlBaseApi}api/usuarios/{usuarioId}");

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

        public async Task<UsuarioGestion> GetUsuario(int usuarioId)
        {
            var response = await _cliente.GetAsync($"{Inicializar.UrlBaseApi}api/usuarios/{usuarioId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var usuario = JsonConvert.DeserializeObject<UsuarioGestion>(content);
                return usuario;
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                var errorModel = JsonConvert.DeserializeObject<ModeloError>(content);
                throw new Exception(errorModel.ErrorMessage);
            }
        }

        public async Task<IEnumerable<UsuarioGestion>> GetUsuarios()
        {
            var response = await _cliente.GetAsync($"{Inicializar.UrlBaseApi}api/usuarios");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var usuarios = JsonConvert.DeserializeObject<IEnumerable<UsuarioGestion>>(content);
                return usuarios;
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                var errorModel = JsonConvert.DeserializeObject<ModeloError>(content);
                throw new Exception(errorModel.ErrorMessage);
            }
        }
    }
}