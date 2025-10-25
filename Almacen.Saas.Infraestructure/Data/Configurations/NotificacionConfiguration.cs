using Almacen.Saas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Almacen.Saas.Infraestructure.Data.Configurations;
public class NotificacionConfiguration:IEntityTypeConfiguration<Notificacion>
{
    public void Configure(EntityTypeBuilder<Notificacion> builder)
    {
        builder.ToTable("Notificaciones");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Tipo)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(n => n.Titulo)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(n => n.Mensaje)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(n => n.Leida)
            .IsRequired();

        builder.Property(n => n.Referencia)
            .HasMaxLength(100);

        // Índices
        builder.HasIndex(n => n.UsuarioId);
        builder.HasIndex(n => n.Leida);
        builder.HasIndex(n => n.FechaCreacion);

        // Relaciones
        builder.HasOne(n => n.Usuario)
            .WithMany(u => u.Notificaciones)
            .HasForeignKey(n => n.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
