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

        // Colección de Usuarios asociados con este Rol
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

        // Colección de Menus
        public ICollection<Menu> Menus { get; set; } = new List<Menu>();
    }
}
