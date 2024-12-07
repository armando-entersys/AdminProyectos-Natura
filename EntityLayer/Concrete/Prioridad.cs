using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Prioridad
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; } = false;

        // Relación de uno a muchos con Material
        public ICollection<Material> Materiales { get; set; } = new List<Material>();
    }
}
