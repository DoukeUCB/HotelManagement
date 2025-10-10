using System.ComponentModel.DataAnnotations;

namespace HotelManagement.DTOs
{
    public class HuespedDTO
    {
        public string ID { get; set; } = string.Empty;
        public string Nombre_Completo { get; set; } = string.Empty;
        public string Documento_Identidad { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public DateTime? Fecha_Nacimiento { get; set; }
    }

    public class HuespedCreateDTO
    {
        [Required]
        public string Nombre_Completo { get; set; } = string.Empty;

        [Required]
        public string Documento_Identidad { get; set; } = string.Empty;

        [Phone]
        public string? Telefono { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? Fecha_Nacimiento { get; set; }
    }
}
