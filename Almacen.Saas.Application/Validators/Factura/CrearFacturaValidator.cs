
using Almacen.Saas.Application.DTOs.Factura;
using FluentValidation;

namespace Almacen.Saas.Application.Validators.Factura;
public class CrearFacturaValidator:AbstractValidator<CrearFacturaDto>
{
    public CrearFacturaValidator()
    {
        RuleFor(x => x.PedidoId)
            .GreaterThan(0).WithMessage("El pedido es requerido");

        RuleFor(x => x.RFC)
            .NotEmpty().WithMessage("El RFC es requerido")
            .Matches(@"^[A-ZÑ&]{3,4}\d{6}[A-Z\d]{3}$").WithMessage("RFC inválido");

        RuleFor(x => x.RazonSocial)
            .NotEmpty().WithMessage("La razón social es requerida")
            .MaximumLength(250).WithMessage("La razón social no puede exceder 250 caracteres");

        RuleFor(x => x.DireccionFiscal)
            .NotEmpty().WithMessage("La dirección fiscal es requerida")
            .MaximumLength(500).WithMessage("La dirección fiscal no puede exceder 500 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido")
            .EmailAddress().WithMessage("El email no es válido");
    }
}
