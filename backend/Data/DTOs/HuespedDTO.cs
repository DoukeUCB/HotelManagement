using System.ComponentModel.DataAnnotations;

namespace HotelManagement.DTOs
{
    public class HuespedDTO
    {
        public string ID { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string? Segundo_Apellido { get; set; }
        public string Documento_Identidad { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public DateTime? Fecha_Nacimiento { get; set; }
        public bool Activo { get; set; }
    }

    public class HuespedCreateDTO
    {
        [Required]
        [StringLength(30)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string Apellido { get; set; } = string.Empty;

        [StringLength(30)]
        public string? Segundo_Apellido { get; set; }

        [Required]
        [StringLength(20)]
        public string Documento_Identidad { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        public string? Telefono { get; set; }

        public string? Fecha_Nacimiento { get; set; }
    }

    public class HuespedUpdateDTO
    {
        [StringLength(30)]
        public string? Nombre { get; set; }

        [StringLength(30)]
        public string? Apellido { get; set; }

        [StringLength(30)]
        public string? Segundo_Apellido { get; set; }

        [StringLength(20)]
        public string? Documento_Identidad { get; set; }

        [Phone]
        [StringLength(20)]
        public string? Telefono { get; set; }

        public string? Fecha_Nacimiento { get; set; }

        public bool? Activo { get; set; }
    }
}
