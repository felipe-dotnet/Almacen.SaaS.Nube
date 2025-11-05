using Almacen.Saas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Almacen.Saas.Infraestructure.Data.Configurations;

public class DatosFiscalesConfiguration : IEntityTypeConfiguration<DatosFiscales>
{
    public void Configure(EntityTypeBuilder<DatosFiscales> builder)
    {
        builder.ToTable("DatosFiscales");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.RFC)
            .IsRequired()
            .HasMaxLength(14);

        builder.Property(d => d.TipoPersonaFiscal)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(d => d.CodigoPostal)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(d => d.RegimenFiscalId)
            .IsRequired();

        builder.Property(d => d.RazonSocial)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.Calle)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.NumExterior)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(d => d.NumInt)
            .HasMaxLength(20);

        builder.Property(d => d.Referencia)
            .HasMaxLength(200);

        builder.HasOne(d => d.Usuario)
            .WithMany(u => u.DatosFiscales)
            .HasForeignKey(d => d.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.RegimenFiscal)
            .WithMany()
            .HasForeignKey("RegimenFiscalId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
