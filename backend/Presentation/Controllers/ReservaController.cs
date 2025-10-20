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
        public async Task<ActionResult<IEnumerable<ReservaDTO>>> GetAll(
            [FromQuery] string? cliente_id = null,
            [FromQuery] string? estado_reserva = null,
            [FromQuery] DateTime? fecha_desde = null,
            [FromQuery] DateTime? fecha_hasta = null)
        {
            // Cargar reservas con cliente y detalles en memoria
            var reservasEntities = await _context.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.DetalleReservas)
                .ToListAsync();

            // Mapear en memoria a DTO
            var reservas = reservasEntities.Select(r =>
            {
                var primerDetalle = r.DetalleReservas != null && r.DetalleReservas.Any()
                    ? r.DetalleReservas.OrderBy(d => d.Fecha_Entrada).First()
                    : null;

                return new ReservaDTO
                {
                    ID = GuidToString(r.ID),
                    Cliente_ID = GuidToString(r.Cliente_ID),
                    Cliente_Nombre = r.Cliente != null ? r.Cliente.Razon_Social : null,
                    Fecha_Creacion = r.Fecha_Creacion,
                    Estado_Reserva = r.Estado_Reserva,
                    Monto_Total = r.Monto_Total,
                    Fecha_Entrada = primerDetalle?.Fecha_Entrada,
                    Fecha_Salida = primerDetalle?.Fecha_Salida
                };
            }).ToList();

            // Aplicar filtros en memoria
            if (!string.IsNullOrWhiteSpace(cliente_id))
                reservas = reservas.Where(r => r.Cliente_ID.Equals(cliente_id, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrWhiteSpace(estado_reserva))
                reservas = reservas.Where(r => r.Estado_Reserva.Equals(estado_reserva, StringComparison.OrdinalIgnoreCase)).ToList();

            if (fecha_desde.HasValue)
                reservas = reservas.Where(r => r.Fecha_Entrada.HasValue && r.Fecha_Entrada.Value >= fecha_desde.Value).ToList();

            if (fecha_hasta.HasValue)
                reservas = reservas.Where(r => r.Fecha_Salida.HasValue && r.Fecha_Salida.Value <= fecha_hasta.Value).ToList();

            return Ok(reservas);
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
                Fecha_Creacion = DateTime.Now,
                Estado_Reserva = dto.Estado_Reserva,
                Monto_Total = dto.Monto_Total,
                Usuario_Creacion_ID = null
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
                Fecha_Creacion = reserva.Fecha_Creacion,
                Estado_Reserva = reserva.Estado_Reserva,
                Monto_Total = reserva.Monto_Total
            };
            
            return Created($"/api/Reserva/{result.ID}", result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReservaDTO>> GetById(string id)
        {
            if (!Guid.TryParse(id, out var guid))
                return BadRequest("ID inválido.");

            var bytes = guid.ToByteArray();
            var reserva = await _context.Reservas
                .Include(r => r.Cliente)
                // Incluir la colección DetalleReservas y luego incluir sus relaciones:
                .Include(r => r.DetalleReservas)
                    .ThenInclude(d => d.Habitacion)
                .Include(r => r.DetalleReservas)
                    .ThenInclude(d => d.Huesped)
                .FirstOrDefaultAsync(r => r.ID != null && r.ID.SequenceEqual(bytes));
            
            if (reserva == null)
            
                return NotFound();

            var primerDetalle = reserva.DetalleReservas?.OrderBy(d => d.Fecha_Entrada).FirstOrDefault();

            return Ok(new ReservaDTO
            {
                ID = GuidToString(reserva.ID),
                Cliente_ID = GuidToString(reserva.Cliente_ID),
                Cliente_Nombre = reserva.Cliente?.Razon_Social,
                Fecha_Creacion = reserva.Fecha_Creacion,
                Estado_Reserva = reserva.Estado_Reserva,
                Monto_Total = reserva.Monto_Total,
                Fecha_Entrada = primerDetalle?.Fecha_Entrada,
                Fecha_Salida = primerDetalle?.Fecha_Salida
            });
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<ReservaDTO>> PartialUpdate(string id, [FromBody] ReservaUpdateDTO dto)
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
            
            if (!string.IsNullOrWhiteSpace(dto.Estado_Reserva)) reserva.Estado_Reserva = dto.Estado_Reserva;
            if (dto.Monto_Total.HasValue) reserva.Monto_Total = dto.Monto_Total.Value;

            await _context.SaveChangesAsync();

            return Ok(new ReservaDTO
            {
                ID = GuidToString(reserva.ID),
                Cliente_ID = GuidToString(reserva.Cliente_ID),
                Cliente_Nombre = reserva.Cliente?.Razon_Social,
                Fecha_Creacion = reserva.Fecha_Creacion,
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