using HotelManagement.Models;
using HotelManagement.Datos.Config;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Repositories
{
    public class DetalleReservaRepository : IDetalleReservaRepository
    {
        private readonly HotelDbContext _context;

        public DetalleReservaRepository(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<List<DetalleReserva>> GetAllAsync()
        {
            return await _context.DetalleReservas
                .Include(d => d.Reserva)
                .Include(d => d.Habitacion)
                .Include(d => d.Huesped)
                .ToListAsync();
        }

        public async Task<DetalleReserva?> GetByIdAsync(byte[] id)
        {
            return await _context.DetalleReservas
                .Include(d => d.Reserva)
                .Include(d => d.Habitacion)
                .Include(d => d.Huesped)
                .FirstOrDefaultAsync(d => d.ID.SequenceEqual(id));
        }

        public async Task<DetalleReserva> CreateAsync(DetalleReserva detalleReserva)
        {
            _context.DetalleReservas.Add(detalleReserva);
            await _context.SaveChangesAsync();
            return detalleReserva;
        }

        public async Task<DetalleReserva> UpdateAsync(DetalleReserva detalleReserva)
        {
            _context.DetalleReservas.Update(detalleReserva);
            await _context.SaveChangesAsync();
            return detalleReserva;
        }

        public async Task<bool> DeleteAsync(byte[] id)
        {
            var detalle = await _context.DetalleReservas
                .FirstOrDefaultAsync(d => d.ID.SequenceEqual(id));
            
            if (detalle == null)
                return false;

            _context.DetalleReservas.Remove(detalle);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<DetalleReserva>> GetByReservaIdAsync(byte[] reservaId)
        {
            return await _context.DetalleReservas
                .Include(d => d.Habitacion)
                .Include(d => d.Huesped)
                .Where(d => d.Reserva_ID.SequenceEqual(reservaId))
                .ToListAsync();
        }
    }
}
