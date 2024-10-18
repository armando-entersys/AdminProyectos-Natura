using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        public Usuario Usuario { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Objetivo { get; set; }
        public string DirigidoA { get; set; }
        public string Comentario { get; set; }
        public string RutaArchivo { get; set; }
        public bool PlanComunicacion { get; set; }
        public int DeterminarEstadoEstadoId { get; set; }
        public DateTime FechaPublicacion { get; set; }

        // Relación con EstatusBrief
        public int EstatusBriefId { get; set; } // Llave foránea para EstatusBrief
        public EstatusBrief EstatusBrief { get; set; }  // Navegación al EstatusBrief

        // Relación con TipoBrief
        public int TipoBriefId { get; set; }  // Llave foránea para TipoBrief
        public TipoBrief TipoBrief { get; set; }  // Navegación a TipoBrief

        public DateTime FechaEntrega { get; set; }
        // Automatización de FechaRegistro en la base de datos
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; }

        // Nueva relación de uno a muchos
        public ICollection<Material> Materiales { get; set; } = new List<Material>();

        // Relación uno a uno con Proyecto
        public Proyecto Proyecto { get; set; }
    }
}
