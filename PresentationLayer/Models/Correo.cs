namespace PresentationLayer.Models
{
    public class Correo
    {
        public int Id { get; set; }
        public string Destinatario { get; set; }
        public string Asunto { get; set; }
        public string Cuerpo { get; set; }
        public bool Enviado { get; set; } = false;

        // Relación con Categoría
        public int CategoriaCorreoId { get; set; }
        public CategoriaCorreo CategoriaCorreo { get; set; }

        public DateTime FechaEnvio { get; set; }
    }
}
