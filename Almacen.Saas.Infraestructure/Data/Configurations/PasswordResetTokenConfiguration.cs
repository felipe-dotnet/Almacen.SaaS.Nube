using Almacen.Saas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Almacen.Saas.Infraestructure.Data.Configurations;

public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
{
    public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
    {
        builder.ToTable("PasswordResetTokens");
        
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Token)
            .IsRequired()
            .HasMaxLength(500);
        builder.Property(t => t.ExpiraEn)
            .IsRequired();

        //Navegacion Usuario
        builder.HasOne(t => t.Usuario)
            .WithMany(u => u.PasswordResetToken)
            .HasForeignKey(t => t.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
