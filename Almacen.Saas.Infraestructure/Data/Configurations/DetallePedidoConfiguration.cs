using Almacen.Saas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Almacen.Saas.Infraestructure.Data.Configurations;
public class DetallePedidoConfiguration:IEntityTypeConfiguration<DetallePedido>
{
    public void Configure(EntityTypeBuilder<DetallePedido> builder)
    {
        builder.ToTable("DetallesPedido");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.NombreProducto)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.Cantidad)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(d => d.PrecioUnitario)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(d => d.Subtotal)
            .IsRequired()
            .HasPrecision(18, 2);

        // Índices
        builder.HasIndex(d => d.PedidoId);
        builder.HasIndex(d => d.ProductoId);

        // Relaciones
        builder.HasOne(d => d.Pedido)
            .WithMany(p => p.Detalles)
            .HasForeignKey(d => d.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Producto)
            .WithMany(p => p.DetallesPedido)
            .HasForeignKey(d => d.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
