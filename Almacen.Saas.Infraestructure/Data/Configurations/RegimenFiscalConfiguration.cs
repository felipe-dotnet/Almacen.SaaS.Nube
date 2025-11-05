using Almacen.Saas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Almacen.Saas.Infraestructure.Data.Configurations;

public class RegimenFiscalConfiguration : IEntityTypeConfiguration<RegimenFiscal>
{
    public void Configure(EntityTypeBuilder<RegimenFiscal> builder)
    {
        builder.ToTable("RegimenesFiscales");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Codigo)
            .IsRequired()
            .HasMaxLength(10);
        
        builder.Property(r => r.Descripcion)
            .IsRequired()
            .HasMaxLength(200);        
    }
}
