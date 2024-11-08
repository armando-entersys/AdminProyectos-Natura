using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class RetrasoMaterial
    {
        public int Id { get; set; }
        public int MotivoId { get; set; }
        public string Comentario { get; set; }

        // Llave foránea para relacionarse con Material
        public int MaterialId { get; set; }
        public Material Material { get; set; }
    }
}
