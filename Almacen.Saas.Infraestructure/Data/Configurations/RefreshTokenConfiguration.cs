using Almacen.Saas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Almacen.Saas.Infraestructure.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(rt => rt.JwtId)
            .IsRequired()
            .HasMaxLength(200);        
        builder.Property(rt => rt.ExpiresAt)
            .IsRequired();
        builder.Property(rt => rt.CreatedAt)
            .IsRequired();
        builder.Property(rt => rt.IsRevoked)
            .IsRequired();
        //Navigation property
        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(rt => rt.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
