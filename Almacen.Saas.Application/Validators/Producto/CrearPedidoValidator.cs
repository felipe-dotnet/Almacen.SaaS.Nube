using Almacen.Saas.Application.DTOs.Pedido;
using FluentValidation;

namespace Almacen.Saas.Application.Validators.Producto;

public class CrearPedidoValidator:AbstractValidator<CrearPedidoDto>
{
    public CrearPedidoValidator()
    {
        RuleFor(x => x.UsuarioId)
            .GreaterThan(0).WithMessage("El usuario es requerido");

        RuleFor(x => x.DireccionEnvio)
            .NotEmpty().WithMessage("La dirección de envío es requerida")
            .MaximumLength(500).WithMessage("La dirección no puede exceder 500 caracteres");

        RuleFor(x => x.CostoEnvio)
            .GreaterThanOrEqualTo(0).WithMessage("El costo de envío no puede ser negativo");

        RuleFor(x => x.Detalles)
            .NotEmpty().WithMessage("El pedido debe tener al menos un producto");

        RuleForEach(x => x.Detalles).ChildRules(detalle =>
        {
            detalle.RuleFor(d => d.ProductoId)
                .GreaterThan(0).WithMessage("Producto inválido");

            detalle.RuleFor(d => d.Cantidad)
                .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0");
        });
    }
}
