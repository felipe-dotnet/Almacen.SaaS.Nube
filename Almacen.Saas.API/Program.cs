using Almacen.Saas.Application.Mappings;
using Almacen.Saas.Infraestructure.Data;
using Almacen.Saas.Infraestructure.Repositories;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Registrar configuración de Mapster
MappingConfig.RegisterMappings();

// Registrar IMapper de Mapster
var config = TypeAdapterConfig.GlobalSettings;
builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper, Mapper>();

// Add services to the container.

// ============================================
// 1. CONFIGURACIÓN DE SERVICIOS
// ============================================

// Obtener la cadena de conexión (viene de User Secrets en Development)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Configurar DbContext con SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);

    // Solo en Development: mostrar queries SQL en consola
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Registrar Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ============================================
// 2. CONFIGURACIÓN DE CORS
// ============================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins(
                "https://localhost:5001",  // Blazor Server HTTPS
                "http://localhost:5000",   // Blazor Server HTTP
                "https://localhost:7001",  // Puertos alternativos
                "http://localhost:7000"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// ============================================
// 4. CONFIGURACIÓN DE LOGGING
// ============================================
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowBlazorClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ============================================
// 7. INICIALIZACIÓN DE BASE DE DATOS
// ============================================
// Aplicar migraciones automáticamente en Development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        // Verificar si hay migraciones pendientes
        if (context.Database.GetPendingMigrations().Any())
        {
            Console.WriteLine("⏳ Aplicando migraciones pendientes...");
            context.Database.Migrate();
            Console.WriteLine("✅ Migraciones aplicadas exitosamente");
        }
        else
        {
            Console.WriteLine("✅ Base de datos actualizada");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "❌ Error al aplicar migraciones");
    }
}

// ============================================
// 8. INFORMACIÓN DE INICIO
// ============================================
app.Lifetime.ApplicationStarted.Register(() =>
{
    Console.WriteLine("========================================");
    Console.WriteLine(" Almacen.Saas API Iniciada");
    Console.WriteLine($" Environment: {app.Environment.EnvironmentName}");
    Console.WriteLine($" URL: {app.Urls.FirstOrDefault()}");
    Console.WriteLine("  Swagger: https://localhost:5001/");
    Console.WriteLine("========================================");
});

app.Run();
