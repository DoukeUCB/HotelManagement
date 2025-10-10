using HotelManagement.Application.Services;
using HotelManagement.DTOs;
using Microsoft.AspNetCore.Mvc;




namespace HotelManagement.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservaController : ControllerBase
    {
        private readonly IReservaService _service;

        public ReservaController(IReservaService service)
        {
            _service = service;
        }

        // GET: api/Reserva
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reservas = await _service.GetAllAsync();
            return Ok(reservas);
        }

        // GET: api/Reserva/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var reserva = await _service.GetByIdAsync(id);
            if (reserva == null)
                return NotFound();

            return Ok(reserva);
        }

        // POST: api/Reserva
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReservaCreateDTO dto)
        {
            await _service.AddAsync(dto);
            return Ok("Reserva creada correctamente.");
        }

        // PUT: api/Reserva/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ReservaUpdateDTO dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            if (!updated)
                return NotFound();

            return Ok("Reserva actualizada correctamente.");
        }

        // DELETE: api/Reserva/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return Ok("Reserva eliminada correctamente.");
        }
    }
}
