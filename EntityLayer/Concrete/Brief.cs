using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Brief
    {
        public int Id { get; set; }
        // Llave foránea para Usuario
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = new Usuario();
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Objetivo { get; set; }
        public string DirigidoA { get; set; }
        public string Comentario { get; set; }
        public string RutaArchivo { get; set; }
        // Llave foránea para TipoBrief
        public int TipoBriefId { get; set; }
        public TipoBrief TipoBrief { get; set; } = new TipoBrief();  // Navegación al TipoBrief

        public int IdEstatusBrief { get; set; }
        public DateTime FechaEntrega { get; set; }

        public DateTime FechaRegistro { get; set; }
        public DateTime FechaModificacion { get; set; }
    }
}
