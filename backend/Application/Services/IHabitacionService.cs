using HotelManagement.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

// NOTA: Revisa tu estructura. Si tus servicios están bajo 'HotelManagement.Aplicacion',
// usa 'namespace HotelManagement.Aplicacion.Services' en su lugar.
namespace HotelManagement.Application.Services 
{
    public interface IHabitacionService
    {
        // CRUD Básico
        Task<IEnumerable<HabitacionDTO>> GetAllAsync();
        Task<HabitacionDTO> GetByIdAsync(string id);
        Task<HabitacionDTO> CreateAsync(HabitacionCreateDTO dto);
        Task<HabitacionDTO> UpdateAsync(string id, HabitacionUpdateDTO dto);
        
        // Operación de Eliminación
        Task<bool> DeleteAsync(string id);
        
        // Operación de Actualización Parcial (Necesario para el escenario de cambio de estado)
        Task<HabitacionDTO> PartialUpdateAsync(string id, HabitacionUpdateDTO dto);

        // Consultas específicas
        Task<IEnumerable<HabitacionDTO>> GetByTipoHabitacionIdAsync(string tipoHabitacionId);
        
        // Operación de disponibilidad (opcional, pero útil)
        Task<bool> IsHabitacionAvailableAsync(string id, System.DateTime fechaEntrada, System.DateTime fechaSalida);
    }
}