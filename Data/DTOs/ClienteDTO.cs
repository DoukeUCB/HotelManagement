namespace HotelManagement.DTOs
{
    // DTO para la respuesta de la API (incluye el ID)
    public class ClienteDTO
    {
        public string? ID { get; set; }
        public string Razon_Social { get; set; } = string.Empty;
        public string NIT { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    // DTO para crear un nuevo cliente
    public class ClienteCreateDTO
    {
        public string Razon_Social { get; set; } = string.Empty;
        public string NIT { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    // DTO para actualizar (los campos son opcionales)
    public class ClienteUpdateDTO
    {
        public string? Razon_Social { get; set; }
        public string? NIT { get; set; }
        public string? Email { get; set; }
    }
}