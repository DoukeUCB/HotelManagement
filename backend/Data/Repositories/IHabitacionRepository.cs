using HotelManagement.Models;

namespace HotelManagement.Repositories
{
    public interface IHabitacionRepository
    {
        Task<List<Habitacion>> GetAllAsync();
        Task<Habitacion?> GetByIdAsync(byte[] id);
        Task<Habitacion> CreateAsync(Habitacion habitacion);
        Task<Habitacion> UpdateAsync(Habitacion habitacion);
        Task<bool> DeleteAsync(byte[] id);
    }
}
