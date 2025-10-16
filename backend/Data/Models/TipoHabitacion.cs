using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagement.Models
{
    public class TipoHabitacion
    {
        [Key]
        [Column(TypeName = "BINARY(16)")]
        public byte[] ID { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        [Required]
        public byte Capacidad_Maxima { get; set; }

        [Required]
        [Column(TypeName = "DECIMAL(10,2)")]
        public decimal Precio_Base { get; set; }

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

        public virtual ICollection<Habitacion>? Habitaciones { get; set; }
    }
}
