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
        [MaxLength(20)]
        public string Estado_Reserva { get; set; } = "Pendiente";

        [Required]
        [Column(TypeName = "DECIMAL(10,2)")]
        public decimal Monto_Total { get; set; } = 0.00m;

        // Campos de auditoría
        [Required]
        public DateTime Fecha_Creacion { get; set; } = DateTime.Now;

        [Column(TypeName = "BINARY(16)")]
        public byte[]? Usuario_Creacion_ID { get; set; }

        // Relaciones
        [ForeignKey("Cliente_ID")]
        public virtual Cliente? Cliente { get; set; }

        [ForeignKey("Usuario_Creacion_ID")]
        public virtual Usuario? UsuarioCreacion { get; set; }

        // Cambiado: colección NO anulable e inicializada para evitar advertencias de nulabilidad en Include/ThenInclude
        public virtual ICollection<DetalleReserva> DetalleReservas { get; set; } = new List<DetalleReserva>();
    }
}
