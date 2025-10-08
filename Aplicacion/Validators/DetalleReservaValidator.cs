using HotelManagement.DTOs;
using HotelManagement.Aplicacion.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Aplicacion.Validators
{
    public interface IDetalleReservaValidator
    {
        Task ValidateCreateAsync(DetalleReservaCreateDTO dto);
        Task ValidateUpdateAsync(string id, DetalleReservaUpdateDTO dto);
        Task ValidateDeleteAsync(string id);
    }

    public class DetalleReservaValidator : IDetalleReservaValidator
    {
        private readonly Datos.Config.HotelDbContext _context;

        public DetalleReservaValidator(Datos.Config.HotelDbContext context)
        {
            _context = context;
        }

        public async Task ValidateCreateAsync(DetalleReservaCreateDTO dto)
        {
            var errors = new List<string>();

            // Validar UUIDs
            if (!IsValidUuid(dto.Reserva_ID))
                errors.Add("Reserva_ID debe ser un UUID válido");
            if (!IsValidUuid(dto.Habitacion_ID))
                errors.Add("Habitacion_ID debe ser un UUID válido");
            if (!IsValidUuid(dto.Huesped_ID))
                errors.Add("Huesped_ID debe ser un UUID válido");

            // Validar precio
            if (dto.Precio_Total <= 0)
                errors.Add("Precio_Total debe ser mayor a 0");

            // Validar cantidad de huéspedes
            if (dto.Cantidad_Huespedes <= 0)
                errors.Add("Cantidad_Huespedes debe ser mayor a 0");

            // Validar existencia de entidades relacionadas
            if (IsValidUuid(dto.Reserva_ID))
            {
                var reservaExists = await _context.Reservas
                    .AnyAsync(r => r.ID == ConvertToGuid(dto.Reserva_ID));
                if (!reservaExists)
                    errors.Add($"No existe una reserva con ID: {dto.Reserva_ID}");
            }

            if (IsValidUuid(dto.Habitacion_ID))
            {
                var habitacionExists = await _context.Habitaciones
                    .AnyAsync(h => h.ID == ConvertToGuid(dto.Habitacion_ID));
                if (!habitacionExists)
                    errors.Add($"No existe una habitación con ID: {dto.Habitacion_ID}");
            }

            if (IsValidUuid(dto.Huesped_ID))
            {
                var huespedExists = await _context.Huespedes
                    .AnyAsync(h => h.ID == ConvertToGuid(dto.Huesped_ID));
                if (!huespedExists)
                    errors.Add($"No existe un huésped con ID: {dto.Huesped_ID}");
            }

            if (errors.Any())
                throw new ValidationException(errors);
        }

        public async Task ValidateUpdateAsync(string id, DetalleReservaUpdateDTO dto)
        {
            var errors = new List<string>();

            if (!IsValidUuid(id))
                errors.Add("ID debe ser un UUID válido");

            if (dto.Precio_Total.HasValue && dto.Precio_Total.Value <= 0)
                errors.Add("Precio_Total debe ser mayor a 0");

            if (dto.Cantidad_Huespedes.HasValue && dto.Cantidad_Huespedes.Value <= 0)
                errors.Add("Cantidad_Huespedes debe ser mayor a 0");

            if (!string.IsNullOrEmpty(dto.Habitacion_ID))
            {
                if (!IsValidUuid(dto.Habitacion_ID))
                    errors.Add("Habitacion_ID debe ser un UUID válido");
                else
                {
                    var habitacionExists = await _context.Habitaciones
                        .AnyAsync(h => h.ID == ConvertToGuid(dto.Habitacion_ID));
                    if (!habitacionExists)
                        errors.Add($"No existe una habitación con ID: {dto.Habitacion_ID}");
                }
            }

            if (!string.IsNullOrEmpty(dto.Huesped_ID))
            {
                if (!IsValidUuid(dto.Huesped_ID))
                    errors.Add("Huesped_ID debe ser un UUID válido");
                else
                {
                    var huespedExists = await _context.Huespedes
                        .AnyAsync(h => h.ID == ConvertToGuid(dto.Huesped_ID));
                    if (!huespedExists)
                        errors.Add($"No existe un huésped con ID: {dto.Huesped_ID}");
                }
            }

            if (errors.Any())
                throw new ValidationException(errors);
        }

        public async Task ValidateDeleteAsync(string id)
        {
            if (!IsValidUuid(id))
                throw new ValidationException("ID debe ser un UUID válido");

            var exists = await _context.DetalleReservas
                .AnyAsync(d => d.ID == ConvertToGuid(id));
            
            if (!exists)
                throw new NotFoundException($"No se encontró el detalle de reserva con ID: {id}");
        }

        private bool IsValidUuid(string value)
        {
            return Guid.TryParse(value, out _);
        }

        private byte[] ConvertToGuid(string uuid)
        {
            return Guid.Parse(uuid).ToByteArray();
        }
    }
}
