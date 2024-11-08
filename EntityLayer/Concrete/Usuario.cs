using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

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
        [StringLength(100, ErrorMessage = "La contraseña debe tener al menos {2} y un máximo de {1} caracteres.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "La contraseña debe tener al menos una letra mayúscula, una minúscula, un número y un carácter especial.")]
        public string? Contrasena { get; set; }
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
