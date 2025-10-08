using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagement.Models
{
    public class Cliente
    {
        [Key]
        [Column(TypeName = "BINARY(16)")]
        public byte[] ID { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string Razon_Social { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string NIT { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        // Relaciones
        public virtual ICollection<Reserva>? Reservas { get; set; }
    }
}
