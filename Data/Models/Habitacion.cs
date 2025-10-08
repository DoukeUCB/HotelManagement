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

        // Relaciones
        [ForeignKey("Tipo_Habitacion_ID")]
        public virtual TipoHabitacion? TipoHabitacion { get; set; }

        public virtual ICollection<DetalleReserva>? DetalleReservas { get; set; }
    }
}
