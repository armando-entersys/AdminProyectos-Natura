using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Material
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Mensaje { get; set; }
        public int PrioridadId { get; set; }
        public string Ciclo { get; set; }
        public int PCNId { get; set; }
        public int AudienciaId { get; set; }
        public DateTime FechaEntrega { get; set; }
        public string Proceso { get; set; }
        public string Produccion { get; set; }
        public string Responsable { get; set; }

        // Nueva llave foránea para relacionarse con Brief
        public int BriefId { get; set; }
        public Brief Brief { get; set; }
    }
}
