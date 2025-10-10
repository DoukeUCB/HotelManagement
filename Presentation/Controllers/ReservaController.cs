using HotelManagement.Datos.Config;
using HotelManagement.DTOs;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HotelManagement.Presentacion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservaController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public ReservaController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservaDTO>>> GetAll()
        {
            var reservas = await _context.Reservas
                .Include(r => r.Cliente)
                .Select(r => new ReservaDTO
                {
                    ID = GuidToString(r.ID),
                    Cliente_ID = GuidToString(r.Cliente_ID),
                    Cliente_Nombre = r.Cliente != null ? r.Cliente.Razon_Social : null,
                    Fecha_Reserva = r.Fecha_Reserva,
                    Fecha_Entrada = r.Fecha_Entrada,
                    Fecha_Salida = r.Fecha_Salida,
                    Estado_Reserva = r.Estado_Reserva,
                    Monto_Total = r.Monto_Total
                })
                .ToListAsync();

            return Ok(reservas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReservaDTO>> GetById(string id)
        {
            if (!Guid.TryParse(id, out var guid))
                return BadRequest("ID inválido.");

            var bytes = guid.ToByteArray();
            var reserva = await _context.Reservas
                .FirstOrDefaultAsync(r => r.ID != null && r.ID.SequenceEqual(bytes));
            if (reserva == null)
                return NotFound();

            return Ok(new ReservaDTO
            {
                ID = GuidToString(reserva.ID),
                Cliente_ID = GuidToString(reserva.Cliente_ID),
                Cliente_Nombre = reserva.Cliente != null ? reserva.Cliente.Razon_Social : null,
                Fecha_Reserva = reserva.Fecha_Reserva,
                Fecha_Entrada = reserva.Fecha_Entrada,
                Fecha_Salida = reserva.Fecha_Salida,
                Estado_Reserva = reserva.Estado_Reserva,
                Monto_Total = reserva.Monto_Total
            });
        }

        [HttpPost]
        public async Task<ActionResult<ReservaDTO>> Create([FromBody] ReservaCreateDTO dto)
        {
            if (!Guid.TryParse(dto.Cliente_ID, out var clienteGuid))
                return BadRequest("Cliente_ID inválido.");

            var clienteBytes = clienteGuid.ToByteArray();
            var clienteExists = await _context.Clientes
                .AnyAsync(c => c.ID != null && c.ID.SequenceEqual(clienteBytes));
            if (!clienteExists)
                return BadRequest("El cliente especificado no existe.");

            var reserva = new Reserva
            {
                ID = Guid.NewGuid().ToByteArray(),
                Cliente_ID = clienteBytes,
                Fecha_Reserva = dto.Fecha_Reserva ?? DateTime.Now,
                Fecha_Entrada = dto.Fecha_Entrada,
                Fecha_Salida = dto.Fecha_Salida,
                Estado_Reserva = dto.Estado_Reserva,
                Monto_Total = dto.Monto_Total
            };

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            var result = new ReservaDTO
            {
                ID = GuidToString(reserva.ID),
                Cliente_ID = dto.Cliente_ID,
                Cliente_Nombre = await _context.Clientes
                    .Where(c => c.ID != null && c.ID.SequenceEqual(clienteBytes))
                    .Select(c => c.Razon_Social)
                    .FirstOrDefaultAsync(),
                Fecha_Reserva = reserva.Fecha_Reserva,
                Fecha_Entrada = reserva.Fecha_Entrada,
                Fecha_Salida = reserva.Fecha_Salida,
                Estado_Reserva = reserva.Estado_Reserva,
                Monto_Total = reserva.Monto_Total
            };
return CreatedAtAction(nameof(GetById), new { id = result.ID }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ReservaDTO>> Update(string id, [FromBody] ReservaUpdateDTO dto)
        {
            if (!Guid.TryParse(id, out var reservaGuid))
                return BadRequest("ID inválido.");

            var bytes = reservaGuid.ToByteArray();
            var reserva = await _context.Reservas
                .Include(r => r.Cliente)
                .FirstOrDefaultAsync(r => r.ID != null && r.ID.SequenceEqual(bytes));

            if (reserva == null)
                return NotFound();

            if (!string.IsNullOrWhiteSpace(dto.Cliente_ID))
            {
                if (!Guid.TryParse(dto.Cliente_ID, out var clienteGuid))
                    return BadRequest("Cliente_ID inválido.");

                var clienteBytes = clienteGuid.ToByteArray();
                var clienteExists = await _context.Clientes
                    .AnyAsync(c => c.ID != null && c.ID.SequenceEqual(clienteBytes));

                if (!clienteExists)
                    return BadRequest("El cliente especificado no existe.");

                reserva.Cliente_ID = clienteBytes;
            }

            if (dto.Fecha_Entrada.HasValue) reserva.Fecha_Entrada = dto.Fecha_Entrada.Value;
            if (dto.Fecha_Salida.HasValue) reserva.Fecha_Salida = dto.Fecha_Salida.Value;
            if (!string.IsNullOrWhiteSpace(dto.Estado_Reserva)) reserva.Estado_Reserva = dto.Estado_Reserva;
            if (dto.Monto_Total.HasValue) reserva.Monto_Total = dto.Monto_Total.Value;

            await _context.SaveChangesAsync();

            return Ok(new ReservaDTO
            {
                ID = GuidToString(reserva.ID),
                Cliente_ID = GuidToString(reserva.Cliente_ID),
                Cliente_Nombre = reserva.Cliente?.Razon_Social,
                Fecha_Reserva = reserva.Fecha_Reserva,
                Fecha_Entrada = reserva.Fecha_Entrada,
                Fecha_Salida = reserva.Fecha_Salida,
                Estado_Reserva = reserva.Estado_Reserva,
                Monto_Total = reserva.Monto_Total
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!Guid.TryParse(id, out var guid))
                return BadRequest("ID inválido.");

            var bytes = guid.ToByteArray();
            var reserva = await _context.Reservas
                .FirstOrDefaultAsync(r => r.ID != null && r.ID.SequenceEqual(bytes));

            if (reserva == null)
                return NotFound();

            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private static string GuidToString(byte[] bytes) => bytes == null ? string.Empty : new Guid(bytes).ToString();
    }
}