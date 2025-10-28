using Almacen.Saas.Application.DTOs.Usuario;
using FluentValidation;

namespace Almacen.Saas.Application.Validators.Usuario;

public class ActualizarUsuarioValidator:AbstractValidator<ActualizarUsuarioDto>
{
   public ActualizarUsuarioValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El Id del usuario es requerido y debe ser mayor que cero");
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");
        RuleFor(x => x.Apellido)
            .NotEmpty().WithMessage("El apellido es requerido")
            .MaximumLength(100).WithMessage("El apellido no puede exceder 100 caracteres");       
        RuleFor(x => x.Telefono)
            .NotEmpty().WithMessage("El teléfono es requerido")
            .Matches(@"^\d{10}$").WithMessage("El teléfono debe tener 10 dígitos");
        RuleFor(x => x.Calle)
            .NotEmpty().WithMessage("La calle es requerida");
        RuleFor(x => x.CodigoPostal)
            .NotEmpty().WithMessage("El código postal es requerido")
            .Matches(@"^\d{5}$").WithMessage("El código postal debe tener 5 dígitos");
    }
}
