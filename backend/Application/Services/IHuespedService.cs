using HotelManagement.DTOs;

namespace HotelManagement.Application.Services
{
    public interface IHuespedService
    {
        Task<List<HuespedDTO>> GetAllAsync();
        Task<HuespedDTO> GetByIdAsync(string id);
        Task<HuespedDTO> CreateAsync(HuespedCreateDTO dto);
        Task<HuespedDTO> UpdateAsync(string id, HuespedUpdateDTO dto);
        Task<bool> DeleteAsync(string id);
    }
}
