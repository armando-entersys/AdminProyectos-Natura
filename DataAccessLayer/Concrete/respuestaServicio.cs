using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    public class respuestaServicio
    {
        public bool Exito { get; set; } = false;
        public string Mensaje { get; set; }
        public dynamic Datos { get; set; }
    }
}
