using HotelManagement.Datos.Config;
using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Datos.Repositories
{
    public class ReservaRepository : IReservaRepository
    {
        private readonly HotelDbContext _context;

        public ReservaRepository(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Reserva>> GetAllAsync()
        {
            return await _context.Reservas
                .Include(r => r.Cliente)
                .ToListAsync();
        }

        public async Task<Reserva?> GetByIdAsync(byte[] id)
        {
            return await _context.Reservas
                .Include(r => r.Cliente)
                .FirstOrDefaultAsync(r => r.ID.SequenceEqual(id));
        }

        public async Task AddAsync(Reserva reserva)
        {
            await _context.Reservas.AddAsync(reserva);
        }

        public async Task UpdateAsync(Reserva reserva)
        {
            _context.Reservas.Update(reserva);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(byte[] id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva != null)
                _context.Reservas.Remove(reserva);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
