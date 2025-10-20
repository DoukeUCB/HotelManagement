using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagement.Models
{
    public class Usuario
    {
        [Key]
        [Column(TypeName = "BINARY(16)")]
        public byte[] ID { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Usuario1 { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Contrasenia { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Apellido { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Segundo_Apellido { get; set; }

        [Required]
        [MaxLength(20)]
        public string Documento_Identidad { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Rol { get; set; } = "Recepcionista";

        public bool Activo { get; set; } = true;

        public virtual ICollection<Cliente>? ClientesCreados { get; set; }
        public virtual ICollection<Cliente>? ClientesActualizados { get; set; }
    }
}
