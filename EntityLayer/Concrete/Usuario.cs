using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Correo { get; set; }
        public string? Contrasena { get; set; }
        public int RolId { get; set; }
        public Rol UserRol { get; set; } = new Rol();
        public int TipoUsuarioId { get; set; }
        public TipoUsuario TipoUsuario { get; set; } = new TipoUsuario();
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaModificacion { get; set; }
        public bool CambioContrasena { get; set; }
        public bool SolicitudRegistro { get; set; }
        // Colección de Briefs asociados con el Usuario
        public ICollection<Brief> Briefs { get; set; }

    }
}
