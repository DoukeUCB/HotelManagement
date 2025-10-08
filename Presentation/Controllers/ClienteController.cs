using Microsoft.AspNetCore.Mvc;
using HotelManagement.Services;
using HotelManagement.DTOs;
using System.Net;

namespace HotelManagement.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        // GET: api/Cliente
        [HttpGet]
        [ProducesResponseType(typeof(List<ClienteDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll()
        {
            var clientes = await _clienteService.GetAllAsync();
            return Ok(clientes);
        }

        // GET: api/Cliente/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ClienteDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetById(string id)
        {
            var cliente = await _clienteService.GetByIdAsync(id);
            return Ok(cliente);
        }

        // POST: api/Cliente
        [HttpPost]
        [ProducesResponseType(typeof(ClienteDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)] // Para emails duplicados
        public async Task<IActionResult> Create([FromBody] ClienteCreateDTO dto)
        {
            var cliente = await _clienteService.CreateAsync(dto);
            // Retorna 201 Created con la ubicación del nuevo recurso
            return CreatedAtAction(nameof(GetById), new { id = cliente.ID }, cliente);
        }

        // PUT: api/Cliente/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ClienteDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Update(string id, [FromBody] ClienteUpdateDTO dto)
        {
            var cliente = await _clienteService.UpdateAsync(id, dto);
            return Ok(cliente);
        }

        // DELETE: api/Cliente/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _clienteService.DeleteAsync(id);
            // Aunque el servicio valida si existe, si no lo hiciera, 
            // no devolveríamos 404 para un DELETE idempotente, pero aquí sí lo hacemos.
            if (!result) return NotFound(); 
            
            return NoContent(); // 204 No Content
        }
    }
}