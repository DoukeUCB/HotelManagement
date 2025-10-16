using FluentValidation;
using HotelManagement.DTOs;

namespace HotelManagement.Validators
{
    public class ReservaCreateValidator : AbstractValidator<ReservaCreateDTO>
    {
        public ReservaCreateValidator()
        {
            RuleFor(x => x.Cliente_ID)
                .NotEmpty().WithMessage("El Cliente_ID es obligatorio.")
                .Must(BeAValidGuid).WithMessage("El Cliente_ID debe ser un GUID válido.");

            RuleFor(x => x.Monto_Total)
                .GreaterThanOrEqualTo(0).WithMessage("El monto total no puede ser negativo.");

            RuleFor(x => x.Estado_Reserva)
                .Must(e => new[] { "Pendiente", "Confirmada", "Cancelada", "Completada", "No-Show" }.Contains(e))
                .WithMessage("El estado de la reserva no es válido.");
        }

        private bool BeAValidGuid(string guidString)
        {
            return Guid.TryParse(guidString, out _);
        }
    }

    public class ReservaUpdateValidator : AbstractValidator<ReservaUpdateDTO>
    {
        public ReservaUpdateValidator()
        {
            RuleFor(x => x.Cliente_ID)
                .Must(BeAValidGuid!)
                .When(x => !string.IsNullOrEmpty(x.Cliente_ID))
                .WithMessage("El Cliente_ID debe ser un GUID válido.");

            RuleFor(x => x.Monto_Total)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Monto_Total.HasValue)
                .WithMessage("El monto total no puede ser negativo.");

            RuleFor(x => x.Estado_Reserva)
                .Must(e => new[] { "Pendiente", "Confirmada", "Cancelada", "Completada", "No-Show" }.Contains(e!))
                .When(x => !string.IsNullOrEmpty(x.Estado_Reserva))
                .WithMessage("El estado de la reserva no es válido.");
        }

        private bool BeAValidGuid(string guidString)
        {
            return Guid.TryParse(guidString, out _);
        }
    }
}
