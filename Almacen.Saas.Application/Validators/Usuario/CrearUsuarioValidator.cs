using Almacen.Saas.Application.DTOs.Usuario;
using FluentValidation;

namespace Almacen.Saas.Application.Validators.Usuario;
public class CrearUsuarioValidator:AbstractValidator<CrearUsuarioDto>
{
    public CrearUsuarioValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.Apellido)
            .NotEmpty().WithMessage("El apellido es requerido")
            .MaximumLength(100).WithMessage("El apellido no puede exceder 100 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido")
            .EmailAddress().WithMessage("El email no es válido")
            .MaximumLength(200).WithMessage("El email no puede exceder 200 caracteres");

        RuleFor(x => x.Telefono)
            .NotEmpty().WithMessage("El teléfono es requerido")
            .Matches(@"^\d{10}$").WithMessage("El teléfono debe tener 10 dígitos");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres")
            .Matches(@"[A-Z]").WithMessage("La contraseña debe contener al menos una mayúscula")
            .Matches(@"[a-z]").WithMessage("La contraseña debe contener al menos una minúscula")
            .Matches(@"[0-9]").WithMessage("La contraseña debe contener al menos un número");

        RuleFor(x => x.Calle)
            .NotEmpty().WithMessage("La calle es requerida");

        RuleFor(x => x.CodigoPostal)
            .NotEmpty().WithMessage("El código postal es requerido")
            .Matches(@"^\d{5}$").WithMessage("El código postal debe tener 5 dígitos");

        RuleFor(x => x.RFC)
            .Matches(@"^[A-ZÑ&]{3,4}\d{6}[A-Z\d]{3}$").WithMessage("RFC inválido")
            .When(x => !string.IsNullOrEmpty(x.RFC));
    }
}
