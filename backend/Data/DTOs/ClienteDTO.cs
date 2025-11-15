using System.Text.Json.Serialization;
namespace HotelManagement.DTOs
{
    public class ClienteDTO
    {
        public string? ID { get; set; }
        public string Razon_Social { get; set; } = string.Empty;
        public string NIT { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool Activo { get; set; }
    }

    public class ClienteCreateDTO
    {
        public string Razon_Social { get; set; } = string.Empty;
        public string NIT { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class ClienteUpdateDTO
    {
        public string? Razon_Social { get; set; }
        public string? NIT { get; set; }
        public string? Email { get; set; }
        public bool? Activo { get; set; }
    }
}