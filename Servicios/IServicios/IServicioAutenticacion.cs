using PersonalizacionProyectoGradoWASM.Modelos;

namespace PersonalizacionProyectoGradoWASM.Servicios.IServicios
{
    public interface IServicioAutenticacion
    {
        Task<RespuestaRegistro> RegistrarUsuario(UsuarioRegistro usuarioParaRegistro);
        Task<RespuestaAutenticacion> Acceder(UsuarioAutenticacion usuarioDesdeAutenticacion);
        Task Salir();
    }
}
