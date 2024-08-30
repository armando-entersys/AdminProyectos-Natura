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
        public int Descripcion { get; set; }

        // Colección de Briefs asociados con el TipoBrief
        public ICollection<Brief> Briefs { get; set; }
    }
}
