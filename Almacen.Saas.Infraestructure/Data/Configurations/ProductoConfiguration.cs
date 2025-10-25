using Almacen.Saas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Almacen.Saas.Infraestructure.Data.Configurations;
public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
{
    public void Configure(EntityTypeBuilder<Producto> builder)
    {
        builder.ToTable("Productos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nombre)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Descripcion)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(p => p.ImagenUrl)
            .HasMaxLength(500);

        builder.Property(p => p.UnidadMedida)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.Precio)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.Stock)
            .IsRequired();

        builder.Property(p => p.StockMinimo)
            .IsRequired();

        builder.Property(p => p.Disponible)
            .IsRequired();

        // Auditoría
        builder.Property(p => p.CreadoPor)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.ModificadoPor)
            .HasMaxLength(100);

        // Índices
        builder.HasIndex(p => p.Nombre);
        builder.HasIndex(p => p.Disponible);

        // Relaciones
        builder.HasMany(p => p.DetallesPedido)
            .WithOne(d => d.Producto)
            .HasForeignKey(d => d.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Movimientos)
            .WithOne(m => m.Producto)
            .HasForeignKey(m => m.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
