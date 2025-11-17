using HotelManagement.Models;
using HotelManagement.Datos.Config;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Repositories
{
    public class HuespedRepository : IHuespedRepository
    {
        private readonly HotelDbContext _context;

        public HuespedRepository(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<List<Huesped>> GetAllAsync()
        {
            return await _context.Huespedes.ToListAsync();
        }

        public async Task<Huesped?> GetByIdAsync(byte[] id)
        {
            return await _context.Huespedes.FindAsync(id);
        }

        public async Task<Huesped?> GetByDocumentoAsync(string documento)
        {
            return await _context.Huespedes
                .FirstOrDefaultAsync(h => h.Documento_Identidad == documento);
        }

        public async Task<Huesped> CreateAsync(Huesped huesped)
        {
            _context.Huespedes.Add(huesped);
            await _context.SaveChangesAsync();
            return huesped;
        }

        public async Task<Huesped> UpdateAsync(Huesped huesped)
        {
            _context.Huespedes.Update(huesped);
            await _context.SaveChangesAsync();
            return huesped;
        }

        public async Task<bool> DeleteAsync(byte[] id)
        {
            var huesped = await _context.Huespedes.FindAsync(id);
            
            if (huesped == null)
                return false;

            _context.Huespedes.Remove(huesped);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
