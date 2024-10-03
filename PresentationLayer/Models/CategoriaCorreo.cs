namespace PresentationLayer.Models
{
    public class CategoriaCorreo
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public ICollection<Correo> Correos { get; set; }
    }
}
