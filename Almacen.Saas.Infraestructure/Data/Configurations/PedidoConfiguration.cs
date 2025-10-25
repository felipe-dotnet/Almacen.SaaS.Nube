using Almacen.Saas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Almacen.Saas.Infraestructure.Data.Configurations;
public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("Pedidos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.NumeroPedido)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(p => p.NumeroPedido)
            .IsUnique();

        builder.Property(p => p.FechaPedido)
            .IsRequired();

        builder.Property(p => p.Estado)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.Subtotal)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.Impuestos)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.CostoEnvio)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.Total)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.DireccionEnvio)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.Observaciones)
            .HasMaxLength(1000);

        builder.Property(p => p.NotasInternas)
            .HasMaxLength(1000);

        // Auditoría
        builder.Property(p => p.CreadoPor)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.ModificadoPor)
            .HasMaxLength(100);

        // Índices
        builder.HasIndex(p => p.UsuarioId);
        builder.HasIndex(p => p.Estado);
        builder.HasIndex(p => p.FechaPedido);

        // Relaciones
        builder.HasOne(p => p.Usuario)
            .WithMany(u => u.Pedidos)
            .HasForeignKey(p => p.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Repartidor)
            .WithMany()
            .HasForeignKey(p => p.RepartidorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Detalles)
            .WithOne(d => d.Pedido)
            .HasForeignKey(d => d.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Factura)
            .WithOne(f => f.Pedido)
            .HasForeignKey<Factura>(f => f.PedidoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
