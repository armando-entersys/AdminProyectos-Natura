using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Tasks<T>
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; }
        public string FechaEntrega { get; set; }
        

    }
}
