using Microsoft.AspNetCore.Components;
using PersonalizacionProyectoGradoWASM.Modelos;
using PersonalizacionProyectoGradoWASM.Servicios.IServicios;

namespace PersonalizacionProyectoGradoWASM.Pages.Autentificacion
{
    public partial class Registro
    {
        private UsuarioRegistro UsuarioParaRegistro = new UsuarioRegistro { Rol = RolEnum.cliente };
        public bool EstaProcesando { get; set; } = false;
        public bool MostrarErroresRegistro { get; set; }

        public IEnumerable<string> Errores { get; set; }

        [Inject]
        public IServicioAutenticacion servicioAutenticacion { get; set; }

        [Inject]
        public NavigationManager navigationManager { get; set; }

        private async Task RegistrarUsuario()
        {
            MostrarErroresRegistro = false;
            EstaProcesando = true;
            var result = await servicioAutenticacion.RegistrarUsuario(UsuarioParaRegistro);
            if (result.registroCorrecto)
            {
                EstaProcesando = false;
                navigationManager.NavigateTo("/acceder");
            }
            else
            {
                EstaProcesando = false;
                Errores = result.Errores;
                MostrarErroresRegistro = true;
            }
        }
    }
}
