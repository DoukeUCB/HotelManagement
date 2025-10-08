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
        [MaxLength(100)]
        public string Nombre_Completo { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Documento_Identidad { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Telefono { get; set; }

        [MaxLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        public DateTime? Fecha_Nacimiento { get; set; }

        // Relaciones
        public virtual ICollection<DetalleReserva>? DetalleReservas { get; set; }
    }
}
