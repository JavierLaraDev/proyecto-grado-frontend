using System.ComponentModel.DataAnnotations;

namespace PersonalizacionProyectoGradoWASM.Modelos
{
    public class PedidosItemsDto
    {
        public int Id { get; set; }
        public int Cantidad { get; set; }
        public Accesorio Accesorio { get; set; }
    }
}
