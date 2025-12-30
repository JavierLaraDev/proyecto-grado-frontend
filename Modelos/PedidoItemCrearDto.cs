// Modelos/PedidoItemCrearDto.cs
using System.ComponentModel.DataAnnotations;

namespace PersonalizacionProyectoGradoWASM.Modelos
{
    public class PedidoItemCrearDto
    {
        [Required]
        public int AccesorioId { get; set; }

        [Required]
        public int Cantidad { get; set; }
    }
}