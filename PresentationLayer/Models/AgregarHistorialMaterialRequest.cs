using EntityLayer.Concrete;

namespace PresentationLayer.Models
{
    public class AgregarHistorialMaterialRequest
    {
        public HistorialMaterial HistorialMaterial { get; set; }
        public bool EnvioCorreo { get; set; }
        public List<Usuario> Usuarios { get; set; }
    }
}
