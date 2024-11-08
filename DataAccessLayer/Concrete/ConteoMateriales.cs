using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    public class ConteoMateriales
    {
        public int Registros { get; set; }
        public int Revision { get; set; }
        public int Produccion { get; set; }
        public int FaltaInfo { get; set; }
        public int Programado { get; set; }
        public int Entregado { get; set; }
        public int InicioCiclo { get; set; }
        public int NoCompartio { get; set; }
    }
}
