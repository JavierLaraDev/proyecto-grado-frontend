using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PersonalizacionProyectoGradoWASM.Helpers;
using PersonalizacionProyectoGradoWASM.Modelos;
using PersonalizacionProyectoGradoWASM.Servicios.IServicios;

namespace PersonalizacionProyectoGradoWASM.Pages.Autentificacion
{
    public partial class RegistroAdmin
    {
        private UsuarioRegistro UsuarioParaRegistro = new UsuarioRegistro { Rol = RolEnum.Administrador };
        public bool EstaProcesando { get; set; } = false;
        public bool MostrarErroresRegistro { get; set; }
        public IEnumerable<string> Errores { get; set; }

        [Inject]
        public IServicioAutenticacion servicioAutenticacion { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        private async Task RegistrarUsuario()
        {
            MostrarErroresRegistro = false;
            EstaProcesando = true;
            var result = await servicioAutenticacion.RegistrarUsuario(UsuarioParaRegistro);
            if (result.registroCorrecto)
            {
                EstaProcesando = false;
                await JSRuntime.ToastrSuccess("Registro completado con éxito");
                // Aquí puedes resetear el formulario si lo deseas
                UsuarioParaRegistro = new UsuarioRegistro { Rol = RolEnum.cliente };
            }
            else
            {
                EstaProcesando = false;
                Errores = result.Errores;
                MostrarErroresRegistro = true;
                await JSRuntime.ToastrError("Error en el registro");
            }
        }
    }
}
