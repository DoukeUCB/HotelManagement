using HotelManagement.Models;

namespace HotelManagement.Datos.Repositories
{
    public interface IReservaRepository
    {
        Task<IEnumerable<Reserva>> GetAllAsync();
        Task<Reserva?> GetByIdAsync(byte[] id);
        Task AddAsync(Reserva reserva);
        Task UpdateAsync(Reserva reserva);
        Task DeleteAsync(byte[] id);
        Task SaveChangesAsync();
    }
}
