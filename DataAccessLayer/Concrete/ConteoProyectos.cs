using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    public class ConteoProyectos
    {
        public int Hoy { get; set; }
        public int EstaSemana { get; set; }
        public int ProximaSemana { get; set; }
        public int TotalProyectos { get; set; }
    }
}
