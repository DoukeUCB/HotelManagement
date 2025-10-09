using HotelManagement.Models;
using HotelManagement.Datos.Config;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly HotelDbContext _context;

        public ClienteRepository(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<List<Cliente>> GetAllAsync()
        {
            return await _context.Clientes.ToListAsync();
        }

        public async Task<Cliente?> GetByIdAsync(byte[] id)
        {
            return await _context.Clientes
                .FirstOrDefaultAsync(c => c.ID.SequenceEqual(id));
        }
        
        // Método para validar unicidad
        public async Task<Cliente?> GetByEmailAsync(string email)
        {
            return await _context.Clientes
                .FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<Cliente> CreateAsync(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            return cliente;
        }

        public async Task<Cliente> UpdateAsync(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();
            return cliente;
        }

        public async Task<bool> DeleteAsync(byte[] id)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.ID.SequenceEqual(id));
            
            if (cliente == null)
                return false;

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}