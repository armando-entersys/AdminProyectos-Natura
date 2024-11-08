using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Alerta
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool lectura { get; set; } = false;
        public string Accion { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Propiedad de navegación
        public int IdTipoAlerta { get; set; }
        public virtual TipoAlerta TipoAlerta { get; set; }
    }
}
