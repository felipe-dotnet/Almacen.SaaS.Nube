using Almacen.Saas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Almacen.Saas.Infraestructure.Data.Configurations;
public class FacturaConfiguration : IEntityTypeConfiguration<Factura>
{
    public void Configure(EntityTypeBuilder<Factura> builder)
    {
        builder.ToTable("Facturas");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.FolioFiscal)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(f => f.FolioFiscal)
            .IsUnique();

        builder.Property(f => f.FechaEmision)
            .IsRequired();

        builder.Property(f => f.RFC)
            .IsRequired()
            .HasMaxLength(13);

        builder.Property(f => f.RazonSocial)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(f => f.DireccionFiscal)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(f => f.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(f => f.Subtotal)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(f => f.IVA)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(f => f.Total)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(f => f.XmlUrl)
            .HasMaxLength(500);

        builder.Property(f => f.PdfUrl)
            .HasMaxLength(500);

        // Auditoría
        builder.Property(f => f.CreadoPor)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.ModificadoPor)
            .HasMaxLength(100);

        // Índices
        builder.HasIndex(f => f.PedidoId);

        // Relaciones
        builder.HasOne(f => f.Pedido)
            .WithOne(p => p.Factura)
            .HasForeignKey<Factura>(f => f.PedidoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
