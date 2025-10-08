using HotelManagement.DTOs;
using HotelManagement.Models;
using HotelManagement.Repositories;
using HotelManagement.Aplicacion.Validators;
using HotelManagement.Aplicacion.Exceptions;

namespace HotelManagement.Services
{
    public class DetalleReservaService : IDetalleReservaService
    {
        private readonly IDetalleReservaRepository _repository;
        private readonly IDetalleReservaValidator _validator;

        public DetalleReservaService(
            IDetalleReservaRepository repository,
            IDetalleReservaValidator validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<List<DetalleReservaDTO>> GetAllAsync()
        {
            var detalles = await _repository.GetAllAsync();
            return detalles.Select(MapToDTO).ToList();
        }

        public async Task<DetalleReservaDTO> GetByIdAsync(string id)
        {
            var guidBytes = Guid.Parse(id).ToByteArray();
            var detalle = await _repository.GetByIdAsync(guidBytes);

            if (detalle == null)
                throw new NotFoundException($"No se encontró el detalle de reserva con ID: {id}");

            return MapToDTO(detalle);
        }

        public async Task<DetalleReservaDTO> CreateAsync(DetalleReservaCreateDTO dto)
        {
            await _validator.ValidateCreateAsync(dto);

            var detalle = new DetalleReserva
            {
                ID = Guid.NewGuid().ToByteArray(),
                Reserva_ID = Guid.Parse(dto.Reserva_ID).ToByteArray(),
                Habitacion_ID = Guid.Parse(dto.Habitacion_ID).ToByteArray(),
                Huesped_ID = Guid.Parse(dto.Huesped_ID).ToByteArray(),
                Precio_Total = dto.Precio_Total,
                Cantidad_Huespedes = dto.Cantidad_Huespedes
            };

            var created = await _repository.CreateAsync(detalle);
            return MapToDTO(created);
        }

        public async Task<DetalleReservaDTO> UpdateAsync(string id, DetalleReservaUpdateDTO dto)
        {
            await _validator.ValidateUpdateAsync(id, dto);

            var guidBytes = Guid.Parse(id).ToByteArray();
            var detalle = await _repository.GetByIdAsync(guidBytes);

            if (detalle == null)
                throw new NotFoundException($"No se encontró el detalle de reserva con ID: {id}");

            // Actualizar solo los campos proporcionados
            if (!string.IsNullOrEmpty(dto.Habitacion_ID))
                detalle.Habitacion_ID = Guid.Parse(dto.Habitacion_ID).ToByteArray();

            if (!string.IsNullOrEmpty(dto.Huesped_ID))
                detalle.Huesped_ID = Guid.Parse(dto.Huesped_ID).ToByteArray();

            if (dto.Precio_Total.HasValue)
                detalle.Precio_Total = dto.Precio_Total.Value;

            if (dto.Cantidad_Huespedes.HasValue)
                detalle.Cantidad_Huespedes = dto.Cantidad_Huespedes.Value;

            var updated = await _repository.UpdateAsync(detalle);
            return MapToDTO(updated);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            await _validator.ValidateDeleteAsync(id);
            var guidBytes = Guid.Parse(id).ToByteArray();
            return await _repository.DeleteAsync(guidBytes);
        }

        public async Task<List<DetalleReservaDTO>> GetByReservaIdAsync(string reservaId)
        {
            var guidBytes = Guid.Parse(reservaId).ToByteArray();
            var detalles = await _repository.GetByReservaIdAsync(guidBytes);
            return detalles.Select(MapToDTO).ToList();
        }

        private DetalleReservaDTO MapToDTO(DetalleReserva detalle)
        {
            return new DetalleReservaDTO
            {
                ID = ByteArrayToGuid(detalle.ID),
                Reserva_ID = ByteArrayToGuid(detalle.Reserva_ID),
                Habitacion_ID = ByteArrayToGuid(detalle.Habitacion_ID),
                Huesped_ID = ByteArrayToGuid(detalle.Huesped_ID),
                Precio_Total = detalle.Precio_Total,
                Cantidad_Huespedes = detalle.Cantidad_Huespedes,
                Numero_Habitacion = detalle.Habitacion?.Numero_Habitacion,
                Nombre_Huesped = detalle.Huesped?.Nombre_Completo,
                Fecha_Entrada = detalle.Reserva?.Fecha_Entrada,
                Fecha_Salida = detalle.Reserva?.Fecha_Salida
            };
        }

        private string ByteArrayToGuid(byte[] bytes)
        {
            return new Guid(bytes).ToString();
        }
    }
}
