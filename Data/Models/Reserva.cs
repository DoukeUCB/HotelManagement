using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagement.Models
{
    public class Reserva
    {
        [Key]
        [Column(TypeName = "BINARY(16)")]
        public byte[] ID { get; set; } = null!;

        [Required]
        [Column(TypeName = "BINARY(16)")]
        public byte[] Cliente_ID { get; set; } = null!;

        [Required]
        public DateTime Fecha_Reserva { get; set; } = DateTime.Now;

        [Required]
        public DateTime Fecha_Entrada { get; set; }

        [Required]
        public DateTime Fecha_Salida { get; set; }

        [Required]
        [MaxLength(20)]
        public string Estado_Reserva { get; set; } = "Pendiente";

        [Required]
        [Column(TypeName = "DECIMAL(10,2)")]
        public decimal Monto_Total { get; set; } = 0.00m;

        // Relaciones
        [ForeignKey("Cliente_ID")]
        public virtual Cliente? Cliente { get; set; }

        public virtual ICollection<DetalleReserva>? DetalleReservas { get; set; }
    }
}
