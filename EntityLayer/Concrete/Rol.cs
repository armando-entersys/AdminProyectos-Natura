using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Rol
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public Usuario Usuario { get; set; }
        // Colección de Menus
        public ICollection<Menu> Menus { get; set; }
    }
}
