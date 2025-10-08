using HotelManagement.Models;

namespace HotelManagement.Repositories
{
    public interface IClienteRepository
    {
        Task<List<Cliente>> GetAllAsync();
        Task<Cliente?> GetByIdAsync(byte[] id);
        Task<Cliente?> GetByEmailAsync(string email);
        Task<Cliente> CreateAsync(Cliente cliente);
        Task<Cliente> UpdateAsync(Cliente cliente);
        Task<bool> DeleteAsync(byte[] id);
    }
}