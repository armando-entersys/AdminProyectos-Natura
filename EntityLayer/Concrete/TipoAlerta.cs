using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class TipoAlerta
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; } = false;
        // Relación inversa
        public virtual ICollection<Alerta> Alertas { get; set; }
    }
}
