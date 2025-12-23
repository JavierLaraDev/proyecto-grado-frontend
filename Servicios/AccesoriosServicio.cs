using Newtonsoft.Json;
using PersonalizacionProyectoGradoWASM.Helpers;
using PersonalizacionProyectoGradoWASM.Modelos;
using PersonalizacionProyectoGradoWASM.Servicios.IServicios;
using System.Text;

namespace PersonalizacionProyectoGradoWASM.Servicios
{
    public class AccesoriosServicio : IAccesoriosServicio
    {
        private readonly HttpClient _cliente;

        public AccesoriosServicio(HttpClient cliente)
        {
            _cliente = cliente;
        }

        public async Task<Accesorio> ActualizarAccesorio(int accesorioId, Accesorio accesorio)
        {
            var content = JsonConvert.SerializeObject(accesorio);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await _cliente.PatchAsync($"{Inicializar.UrlBaseApi}api/accesorios/{accesorioId}", bodyContent);

            if (response.IsSuccessStatusCode)
            {
                var contentTemp = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Accesorio>(contentTemp);
                return result;
            }
            else
            {
                var contentTemp = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error response: {contentTemp}");
                var errorModel = JsonConvert.DeserializeObject<ModeloError>(contentTemp);
                throw new Exception(errorModel.ErrorMessage);
            }
        }

        public async Task<Accesorio> CrearAccesorio(Accesorio accesorio)
        {
            var content = JsonConvert.SerializeObject(accesorio);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await _cliente.PostAsync($"{Inicializar.UrlBaseApi}api/accesorios", bodyContent);

            if (response.IsSuccessStatusCode)
            {
                var contentTemp = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Accesorio>(contentTemp);
                return result;
            }
            else
            {
                var contentTemp = await response.Content.ReadAsStringAsync();
                // 🔥 BUG CORREGIDO: Cambiar 'content' por 'contentTemp'
                var errorModel = JsonConvert.DeserializeObject<ModeloError>(contentTemp);
                throw new Exception(errorModel.ErrorMessage);
            }
        }

        public async Task<bool> EliminarAccesorio(int accesorioId)
        {
            var response = await _cliente.DeleteAsync($"{Inicializar.UrlBaseApi}api/accesorios/{accesorioId}");
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

        public async Task<Accesorio> GetAccesorio(int accesorioId)
        {
            var response = await _cliente.GetAsync($"{Inicializar.UrlBaseApi}api/accesorios/{accesorioId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var accesorio = JsonConvert.DeserializeObject<Accesorio>(content);
                return accesorio;
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                var errorModel = JsonConvert.DeserializeObject<ModeloError>(content);
                throw new Exception(errorModel.ErrorMessage);
            }
        }

        public async Task<IEnumerable<Accesorio>> GetAccesorios()
        {
            var response = await _cliente.GetAsync($"{Inicializar.UrlBaseApi}api/accesorios");
            var content = await response.Content.ReadAsStringAsync();
            var accesorios = JsonConvert.DeserializeObject<IEnumerable<Accesorio>>(content);
            return accesorios;
        }

        public async Task<string> ObtenerRutaBicicleta()
        {
            var accesorios = await GetAccesorios();
            var bici = accesorios.FirstOrDefault(a =>
                a.Nombre == "Bicicleta" && a.Descripcion == "Retro");

            return bici?.RutaImagen ?? "";
        }

        public async Task<string> SubidaImagen(MultipartFormDataContent content)
        {
            var response = await _cliente.PostAsync($"{Inicializar.UrlBaseApi}api/upload", content);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"❌ Error al subir imagen: {json}");
                throw new ApplicationException($"Error al subir: {json}");
            }

            Console.WriteLine($"✅ Respuesta del servidor: {json}");

            var resultado = JsonConvert.DeserializeObject<BackblazeUploadResponse>(json);

            if (string.IsNullOrEmpty(resultado?.url))
            {
                Console.WriteLine("⚠️ URL vacía en la respuesta");
                throw new ApplicationException("No se recibió URL del archivo");
            }

            Console.WriteLine($"🔗 URL obtenida: {resultado.url}");
            return resultado.url;
        }
    }
}