using Almacen.Saas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Almacen.Saas.Infraestructure.Data.Configurations;
public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Apellido)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.Telefono)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.Rol)
            .IsRequired()
            .HasConversion<int>();

        // Dirección
        builder.Property(u => u.Calle)
            .HasMaxLength(200);

        builder.Property(u => u.NumeroExterior)
            .HasMaxLength(20);

        builder.Property(u => u.NumeroInterior)
            .HasMaxLength(20);

        builder.Property(u => u.Colonia)
            .HasMaxLength(100);

        builder.Property(u => u.Ciudad)
            .HasMaxLength(100);

        builder.Property(u => u.Estado)
            .HasMaxLength(100);

        builder.Property(u => u.CodigoPostal)
            .HasMaxLength(10);

        // Datos Fiscales
        builder.Property(u => u.RFC)
            .HasMaxLength(13);

        builder.Property(u => u.RazonSocial)
            .HasMaxLength(250);

        builder.Property(u => u.DireccionFiscal)
            .HasMaxLength(500);

        // Auditoría
        builder.Property(u => u.CreadoPor)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.ModificadoPor)
            .HasMaxLength(100);

        // Relaciones
        builder.HasMany(u => u.Pedidos)
            .WithOne(p => p.Usuario)
            .HasForeignKey(p => p.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Notificaciones)
            .WithOne(n => n.Usuario)
            .HasForeignKey(n => n.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
