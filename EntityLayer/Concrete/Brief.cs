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
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Objetivo { get; set; }
        public string DirigidoA { get; set; }
        public string Comentario { get; set; }
        public string RutaArchivo { get; set; }
        public int IdTipoBrief { get; set; }
        public int IdEstatusBrief { get; set; }
        public DateTime FechaEntrega { get; set; }

        public DateTime FechaRegistro { get; set; }
        public DateTime FechaModificacion { get; set; }
    }
}
