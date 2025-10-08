using Microsoft.AspNetCore.Mvc;
using HotelManagement.DTOs;
using HotelManagement.Services;

namespace HotelManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DetalleReservaController : ControllerBase
    {
        private readonly IDetalleReservaService _service;
        private readonly ILogger<DetalleReservaController> _logger;

        public DetalleReservaController(
            IDetalleReservaService service,
            ILogger<DetalleReservaController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<DetalleReservaDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<DetalleReservaDTO>>> GetAll()
        {
            _logger.LogInformation("Obteniendo todos los detalles de reservas");
            var detalles = await _service.GetAllAsync();
            return Ok(detalles);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DetalleReservaDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DetalleReservaDTO>> GetById(string id)
        {
            _logger.LogInformation("Obteniendo detalle de reserva con ID: {Id}", id);
            var detalle = await _service.GetByIdAsync(id);
            return Ok(detalle);
        }

        [HttpGet("reserva/{reservaId}")]
        [ProducesResponseType(typeof(List<DetalleReservaDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<DetalleReservaDTO>>> GetByReservaId(string reservaId)
        {
            _logger.LogInformation("Obteniendo detalles de la reserva: {ReservaId}", reservaId);
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
            return CreatedAtAction(nameof(GetById), new { id = created.ID }, created);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(DetalleReservaDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DetalleReservaDTO>> Update(string id, [FromBody] DetalleReservaUpdateDTO dto)
        {
            _logger.LogInformation("Actualizando detalle de reserva con ID: {Id}", id);
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id)
        {
            _logger.LogInformation("Eliminando detalle de reserva con ID: {Id}", id);
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
