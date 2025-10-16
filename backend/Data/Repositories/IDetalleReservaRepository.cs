using HotelManagement.Models;
using HotelManagement.DTOs;

namespace HotelManagement.Repositories
{
    public interface IDetalleReservaRepository
    {
        Task<List<DetalleReserva>> GetAllAsync();
        Task<DetalleReserva?> GetByIdAsync(byte[] id);
        Task<DetalleReserva> CreateAsync(DetalleReserva detalleReserva);
        Task<DetalleReserva> UpdateAsync(DetalleReserva detalleReserva);
        Task<bool> DeleteAsync(byte[] id);
        Task<List<DetalleReserva>> GetByReservaIdAsync(byte[] reservaId);
    }
}
