using Almacen.Saas.Application.DTOs.Producto;
using FluentValidation;

namespace Almacen.Saas.Application.Validators.Producto;

public class CrearProductoValidator : AbstractValidator<CrearProductoDto>
{
    public CrearProductoValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del producto es requerido");

        RuleFor(x => x.Descripcion)
            .NotEmpty().WithMessage("La descripción de producto requerida")
            .MaximumLength(500).WithMessage("La descripción del producto no puede exceder 500 caracteres");

        RuleFor(x => x.Precio)
            .GreaterThanOrEqualTo(0).WithMessage("El precio del producto no puede ser negativo");

        RuleFor(x => x.UnidadMedida)
            .NotEmpty().WithMessage("La unidad de medida es requerida");

    }
}
