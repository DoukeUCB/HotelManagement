using HotelManagement.Datos.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Presentacion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HabitacionController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public HabitacionController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var habitaciones = await _context.Habitaciones
                .Include(h => h.TipoHabitacion)
                .Select(h => new
                {
                    ID = GuidToString(h.ID),
                    Numero_Habitacion = h.Numero_Habitacion,
                    Piso = h.Piso,
                    Estado_Habitacion = h.Estado_Habitacion,
                    Tipo_Nombre = h.TipoHabitacion != null ? h.TipoHabitacion.Nombre_Tipo : null,
                    Capacidad_Maxima = h.TipoHabitacion != null ? h.TipoHabitacion.Capacidad_Maxima : (byte?)null,
                    Tarifa_Base = h.TipoHabitacion != null ? h.TipoHabitacion.Precio_Base : (decimal?)null
                })
                .ToListAsync();

            return Ok(habitaciones);
        }

        private static string GuidToString(byte[] bytes) => new Guid(bytes).ToString();
    }
}
