using System.ComponentModel.DataAnnotations;

namespace HotelManagement.DTOs
{
    public class ReservaDTO
    {
        public string ID { get; set; } = string.Empty;
        public string Cliente_ID { get; set; } = string.Empty;
        public string Estado_Reserva { get; set; } = string.Empty;
        public decimal Monto_Total { get; set; }
        public DateTime Fecha_Creacion { get; set; }
        public string? Cliente_Nombre { get; set; }
        
        // Campos calculados desde DetalleReserva (no están en Reserva)
        public DateTime? Fecha_Entrada { get; set; }
        public DateTime? Fecha_Salida { get; set; }
    }

    public class ReservaCreateDTO
    {
        [Required]
        public string Cliente_ID { get; set; } = string.Empty;

        [Required]
        public string Estado_Reserva { get; set; } = "Pendiente";

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Monto_Total { get; set; }
    }

    public class ReservaUpdateDTO
    {
        public string? Cliente_ID { get; set; }
        public string? Estado_Reserva { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal? Monto_Total { get; set; }
    }
}