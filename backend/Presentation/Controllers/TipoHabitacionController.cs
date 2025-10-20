using HotelManagement.Datos.Config;
using HotelManagement.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Presentacion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TipoHabitacionController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public TipoHabitacionController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tipos = await _context.TipoHabitaciones
                .Where(t => t.Activo)
                .Select(t => new TipoHabitacionDTO
                {
                    ID = GuidToString(t.ID),
                    Nombre = t.Nombre,
                    Capacidad_Maxima = t.Capacidad_Maxima,
                    Tarifa_Base = t.Precio_Base
                })
                .ToListAsync();

            return Ok(tipos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (!Guid.TryParse(id, out var guid))
                return BadRequest("ID inválido.");

            var bytes = guid.ToByteArray();
            var tipo = await _context.TipoHabitaciones
                .FirstOrDefaultAsync(t => t.ID != null && t.ID.SequenceEqual(bytes));

            if (tipo == null)
                return NotFound();

            var result = new TipoHabitacionDTO
            {
                ID = GuidToString(tipo.ID),
                Nombre = tipo.Nombre,
                Capacidad_Maxima = tipo.Capacidad_Maxima,
                Tarifa_Base = tipo.Precio_Base
            };

            return Ok(result);
        }

        private static string GuidToString(byte[] bytes) => new Guid(bytes).ToString();
    }
}
