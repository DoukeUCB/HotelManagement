using System.ComponentModel.DataAnnotations;

namespace HotelManagement.DTOs
{
    public class TipoHabitacionDTO
    {
        public string ID { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public byte Capacidad_Maxima { get; set; }
        public decimal Tarifa_Base { get; set; }
        public bool Activo { get; set; }
    }

    public class TipoHabitacionCreateDTO
    {
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [Range(1, 255)]
        public byte Capacidad_Maxima { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Tarifa_Base { get; set; }
    }

    public class TipoHabitacionUpdateDTO
    {
        [StringLength(50)]
        public string? Nombre { get; set; }

        [Range(1, 255)]
        public byte? Capacidad_Maxima { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Tarifa_Base { get; set; }

        public bool? Activo { get; set; }
    }
}
