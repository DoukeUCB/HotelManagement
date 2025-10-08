using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagement.Models
{
    public class DetalleReserva
    {
        [Key]
        [Column(TypeName = "BINARY(16)")]
        public byte[] ID { get; set; } = null!;

        [Required]
        [Column(TypeName = "BINARY(16)")]
        public byte[] Reserva_ID { get; set; } = null!;

        [Required]
        [Column(TypeName = "BINARY(16)")]
        public byte[] Habitacion_ID { get; set; } = null!;

        [Required]
        [Column(TypeName = "BINARY(16)")]
        public byte[] Huesped_ID { get; set; } = null!;

        [Required]
        [Column(TypeName = "DECIMAL(10,2)")]
        public decimal Precio_Total { get; set; }

        [Required]
        public byte Cantidad_Huespedes { get; set; }

        // Relaciones
        [ForeignKey("Reserva_ID")]
        public virtual Reserva? Reserva { get; set; }

        [ForeignKey("Habitacion_ID")]
        public virtual Habitacion? Habitacion { get; set; }

        [ForeignKey("Huesped_ID")]
        public virtual Huesped? Huesped { get; set; }
    }
}
