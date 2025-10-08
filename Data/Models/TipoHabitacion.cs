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
        public string Nombre_Tipo { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        [Required]
        public byte Capacidad_Maxima { get; set; }

        [Required]
        [Column(TypeName = "DECIMAL(10,2)")]
        public decimal Precio_Base { get; set; }

        // Relaciones
        public virtual ICollection<Habitacion>? Habitaciones { get; set; }
    }
}
