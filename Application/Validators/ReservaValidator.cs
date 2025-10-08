using FluentValidation;
using HotelManagement.DTOs;

namespace HotelManagement.Validators
{
    public class ReservaCreateValidator : AbstractValidator<ReservaCreateDTO>
    {
        public ReservaCreateValidator()
        {
            RuleFor(x => x.Cliente_ID)
                .NotEmpty().WithMessage("El Cliente_ID es obligatorio.");

            RuleFor(x => x.Fecha_Entrada)
                .NotEmpty().WithMessage("La Fecha de Entrada es obligatoria.")
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage("La Fecha de Entrada no puede ser anterior a hoy.");

            RuleFor(x => x.Fecha_Salida)
                .NotEmpty().WithMessage("La Fecha de Salida es obligatoria.")
                .GreaterThan(x => x.Fecha_Entrada)
                .WithMessage("La Fecha de Salida debe ser posterior a la Fecha de Entrada.");

            RuleFor(x => x.Monto_Total)
                .GreaterThanOrEqualTo(0).WithMessage("El monto total no puede ser negativo.");

            RuleFor(x => x.Estado_Reserva)
                .Must(e => new[] { "Pendiente", "Confirmada", "Cancelada", "Completada", "No-Show" }.Contains(e))
                .WithMessage("El estado de la reserva no es válido.");
        }
    }

    public class ReservaUpdateValidator : AbstractValidator<ReservaUpdateDTO>
    {
        public ReservaUpdateValidator()
        {
            RuleFor(x => x.Fecha_Entrada)
                .GreaterThanOrEqualTo(DateTime.Today)
                .When(x => x.Fecha_Entrada.HasValue)
                .WithMessage("La Fecha de Entrada no puede ser anterior a hoy.");

            RuleFor(x => x.Fecha_Salida)
                .GreaterThan(x => x.Fecha_Entrada)
                .When(x => x.Fecha_Entrada.HasValue && x.Fecha_Salida.HasValue)
                .WithMessage("La Fecha de Salida debe ser posterior a la Fecha de Entrada.");

            RuleFor(x => x.Monto_Total)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Monto_Total.HasValue)
                .WithMessage("El monto total no puede ser negativo.");

            RuleFor(x => x.Estado_Reserva)
                .Must(e => new[] { "Pendiente", "Confirmada", "Cancelada", "Completada", "No-Show" }.Contains(e!))
                .When(x => !string.IsNullOrEmpty(x.Estado_Reserva))
                .WithMessage("El estado de la reserva no es válido.");
        }
    }
}
