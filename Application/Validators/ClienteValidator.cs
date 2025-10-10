using HotelManagement.DTOs;
using HotelManagement.Aplicacion.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace HotelManagement.Aplicacion.Validators
{
    public class ClienteValidator : IClienteValidator
    {
        private readonly Datos.Config.HotelDbContext _context;

        public ClienteValidator(Datos.Config.HotelDbContext context)
        {
            _context = context;
        }

        public async Task ValidateCreateAsync(ClienteCreateDTO dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Razon_Social))
                errors.Add("La Razón Social es obligatoria.");
            
            if (string.IsNullOrWhiteSpace(dto.NIT))
                errors.Add("El NIT es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.Email) || !IsValidEmail(dto.Email))
                errors.Add("El Email es obligatorio y debe tener un formato válido.");
            
            // Validar unicidad del Email
            var emailExists = await _context.Clientes
                .AnyAsync(c => c.Email == dto.Email);
            if (emailExists)
                errors.Add($"Ya existe un cliente con el Email: {dto.Email}");

            if (errors.Any())
                throw new ValidationException(errors);
        }

        public async Task ValidateUpdateAsync(string id, ClienteUpdateDTO dto)
        {
            var errors = new List<string>();

            if (!IsValidUuid(id))
                errors.Add("ID debe ser un UUID válido.");

            // Validar que el cliente exista
            var guidBytes = Guid.TryParse(id, out Guid guid) ? guid.ToByteArray() : null;
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.ID.SequenceEqual(guidBytes!));

            if (cliente == null)
                throw new NotFoundException($"No se encontró el cliente con ID: {id}");

            // Validar Email (si se proporciona)
            if (!string.IsNullOrEmpty(dto.Email))
            {
                if (!IsValidEmail(dto.Email))
                    errors.Add("El Email debe tener un formato válido.");
                
                // Validar unicidad del Email, excluyendo al cliente actual
                var emailExists = await _context.Clientes
                    .AnyAsync(c => c.Email == dto.Email && !c.ID.SequenceEqual(guidBytes!));

                if (emailExists)
                    errors.Add($"Ya existe otro cliente con el Email: {dto.Email}");
            }

            if (errors.Any())
                throw new ValidationException(errors);
        }

        public async Task ValidateDeleteAsync(string id)
        {
            if (!IsValidUuid(id))
                throw new ValidationException("ID debe ser un UUID válido.");

            var guidBytes = ConvertToGuid(id);
            var exists = await _context.Clientes
                .AnyAsync(c => c.ID.SequenceEqual(guidBytes));
            
            if (!exists)
                throw new NotFoundException($"No se encontró el cliente con ID: {id}");

            // Opcional: Validar si tiene reservas asociadas (aunque la DB lo restringirá)
            var hasReservas = await _context.Reservas
                .AnyAsync(r => r.Cliente_ID.SequenceEqual(guidBytes));
            
            if (hasReservas)
                throw new ConflictException("No se puede eliminar el cliente porque tiene reservas asociadas.");
        }

        private bool IsValidUuid(string value) => Guid.TryParse(value, out _);

        private bool IsValidEmail(string email)
        {
            // Regex básico para validar formato de email
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private byte[] ConvertToGuid(string uuid) => Guid.Parse(uuid).ToByteArray();
    }
}