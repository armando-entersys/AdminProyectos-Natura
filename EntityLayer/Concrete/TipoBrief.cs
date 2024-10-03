using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class TipoBrief
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }

        // Navegación inversa
        public ICollection<Brief> Briefs { get; set; }
    }
}
