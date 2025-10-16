using HotelManagement.DTOs;
using HotelManagement.Models;
using HotelManagement.Datos.Repositories;
using HotelManagement.Repositories;
using HotelManagement.Aplicacion.Exceptions;

namespace HotelManagement.Application.Services
{
    public class ReservaService : IReservaService
    {
        private readonly IReservaRepository _repository;
        private readonly IClienteRepository _clienteRepository;

        public ReservaService(IReservaRepository repository, IClienteRepository clienteRepository)
        {
            _repository = repository;
            _clienteRepository = clienteRepository;
        }

        public async Task<IEnumerable<ReservaDTO>> GetAllAsync()
        {
            var reservas = await _repository.GetAllAsync();

            return reservas.Select(r => new ReservaDTO
            {
                ID = new Guid(r.ID).ToString(),
                Cliente_ID = new Guid(r.Cliente_ID).ToString(),
                Cliente_Nombre = r.Cliente?.Razon_Social ?? "Cliente no disponible",
                Fecha_Creacion = r.Fecha_Creacion,
                Estado_Reserva = r.Estado_Reserva,
                Monto_Total = r.Monto_Total
            });
        }

        public async Task<ReservaDTO?> GetByIdAsync(Guid id)
        {
            var binaryId = id.ToByteArray();
            var reserva = await _repository.GetByIdAsync(binaryId);
            if (reserva == null) return null;

            return new ReservaDTO
            {
                ID = new Guid(reserva.ID).ToString(),
                Cliente_ID = new Guid(reserva.Cliente_ID).ToString(),
                Cliente_Nombre = reserva.Cliente?.Razon_Social ?? "Cliente no disponible",
                Fecha_Creacion = reserva.Fecha_Creacion,
                Estado_Reserva = reserva.Estado_Reserva,
                Monto_Total = reserva.Monto_Total
            };
        }

        public async Task AddAsync(ReservaCreateDTO dto)
        {
            var clienteId = Guid.Parse(dto.Cliente_ID).ToByteArray();
            var cliente = await _clienteRepository.GetByIdAsync(clienteId);
            
            if (cliente == null)
            {
                throw new NotFoundException($"No se encontró un cliente con ID: {dto.Cliente_ID}");
            }

            var reserva = new Reserva
            {
                ID = Guid.NewGuid().ToByteArray(),
                Cliente_ID = clienteId,
                Fecha_Creacion = DateTime.Now,
                Estado_Reserva = dto.Estado_Reserva,
                Monto_Total = dto.Monto_Total,
                Usuario_Creacion_ID = null
            };

            await _repository.AddAsync(reserva);
            await _repository.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Guid id, ReservaUpdateDTO dto)
        {
            var binaryId = id.ToByteArray();
            var reserva = await _repository.GetByIdAsync(binaryId);
            if (reserva == null) return false;

            if (!string.IsNullOrEmpty(dto.Cliente_ID))
            {
                var clienteId = Guid.Parse(dto.Cliente_ID).ToByteArray();
                var cliente = await _clienteRepository.GetByIdAsync(clienteId);
                if (cliente == null)
                {
                    throw new NotFoundException($"No se encontró un cliente con ID: {dto.Cliente_ID}");
                }
                reserva.Cliente_ID = clienteId;
            }
            
            if (!string.IsNullOrEmpty(dto.Estado_Reserva)) reserva.Estado_Reserva = dto.Estado_Reserva;
            if (dto.Monto_Total.HasValue) reserva.Monto_Total = dto.Monto_Total.Value;

            await _repository.UpdateAsync(reserva);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var binaryId = id.ToByteArray();
            var reserva = await _repository.GetByIdAsync(binaryId);
            if (reserva == null) return false;

            await _repository.DeleteAsync(binaryId);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
