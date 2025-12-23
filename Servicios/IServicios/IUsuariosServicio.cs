using PersonalizacionProyectoGradoWASM.Modelos;

namespace PersonalizacionProyectoGradoWASM.Servicios.IServicios
{
    public interface IUsuariosServicio
    {
        public Task<IEnumerable<UsuarioGestion>> GetUsuarios();
        public Task<UsuarioGestion> GetUsuario(int usuarioId);
        public Task<UsuarioGestion> ActualizarUsuario(int usuarioId, UsuarioGestion usuario);
        public Task<bool> EliminarUsuario(int usuarioId);

    }
}
