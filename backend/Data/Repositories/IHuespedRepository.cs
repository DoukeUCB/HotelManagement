using HotelManagement.Models;

namespace HotelManagement.Repositories
{
    public interface IHuespedRepository
    {
        Task<List<Huesped>> GetAllAsync();
        Task<Huesped?> GetByIdAsync(byte[] id);
        Task<Huesped?> GetByDocumentoAsync(string documento);
        Task<Huesped> CreateAsync(Huesped huesped);
        Task<Huesped> UpdateAsync(Huesped huesped);
        Task<bool> DeleteAsync(byte[] id);
    }
}
