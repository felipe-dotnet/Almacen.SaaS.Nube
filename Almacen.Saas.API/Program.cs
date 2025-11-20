using Almacen.Saas.Application.Mappings;
using Almacen.Saas.Application.Services.Implementations;
using Almacen.Saas.Application.Services.Interfaces;
using Almacen.Saas.Application.Services.Utilities;
using Almacen.Saas.Domain.Interfaces;
using Almacen.Saas.Domain.Services;
using Almacen.Saas.Domain.Settings;
using Almacen.Saas.Infraestructure.Data;
using Almacen.Saas.Infraestructure.Repositories;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Almacen.Saas.Application.Services.Authentication;
using Almacen.Saas.Application.Services.Implementations.Authentication;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. CONFIGURACIÓN DE CONEXIÓN A BD
// ============================================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

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



// ============================================================
// 2. CONFIGURACIÓN DE SETTINGS
// ============================================================
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
builder.Services.Configure<WhatsAppSettings>(builder.Configuration.GetSection("WhatsApp"));

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()
    ?? throw new InvalidOperationException("JwtSettings not found in configuration");

// ============================================================
// 3. MAPSTER - MAPEO DE OBJETOS
// ============================================================
MappingConfig.RegisterMappings();
var config = TypeAdapterConfig.GlobalSettings;
builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper, Mapper>();

// ============================================================
// 4. REGISTRAR SERVICIOS DE DOMINIO
// ============================================================
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddScoped<IWhatsAppService, TwilioWhatsAppService>();
builder.Services.AddScoped<INotificacionChannelService, NotificacionChannelService>();

// ============================================================
// 5. REGISTRAR SERVICIOS DE APLICACIÓN
// ============================================================
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IProductoService, ProductService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<IFacturaService, FacturaService>();
builder.Services.AddScoped<IMovimientoInventarioService, MovimientoInventarioService>();
builder.Services.AddScoped<INotificacionService, NotificacionService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

// ============================================================
// 5.1 AUTENTICACIÓN JWT
// ============================================================
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Token validation failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated successfully");
            return Task.CompletedTask;
        }
    };
});

// ============================================================
// 6. SWAGGER - DOCUMENTACIÓN API
// ============================================================
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Almacén SaaS - API REST",
        Version = "v1.0.0",
        Description = "API completa para gestión de almacén en la nube. Incluye gestión de productos, pedidos, facturas, inventario y notificaciones.",
        Contact = new OpenApiContact
        {
            Name = "Aplicaciones-TI",
            Email = "soporte@aplicacionesti.com.mx",
            Url = new Uri("https://aplicacionesti.com.mx")
        },
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://opensource.org/licenses/MIT")
        },
        TermsOfService = new Uri("https://www.tuempresa.com/terms")


    });

    // Configuración de seguridad para JWT en Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Por favor ingresa tu token JWT",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },

        Array.Empty<string>()
    }
});

    // Comentarios XML para documentación
    var xmlFile = "Almacen.Saas.API.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Configuración de Swagger
    options.EnableAnnotations();
    options.UseInlineDefinitionsForEnums();
    options.OrderActionsBy(x => x.RelativePath);
});

// ============================================================
// 7. CORS - CONFIGURACIÓN DE ORÍGENES PERMITIDOS
// ============================================================
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

// ============================================================
// 8. CONTROLADORES
// ============================================================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });
builder.Services.AddEndpointsApiExplorer();

// ============================================================
// 9. LOGGING
// ============================================================
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ============================================================
// CONSTRUIR LA APLICACIÓN
// ============================================================
var app = builder.Build();

// ============================================================
// 10. MIDDLEWARE
// ============================================================

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "swagger/{documentName}/swagger.json";
    });

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Almacén SaaS API v1.0");
        options.RoutePrefix = string.Empty;
        options.DefaultModelsExpandDepth(2);
        options.DefaultModelExpandDepth(2);
        options.DocExpansion(DocExpansion.List);
        options.DisplayOperationId();
        options.DisplayRequestDuration();
        options.DocumentTitle = "Almacén SaaS API - Swagger";
    });
}

// HTTPS Redirection
app.UseHttpsRedirection();

// CORS
app.UseCors("AllowBlazorClient");

// Autenticación y Autorización
app.UseAuthentication();
app.UseAuthorization();

// Mapear controladores
app.MapControllers();

// ============================================================
// 11. INICIALIZACIÓN DE BASE DE DATOS
// ============================================================
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

// ============================================================
// 12. INFORMACIÓN DE INICIO
// ============================================================
app.Lifetime.ApplicationStarted.Register(() =>
{
    Console.WriteLine("\n========================================");
    Console.WriteLine(" 🚀 Almacén SaaS API Iniciada");
    Console.WriteLine("========================================");
    Console.WriteLine($" 📍 Environment: {app.Environment.EnvironmentName}");
    Console.WriteLine($" 🌐 URL: {app.Urls.FirstOrDefault()}");
    Console.WriteLine($" 📚 Swagger: {app.Urls.FirstOrDefault()}/");
    Console.WriteLine("========================================\n");
});

app.Run();

// Solamente para soporte de tests de integración:
public partial class Program { }