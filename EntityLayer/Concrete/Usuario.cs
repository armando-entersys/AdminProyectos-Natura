using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityLayer.Concrete
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Correo { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$",
         ErrorMessage = "La contraseña debe tener al menos 8 caracteres, una letra mayúscula, un número y un carácter especial.")]
        public string Contrasena { get; set; }

        [NotMapped]
        [Compare("Contrasena", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarContrasena { get; set; }
        public int RolId { get; set; }
        public Rol UserRol { get; set; }
        public bool Estatus { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaModificacion { get; set; }
        public bool CambioContrasena { get; set; }
        public bool SolicitudRegistro { get; set; }
        // Colección de Briefs asociados con el Usuario
        public ICollection<Brief> Briefs { get; set; }
        // Nueva colección para Participantes
        public ICollection<Participante> Participantes { get; set; } // Nueva colección

    }
}
