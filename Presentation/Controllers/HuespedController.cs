using HotelManagement.Datos.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Presentacion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HuespedController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public HuespedController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var huespedes = await _context.Huespedes
                .Select(h => new
                {
                    ID = GuidToString(h.ID),
                    Nombre_Completo = h.Nombre_Completo,
                    Documento_Identidad = h.Documento_Identidad,
                    Telefono = h.Telefono,
                    Email = h.Email,
                    Fecha_Nacimiento = h.Fecha_Nacimiento
                })
                .ToListAsync();

            return Ok(huespedes);
        }

        private static string GuidToString(byte[] bytes) => new Guid(bytes).ToString();
    }
}
