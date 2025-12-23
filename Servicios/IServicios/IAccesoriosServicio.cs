using PersonalizacionProyectoGradoWASM.Modelos;

namespace PersonalizacionProyectoGradoWASM.Servicios.IServicios
{
    public interface IAccesoriosServicio
    {
        public Task<IEnumerable<Accesorio>> GetAccesorios();
        public Task<Accesorio> GetAccesorio(int accesorioId);
        public Task<Accesorio> CrearAccesorio(Accesorio accesorio);
        public Task<Accesorio> ActualizarAccesorio(int accesorioId, Accesorio accesorio);
        public Task<bool> EliminarAccesorio(int accesorioId);
        public Task<string> SubidaImagen(MultipartFormDataContent content);
        public Task<String> ObtenerRutaBicicleta();
    }
}
