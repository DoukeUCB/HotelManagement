namespace HotelManagement.DTOs
{
    public class ReservaDTO
    {
        public string? ID { get; set; }
        public string Cliente_ID { get; set; } = string.Empty;
        public DateTime Fecha_Reserva { get; set; }
        public DateTime Fecha_Entrada { get; set; }
        public DateTime Fecha_Salida { get; set; }
        public string Estado_Reserva { get; set; } = string.Empty;
        public decimal Monto_Total { get; set; }
    }

    public class ReservaCreateDTO
    {
        public string Cliente_ID { get; set; } = string.Empty;
        public DateTime Fecha_Entrada { get; set; }
        public DateTime Fecha_Salida { get; set; }
        public string Estado_Reserva { get; set; } = "Pendiente";
        public decimal Monto_Total { get; set; }
    }

    public class ReservaUpdateDTO
    {
        public DateTime? Fecha_Entrada { get; set; }
        public DateTime? Fecha_Salida { get; set; }
        public string? Estado_Reserva { get; set; }
        public decimal? Monto_Total { get; set; }
    }
}
