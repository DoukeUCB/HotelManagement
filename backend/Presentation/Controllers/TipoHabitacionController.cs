using HotelManagement.Datos.Config;
using HotelManagement.DTOs;
using HotelManagement.Models; // Asegúrate de que esta sea la ruta a tu modelo 'TipoHabitacion'
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System; // Necesario para DateTime.Now

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

        // --- MÉTODO PARA OBTENER TODOS LOS TIPOS (El que ya tenías) ---
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
                    Tarifa_Base = t.Precio_Base,
                    Activo = t.Activo // Añadido para que el DTO esté completo
                })
                .ToListAsync();

            return Ok(tipos);
        }

        // --- MÉTODO PARA OBTENER UN TIPO POR ID (El que ya tenías) ---
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
                Tarifa_Base = tipo.Precio_Base,
                Activo = tipo.Activo // Añadido para que el DTO esté completo
            };

            return Ok(result);
        }

        // --- MÉTODO PARA CREAR UN NUEVO TIPO (El que faltaba) ---
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TipoHabitacionCreateDTO dto)
        {
            // Asumo que tu modelo se llama 'TipoHabitacion'
            var tipoHabitacion = new TipoHabitacion
            {
                ID = Guid.NewGuid().ToByteArray(),
                Nombre = dto.Nombre,
                Capacidad_Maxima = dto.Capacidad_Maxima,
                // Mapeamos Tarifa_Base (del DTO) a Precio_Base (del modelo)
                Precio_Base = dto.Tarifa_Base, 
                Activo = true, // Por defecto, un nuevo tipo nace 'Activo'
                Fecha_Creacion = DateTime.Now 
            };

            _context.TipoHabitaciones.Add(tipoHabitacion);
            await _context.SaveChangesAsync();

            // Creamos el DTO que vamos a devolver
            var result = new TipoHabitacionDTO
            {
                ID = GuidToString(tipoHabitacion.ID),
                Nombre = tipoHabitacion.Nombre,
                Capacidad_Maxima = tipoHabitacion.Capacidad_Maxima,
                Tarifa_Base = tipoHabitacion.Precio_Base,
                Activo = tipoHabitacion.Activo
            };

            // Devolvemos un "201 Created"
            // Esto usa tu método 'GetById' para crear la URL del nuevo recurso
            return CreatedAtAction(nameof(GetById), new { id = result.ID }, result);
        }
        
        // --- MÉTODO PRIVADO DE AYUDA (El que ya tenías) ---
        private static string GuidToString(byte[] bytes) => new Guid(bytes).ToString();
    }
}