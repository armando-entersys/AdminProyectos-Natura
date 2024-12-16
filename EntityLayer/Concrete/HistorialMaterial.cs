using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class HistorialMaterial
    {
        public int Id { get; set; }
        public string Comentarios { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public DateTime? FechaEntrega { get; set; }

        public int EstatusMaterialId { get; set; }

        // Llave foránea para Usuario
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        // Llave foránea para relacionarse con Material
        public int MaterialId { get; set; }
        public Material Material { get; set; }
    }
}
