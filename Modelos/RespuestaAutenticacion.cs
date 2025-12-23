namespace PersonalizacionProyectoGradoWASM.Modelos
{
    public class RespuestaAutenticacion
    {
        public bool IsSuccess { get; set; }
        public string Token { get; set; }
        public Usuario Usuario { get; set; }
        public string ErrorMessage { get; set; }
    }
}
