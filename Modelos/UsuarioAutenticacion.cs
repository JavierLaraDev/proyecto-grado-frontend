using System.ComponentModel.DataAnnotations;

namespace PersonalizacionProyectoGradoWASM.Modelos
{
    public class UsuarioAutenticacion
    {
        [Required(ErrorMessage = "El Usuario es obligatorio")]
        public string NombreUsuario { get; set; }
        [Required(ErrorMessage = "La Contraseña es obligatoria")]
        public string Password { get; set; }
    }
}
