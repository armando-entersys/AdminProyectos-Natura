using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class EstatusBrief
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; } = false;
        // Colección de Briefs asociados con el TipoBrief
        public ICollection<Brief> Briefs { get; set; }
    }
}
