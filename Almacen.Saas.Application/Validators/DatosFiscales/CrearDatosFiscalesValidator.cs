using Almacen.Saas.Application.DTOs.DatosFiscales;
using FluentValidation;

namespace Almacen.Saas.Application.Validators.DatosFiscales;
public class CrearDatosFiscalesValidator:AbstractValidator<CrearDatosFiscalesDto>
{
    public CrearDatosFiscalesValidator()
    {
        RuleFor(x => x.RazonSocial)
            .NotEmpty().WithMessage("La razón social es requerida")
            .MaximumLength(200).WithMessage("La razón social no puede exceder 200 caracteres");

        RuleFor(x => x.RFC)
            .NotEmpty().WithMessage("El RFC es requerido")
            .Matches(@"^[A-ZÑ&]{3,4}\d{6}[A-Z0-9]{3}$").WithMessage("El RFC no es válido");

        RuleFor(x => x.Calle)
            .NotEmpty().WithMessage("La dirección es requerida")
            .MaximumLength(300).WithMessage("La dirección no puede exceder 300 caracteres");
        
        RuleFor(x => x.NumeroExterior)
            .NotEmpty().WithMessage("El número exterior es requerido")
            .MaximumLength(10).WithMessage("El número exterior no puede exceder 10 caracteres");

        RuleFor(x => x.RegimenFiscalId)
            .GreaterThan(0).WithMessage("El ID del cliente debe ser mayor que cero");
        
        RuleFor(x=> x.UsuarioId)
            .GreaterThan(0).WithMessage("El ID del usuario debe ser mayor que cero");
        
        RuleFor(x => x.CodigoPostal)
            .NotEmpty().WithMessage("El código postal es requerido")
            .Matches(@"^\d{5}$").WithMessage("El código postal debe tener 5 dígitos");
        
        RuleFor(x => x.TipoPersona)
            .InclusiveBetween(1, 2).WithMessage("El tipo de persona debe ser 1 (Física) o 2 (Moral)");
    }
}
