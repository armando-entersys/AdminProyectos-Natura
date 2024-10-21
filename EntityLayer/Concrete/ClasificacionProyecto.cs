using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class ClasificacionProyecto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public bool Estatus { get; set; }

        // Navigation property for related Proyectos
        public ICollection<Proyecto> Proyectos { get; set; }
    }
}
