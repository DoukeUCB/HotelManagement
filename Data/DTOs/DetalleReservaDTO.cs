namespace HotelManagement.DTOs
{
    public class DetalleReservaDTO
    {
        public string? ID { get; set; }
        public string Reserva_ID { get; set; } = string.Empty;
        public string Habitacion_ID { get; set; } = string.Empty;
        public string Huesped_ID { get; set; } = string.Empty;
        public decimal Precio_Total { get; set; }
        public byte Cantidad_Huespedes { get; set; }

        // Información adicional para respuestas
        public string? Numero_Habitacion { get; set; }
        public string? Nombre_Huesped { get; set; }
        public DateTime? Fecha_Entrada { get; set; }
        public DateTime? Fecha_Salida { get; set; }
    }

    public class DetalleReservaCreateDTO
    {
        public string Reserva_ID { get; set; } = string.Empty;
        public string Habitacion_ID { get; set; } = string.Empty;
        public string Huesped_ID { get; set; } = string.Empty;
        public decimal Precio_Total { get; set; }
        public byte Cantidad_Huespedes { get; set; }
    }

    public class DetalleReservaUpdateDTO
    {
        public string? Habitacion_ID { get; set; }
        public string? Huesped_ID { get; set; }
        public decimal? Precio_Total { get; set; }
        public byte? Cantidad_Huespedes { get; set; }
    }
}
