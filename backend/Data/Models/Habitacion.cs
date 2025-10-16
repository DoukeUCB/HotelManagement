using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagement.Models
{
    public class Habitacion
    {
        [Key]
        [Column(TypeName = "BINARY(16)")]
        public byte[] ID { get; set; } = null!;

        [Required]
        [Column(TypeName = "BINARY(16)")]
        public byte[] Tipo_Habitacion_ID { get; set; } = null!;

        [Required]
        [MaxLength(10)]
        public string Numero_Habitacion { get; set; } = string.Empty;

        [Required]
        public short Piso { get; set; }

        [Required]
        [MaxLength(20)]
        public string Estado_Habitacion { get; set; } = "Libre";

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
        [ForeignKey("Tipo_Habitacion_ID")]
        public virtual TipoHabitacion? TipoHabitacion { get; set; }

        [ForeignKey("Usuario_Creacion_ID")]
        public virtual Usuario? UsuarioCreacion { get; set; }

        [ForeignKey("Usuario_Actualizacion_ID")]
        public virtual Usuario? UsuarioActualizacion { get; set; }

        public virtual ICollection<DetalleReserva>? DetalleReservas { get; set; }
    }
}
