using HotelManagement.DTOs;
using HotelManagement.Models;
using HotelManagement.Repositories;
using HotelManagement.Aplicacion.Validators;
using HotelManagement.Aplicacion.Exceptions;

namespace HotelManagement.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _repository;
        private readonly IClienteValidator _validator;

        public ClienteService(
            IClienteRepository repository,
            IClienteValidator validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<List<ClienteDTO>> GetAllAsync()
        {
            var clientes = await _repository.GetAllAsync();
            return clientes.Select(MapToDTO).ToList();
        }

        public async Task<ClienteDTO> GetByIdAsync(string id)
        {
            if (!Guid.TryParse(id, out Guid guid))
                throw new BadRequestException("El ID proporcionado no es un GUID válido.");

            var guidBytes = guid.ToByteArray();
            var cliente = await _repository.GetByIdAsync(guidBytes);

            if (cliente == null)
                throw new NotFoundException($"No se encontró el cliente con ID: {id}");

            return MapToDTO(cliente);
        }

        public async Task<ClienteDTO> CreateAsync(ClienteCreateDTO dto)
        {
            await _validator.ValidateCreateAsync(dto);

            var cliente = new Cliente
            {
                ID = Guid.NewGuid().ToByteArray(),
                Razon_Social = dto.Razon_Social,
                NIT = dto.NIT,
                Email = dto.Email
            };

            var created = await _repository.CreateAsync(cliente);
            return MapToDTO(created);
        }

        public async Task<ClienteDTO> UpdateAsync(string id, ClienteUpdateDTO dto)
        {
            await _validator.ValidateUpdateAsync(id, dto);

            var guidBytes = Guid.Parse(id).ToByteArray();
            var cliente = await _repository.GetByIdAsync(guidBytes);

            // La validación ya revisó que exista, pero es buena práctica de defensa
            if (cliente == null) 
                throw new NotFoundException($"No se encontró el cliente con ID: {id}");

            // Actualizar solo los campos proporcionados
            if (!string.IsNullOrEmpty(dto.Razon_Social))
                cliente.Razon_Social = dto.Razon_Social;
            
            if (!string.IsNullOrEmpty(dto.NIT))
                cliente.NIT = dto.NIT;

            if (!string.IsNullOrEmpty(dto.Email))
                cliente.Email = dto.Email;

            var updated = await _repository.UpdateAsync(cliente);
            return MapToDTO(updated);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            await _validator.ValidateDeleteAsync(id);
            var guidBytes = Guid.Parse(id).ToByteArray();
            
            // DeleteAsync retorna true si eliminó, false si no lo encontró.
            // La validación ya garantiza que existe, por lo que el resultado debería ser true.
            return await _repository.DeleteAsync(guidBytes);
        }

        private ClienteDTO MapToDTO(Cliente cliente)
        {
            return new ClienteDTO
            {
                ID = ByteArrayToGuid(cliente.ID),
                Razon_Social = cliente.Razon_Social,
                NIT = cliente.NIT,
                Email = cliente.Email
            };
        }

        private string ByteArrayToGuid(byte[] bytes) => new Guid(bytes).ToString();
    }
}