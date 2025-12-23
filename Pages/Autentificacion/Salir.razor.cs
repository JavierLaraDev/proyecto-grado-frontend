using Microsoft.AspNetCore.Components;
using PersonalizacionProyectoGradoWASM.Servicios.IServicios;

namespace PersonalizacionProyectoGradoWASM.Pages.Autentificacion
{
    public partial class Salir
    {
        [Inject]
        public IServicioAutenticacion servicioAutenticacion { get; set; }
        [Inject]
        public NavigationManager navigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await servicioAutenticacion.Salir();
            navigationManager.NavigateTo("/");
        }
    }
}
