using System.ComponentModel.DataAnnotations;

namespace PersonalizacionProyectoGradoWASM.Modelos
{
    public class UsuarioGestion
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        [Required(ErrorMessage = "El Nombre es obligatorio")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "Los Apellidos son obligatorios")]
        public string Apellido { get; set; }
        [Required(ErrorMessage = "El Numero de celular es obligatorio")]
        public int NumeroCelular { get; set; }
        [Required(ErrorMessage = "La Dirección es obligatoria")]
        public string Direccion { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public RolEnum Rol { get; set; }

    }
}
