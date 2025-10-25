using Almacen.Saas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Almacen.Saas.Infraestructure.Data.Configurations;
public class MovimientoInventarioConfiguration : IEntityTypeConfiguration<MovimientoInventario>
{
    public void Configure(EntityTypeBuilder<MovimientoInventario> builder)
    {
        builder.ToTable("MovimientosInventario");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.TipoMovimiento)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(m => m.Cantidad)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(m => m.StockAnterior)
            .IsRequired();

        builder.Property(m => m.StockNuevo)
            .IsRequired();

        builder.Property(m => m.Motivo)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(m => m.Referencia)
            .HasMaxLength(100);

        // Auditoría
        builder.Property(m => m.CreadoPor)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.ModificadoPor)
            .HasMaxLength(100);

        // Índices
        builder.HasIndex(m => m.ProductoId);
        builder.HasIndex(m => m.TipoMovimiento);
        builder.HasIndex(m => m.FechaCreacion);

        // Relaciones
        builder.HasOne(m => m.Producto)
            .WithMany(p => p.Movimientos)
            .HasForeignKey(m => m.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
