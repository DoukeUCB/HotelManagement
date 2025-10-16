using Microsoft.AspNetCore.Mvc;
using HotelManagement.DTOs;
using HotelManagement.Services;
using HotelManagement.Models;
using HotelManagement.Datos.Config;

namespace HotelManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DetalleReservaController : ControllerBase
    {
        private readonly IDetalleReservaService _service;
        private readonly ILogger<DetalleReservaController> _logger;
        private readonly HotelDbContext _context;

        public DetalleReservaController(
            IDetalleReservaService service,
            ILogger<DetalleReservaController> logger,
            HotelDbContext context)
        {
            _service = service;
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<DetalleReservaDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<DetalleReservaDTO>>> GetAll(
            [FromQuery] string? reserva_id = null,
            [FromQuery] string? habitacion_id = null,
            [FromQuery] string? huesped_id = null,
            [FromQuery] DateTime? fecha_entrada = null,
            [FromQuery] DateTime? fecha_salida = null)
        {
            _logger.LogInformation("Obteniendo todos los detalles de reservas");
            var detalles = await _service.GetAllAsync();
            
            // Aplicar filtros
            if (!string.IsNullOrWhiteSpace(reserva_id))
                detalles = detalles.Where(d => d.Reserva_ID.Equals(reserva_id, StringComparison.OrdinalIgnoreCase)).ToList();
            
            if (!string.IsNullOrWhiteSpace(habitacion_id))
                detalles = detalles.Where(d => d.Habitacion_ID.Equals(habitacion_id, StringComparison.OrdinalIgnoreCase)).ToList();
            
            if (!string.IsNullOrWhiteSpace(huesped_id))
                detalles = detalles.Where(d => d.Huesped_ID.Equals(huesped_id, StringComparison.OrdinalIgnoreCase)).ToList();
            
            if (fecha_entrada.HasValue)
                detalles = detalles.Where(d => d.Fecha_Entrada >= fecha_entrada.Value).ToList();
            
            if (fecha_salida.HasValue)
                detalles = detalles.Where(d => d.Fecha_Salida <= fecha_salida.Value).ToList();
            
            return Ok(detalles);
        }

        [HttpGet("reserva/{reservaId}")]
        [ProducesResponseType(typeof(List<DetalleReservaDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<DetalleReservaDTO>>> GetByReservaId(string reservaId)
        {
            _logger.LogInformation("Obteniendo detalles de reserva: {ReservaId}", reservaId);
            var detalles = await _service.GetByReservaIdAsync(reservaId);
            return Ok(detalles);
        }

        [HttpPost]
        [ProducesResponseType(typeof(DetalleReservaDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DetalleReservaDTO>> Create([FromBody] DetalleReservaCreateDTO dto)
        {
            _logger.LogInformation("Creando nuevo detalle de reserva");
            var created = await _service.CreateAsync(dto);
            return Created($"/api/DetalleReserva/{created.ID}", created);
        }

        /// <summary>
        /// Crea múltiples detalles de reserva (múltiples habitaciones con múltiples huéspedes)
        /// </summary>
        [HttpPost("multiple")]
        [ProducesResponseType(typeof(List<DetalleReservaDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<DetalleReservaDTO>>> CreateMultiple([FromBody] DetalleReservaMultipleCreateDTO dto)
        {
            _logger.LogInformation("Creando múltiples detalles para reserva: {ReservaId}", dto.Reserva_ID);

            var detallesCreados = new List<DetalleReservaDTO>();

            foreach (var habitacion in dto.Habitaciones)
            {
                foreach (var huespedId in habitacion.Huesped_IDs)
                {
                    var detalleDto = new DetalleReservaCreateDTO
                    {
                        Reserva_ID = dto.Reserva_ID,
                        Habitacion_ID = habitacion.Habitacion_ID,
                        Huesped_ID = huespedId,
                        Fecha_Entrada = habitacion.Fecha_Entrada,
                        Fecha_Salida = habitacion.Fecha_Salida
                    };

                    var creado = await _service.CreateAsync(detalleDto);
                    detallesCreados.Add(creado);
                }
            }

            return Created($"/api/DetalleReserva/reserva/{dto.Reserva_ID}", detallesCreados);
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(DetalleReservaDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DetalleReservaDTO>> PartialUpdate(string id, [FromBody] DetalleReservaUpdateDTO dto)
        {
            _logger.LogInformation("Actualizando parcialmente detalle de reserva con ID: {Id}", id);
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }
    }
}
