namespace PresentationLayer.Models
{
    public class respuestaServicio
    {
        public bool Exito { get; set; } = false;
        public string Mensaje { get; set; }
        public dynamic Datos { get; set; }
       
    }
}
