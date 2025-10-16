using HotelManagement.DTOs;

namespace HotelManagement.Application.Services
{
    public interface IReservaService
    {
        Task<IEnumerable<ReservaDTO>> GetAllAsync();
        Task<ReservaDTO?> GetByIdAsync(Guid id);
        Task AddAsync(ReservaCreateDTO dto);
        Task<bool> UpdateAsync(Guid id, ReservaUpdateDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
