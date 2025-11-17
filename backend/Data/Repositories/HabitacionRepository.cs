using HotelManagement.Models;
using HotelManagement.Datos.Config;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Repositories
{
    public class HabitacionRepository : IHabitacionRepository
    {
        private readonly HotelDbContext _context;

        public HabitacionRepository(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<List<Habitacion>> GetAllAsync()
        {
            return await _context.Habitaciones
                .Include(h => h.TipoHabitacion)
                .ToListAsync();
        }

        public async Task<Habitacion?> GetByIdAsync(byte[] id)
        {
            var habitacion = await _context.Habitaciones.FindAsync(id);
            if (habitacion != null)
            {
                await _context.Entry(habitacion).Reference(h => h.TipoHabitacion).LoadAsync();
            }
            return habitacion;
        }

        public async Task<Habitacion> CreateAsync(Habitacion habitacion)
        {
            _context.Habitaciones.Add(habitacion);
            await _context.SaveChangesAsync();
            return habitacion;
        }

        public async Task<Habitacion> UpdateAsync(Habitacion habitacion)
        {
            _context.Habitaciones.Update(habitacion);
            await _context.SaveChangesAsync();
            return habitacion;
        }

        public async Task<bool> DeleteAsync(byte[] id)
        {
            var habitacion = await _context.Habitaciones.FindAsync(id);
            
            if (habitacion == null)
                return false;

            _context.Habitaciones.Remove(habitacion);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
