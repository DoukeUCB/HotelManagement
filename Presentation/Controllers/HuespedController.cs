using HotelManagement.Datos.Config;
using HotelManagement.DTOs;
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
                .Select(h => new HuespedDTO
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

        [HttpGet("{id}")]
        public async Task<ActionResult<HuespedDTO>> GetById(string id)
        {
            if (!Guid.TryParse(id, out var guid))
                return BadRequest("ID inválido.");

            var bytes = guid.ToByteArray();
            var huesped = await _context.Huespedes
                .FirstOrDefaultAsync(h => h.ID != null && h.ID.SequenceEqual(bytes));

            if (huesped == null)
                return NotFound();

            return Ok(new HuespedDTO
            {
                ID = GuidToString(huesped.ID),
                Nombre_Completo = huesped.Nombre_Completo,
                Documento_Identidad = huesped.Documento_Identidad,
                Telefono = huesped.Telefono,
                Email = huesped.Email,
                Fecha_Nacimiento = huesped.Fecha_Nacimiento
            });
        }

        [HttpPost]
        public async Task<ActionResult<HuespedDTO>> Create([FromBody] HuespedCreateDTO dto)
        {
            DateTime? fechaNacimiento = null;
            if (!string.IsNullOrWhiteSpace(dto.Fecha_Nacimiento))
            {
                if (!DateTime.TryParse(dto.Fecha_Nacimiento, out var parsedDate))
                    return BadRequest("Fecha_Nacimiento tiene un formato inválido.");
                fechaNacimiento = parsedDate;
            }

            var huesped = new HotelManagement.Models.Huesped
            {
                ID = Guid.NewGuid().ToByteArray(),
                Nombre_Completo = dto.Nombre_Completo,
                Documento_Identidad = dto.Documento_Identidad,
                Telefono = dto.Telefono,
                Email = dto.Email,
                Fecha_Nacimiento = fechaNacimiento
            };

            _context.Huespedes.Add(huesped);
            await _context.SaveChangesAsync();

            var result = new HuespedDTO
            {
                ID = GuidToString(huesped.ID),
                Nombre_Completo = huesped.Nombre_Completo,
                Documento_Identidad = huesped.Documento_Identidad,
                Telefono = huesped.Telefono,
                Email = huesped.Email,
                Fecha_Nacimiento = huesped.Fecha_Nacimiento
            };

            return CreatedAtAction(nameof(GetById), new { id = result.ID }, result);
        }

        private static string GuidToString(byte[] bytes) => new Guid(bytes).ToString();
    }
}
