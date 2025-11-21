using Almacen.Saas.Domain.Entities;
using Almacen.Saas.Infraestructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace Almacen.Saas.API.Test;

public class IntegrationTestFixture : WebApplicationFactory<Program>
{
    protected async override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remover el descriptor de DbContextOptions<ApplicationDbContext>
            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Agregar el contexto usando InMemory
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });

            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();

            // SEED de usuario real para pruebas
            db.Usuarios.Add(new Usuario
            {
                Email = "usuario@prueba.com",
                PasswordHash = "123456",
                Nombre = "Usuario Test",
                Activo = true,
                Apellido = "Apellido",
                Calle = "Calle Falsa 123",
                Ciudad = "Ciudad",
                CodigoPostal = "12345",
                Colonia = "Colonia",
                Estado = "Estado",
                Telefono = "555-1234",
                Rol = Domain.Enums.RolUsuario.Administrador
            });

            db.SaveChangesAsync();
        });
    }
}
