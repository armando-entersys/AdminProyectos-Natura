using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Menu
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Ruta { get; set; }
        public int Orden { get; set; }
        public string Icono { get; set; }
        public int RolId { get; set; }
        public Rol Rol { get; set; }
    }
}
