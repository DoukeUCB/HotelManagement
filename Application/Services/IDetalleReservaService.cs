using HotelManagement.DTOs;

namespace HotelManagement.Services
{
    public interface IDetalleReservaService
    {
        Task<List<DetalleReservaDTO>> GetAllAsync();
        Task<DetalleReservaDTO> GetByIdAsync(string id);
        Task<DetalleReservaDTO> CreateAsync(DetalleReservaCreateDTO dto);
        Task<DetalleReservaDTO> UpdateAsync(string id, DetalleReservaUpdateDTO dto);
        Task<bool> DeleteAsync(string id);
        Task<List<DetalleReservaDTO>> GetByReservaIdAsync(string reservaId);
    }
}
