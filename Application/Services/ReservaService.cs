using HotelManagement.DTOs;
using HotelManagement.Models;
using HotelManagement.Datos.Repositories;

namespace HotelManagement.Application.Services
{
    public class ReservaService : IReservaService
    {
        private readonly IReservaRepository _repository;

        public ReservaService(IReservaRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ReservaDTO>> GetAllAsync()
        {
            var reservas = await _repository.GetAllAsync();

            return reservas.Select(r => new ReservaDTO
            {
                ID = new Guid(r.ID).ToString(),
                Cliente_ID = new Guid(r.Cliente_ID).ToString(),
                Fecha_Reserva = r.Fecha_Reserva,
                Fecha_Entrada = r.Fecha_Entrada,
                Fecha_Salida = r.Fecha_Salida,
                Estado_Reserva = r.Estado_Reserva,
                Monto_Total = r.Monto_Total
            });
        }

        public async Task<ReservaDTO?> GetByIdAsync(Guid id)
        {
            var binaryId = GuidToMySqlBinary(id);
            var reserva = await _repository.GetByIdAsync(binaryId);
            if (reserva == null) return null;

            return new ReservaDTO
            {
                ID = new Guid(reserva.ID).ToString(),
                Cliente_ID = new Guid(reserva.Cliente_ID).ToString(),
                Fecha_Reserva = reserva.Fecha_Reserva,
                Fecha_Entrada = reserva.Fecha_Entrada,
                Fecha_Salida = reserva.Fecha_Salida,
                Estado_Reserva = reserva.Estado_Reserva,
                Monto_Total = reserva.Monto_Total
            };
        }

        public async Task AddAsync(ReservaCreateDTO dto)
        {
            var reserva = new Reserva
            {
                ID = GuidToMySqlBinary(Guid.NewGuid()),
                Cliente_ID = GuidToMySqlBinary(Guid.Parse(dto.Cliente_ID)),
                Fecha_Reserva = DateTime.Now,
                Fecha_Entrada = dto.Fecha_Entrada,
                Fecha_Salida = dto.Fecha_Salida,
                Estado_Reserva = dto.Estado_Reserva,
                Monto_Total = dto.Monto_Total
            };

            await _repository.AddAsync(reserva);
            await _repository.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Guid id, ReservaUpdateDTO dto)
        {
            var binaryId = GuidToMySqlBinary(id);
            var reserva = await _repository.GetByIdAsync(binaryId);
            if (reserva == null) return false;

            if (dto.Fecha_Entrada.HasValue) reserva.Fecha_Entrada = dto.Fecha_Entrada.Value;
            if (dto.Fecha_Salida.HasValue) reserva.Fecha_Salida = dto.Fecha_Salida.Value;
            if (!string.IsNullOrEmpty(dto.Estado_Reserva)) reserva.Estado_Reserva = dto.Estado_Reserva;
            if (dto.Monto_Total.HasValue) reserva.Monto_Total = dto.Monto_Total.Value;

            await _repository.UpdateAsync(reserva);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var binaryId = GuidToMySqlBinary(id);
            var reserva = await _repository.GetByIdAsync(binaryId);
            if (reserva == null) return false;

            await _repository.DeleteAsync(binaryId);
            await _repository.SaveChangesAsync();
            return true;
        }

        private static byte[] GuidToMySqlBinary(Guid guid)
        {
            var bytes = guid.ToByteArray();
            return new byte[]
            {
                bytes[3], bytes[2], bytes[1], bytes[0],
                bytes[5], bytes[4],
                bytes[7], bytes[6],
                bytes[8], bytes[9],
                bytes[10], bytes[11], bytes[12], bytes[13], bytes[14], bytes[15]
            };
        }
    }
}
