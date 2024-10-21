using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Proyecto
    {
        public int Id { get; set; }
        public int BriefId { get; set; }
        public string Comentario { get; set; }
        public bool RequierePlan { get; set; }
        public  string  Estado { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; }

        // Propiedad de navegación a Brief (relación uno a uno)
        public Brief Brief { get; set; }

        // Foreign key for ClasificacionProyecto
        public int ClasificacionProyectoId { get; set; }

        // Navigation property
        public ClasificacionProyecto ClasificacionProyecto { get; set; }
    }
}
