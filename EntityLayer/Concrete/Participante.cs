using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Participante
    {
        public int Id { get; set; }
        public int BriefId { get; set; }
        public int UsuarioId { get; set; }
        // Propiedades de navegación
        public Brief Brief { get; set; }
        public Usuario Usuario { get; set; }
    }
}
