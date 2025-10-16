using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagement.Models
{
    public class Huesped
    {
        [Key]
        [Column(TypeName = "BINARY(16)")]
        public byte[] ID { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string Apellido { get; set; } = string.Empty;

        [MaxLength(30)]
        public string? Segundo_Apellido { get; set; }

        [Required]
        [MaxLength(20)]
        public string Documento_Identidad { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Telefono { get; set; }

        public DateTime? Fecha_Nacimiento { get; set; }

        public bool Activo { get; set; } = true;

        // Campos de auditoría
        [Required]
        public DateTime Fecha_Creacion { get; set; } = DateTime.Now;

        [Required]
        public DateTime Fecha_Actualizacion { get; set; } = DateTime.Now;

        [Column(TypeName = "BINARY(16)")]
        public byte[]? Usuario_Creacion_ID { get; set; }

        [Column(TypeName = "BINARY(16)")]
        public byte[]? Usuario_Actualizacion_ID { get; set; }

        // Relaciones
        [ForeignKey("Usuario_Creacion_ID")]
        public virtual Usuario? UsuarioCreacion { get; set; }

        [ForeignKey("Usuario_Actualizacion_ID")]
        public virtual Usuario? UsuarioActualizacion { get; set; }

        public virtual ICollection<DetalleReserva>? DetalleReservas { get; set; }
    }
}
