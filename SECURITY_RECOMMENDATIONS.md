# Recomendaciones de Seguridad - Almacén SaaS Nube

## Resumen Ejecutivo

Este documento detalla las vulnerabilidades de seguridad identificadas en la solución Almacén.SaaS.Nube y proporciona recomendaciones basadas en OWASP Top 10 2021.

---

## 1. FALTA DE AUTENTICACIÓN Y AUTORIZACIÓN

### Problema Crítico
- No hay decoradores `[Authorize]` en los endpoints de la API
- Todos los endpoints son públicos sin protección
- No existe implementación de JWT o autenticación basada en tokens

### Recomendación
```csharp
// Program.cs - Agregar autenticación JWT
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    };
});

builder.Services.AddAuthorization();
```

### En Controllers
```csharp
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UsuariosController : BaseController
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto) { ... }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id) { ... }
}
```

---

## 2. INYECCIÓN SQL

### Estado Actual
EL código utiliza Entity Framework, lo cual protege contra inyección SQL si se usan consultas parametrizadas.

### Riesgo Residual
Si hay consultas raw SQL, deben usar parámetros.

### Validación
```csharp
// ✅ CORRECTO - Parametrizado
var usuarios = await context.Usuarios
    .Where(u => u.Email == email)
    .ToListAsync();

// ❌ INCORRECTO - Vulnerable a inyección SQL
var query = $"SELECT * FROM Usuarios WHERE Email = '{email}'";
var usuarios = context.Usuarios.FromSqlInterpolated(query);
```

---

## 3. EXPOSICIÓN DE DATOS SENSIBLES

### Problemas Identificados

#### 3.1 Datos Sensibles en Logs
```csharp
// ❌ PROBLEMA - Logs exponen información sensible
_logger.LogInformation($"Obteniendo usuario por email: {email}");
_logger.LogInformation($"Creando nuevo usuario: {dto.Email}");
```

### Solución
```csharp
// ✅ CORRECTO - Sanitizar información sensible
_logger.LogInformation("Obteniendo usuario por email");
_logger.LogInformation("Creando nuevo usuario");
```

#### 3.2 Contraseñas en Tránsito
Asegurar que todas las contraseñas usen HTTPS y se transmitan en POST/PUT.

#### 3.3 Almacenamiento de Contraseñas
Implementar hashing fuerte:
```csharp
public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        // Usar bcrypt con costo de 12 o superior
        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
    }
    
    public bool Verify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
```

---

## 4. VALIDACIÓN INSUFICIENTE DE ENTRADA

### Problema
No se aplican validaciones robustas en DTOs de entrada.

### Solución
```csharp
public class CrearUsuarioDto
{
    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [MaxLength(255)]
    public string Email { get; set; }
    
    [Required]
    [MinLength(8, ErrorMessage = "La contraseña debe tener mínimo 8 caracteres")]
    [Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])",
        ErrorMessage = "La contraseña debe contener mayúsculas, minúsculas, números y símbolos")]
    public string Password { get; set; }
    
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Nombre { get; set; }
}
```

### Implementar Global Exception Handler
```csharp
app.UseExceptionHandler(errorApp => {
    errorApp.Run(async context => {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        var response = new { message = "Error interno del servidor", code = "INTERNAL_ERROR" };
        
        if (exception is FluentValidation.ValidationException vex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            response = new { message = "Errores de validación", errors = vex.Errors };
        }
        
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(response);
    });
});
```

---

## 5. CONTROL DE ACCESO BASADO EN ROLES (RBAC)

### Problema
No hay implementación de roles o permisos.

### Solución
```csharp
// Domain/Enums/RoleEnum.cs
public enum RoleEnum
{
    Admin = 1,
    Manager = 2,
    Operario = 3,
    Visualizador = 4
}

// En Controllers
[Authorize(Roles = "Admin")]
[HttpDelete("{id}")]
public async Task<IActionResult> Eliminar(int id)
{
    // Solo administradores pueden eliminar
}

[Authorize(Roles = "Admin,Manager")]
[HttpPut("{id}")]
public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarUsuarioDto dto)
{
    // Admin y Manager pueden actualizar
}
```

---

## 6. CROSS-ORIGIN RESOURCE SHARING (CORS)

### Problema Actual
```csharp
// Program.cs - CORS demasiado permisivo
policy.WithOrigins(
    "https://localhost:5001",
    "http://localhost:5000"
)
.AllowAnyMethod()    // ⚠️ Demasiado permisivo
.AllowAnyHeader()    // ⚠️ Demasiado permisivo
.AllowCredentials();
```

### Solución
```csharp
builder.Services.AddCors(options => {
    options.AddPolicy("AllowBlazorClient", policy => {
        policy.WithOrigins(
                "https://almacen-app.com",
                "https://www.almacen-app.com"
            )
            .WithMethods("GET", "POST", "PUT", "DELETE")
            .WithHeaders("Content-Type", "Authorization")
            .AllowCredentials()
            .WithExposedHeaders("X-Total-Count", "X-Page-Number");
    });
});
```

---

## 7. SEGURIDAD EN CABECERAS HTTP

### Agregar Headers de Seguridad
```csharp
app.Use(async (context, next) => {
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Add("Permissions-Policy", "geolocation=(), microphone=(), camera=()");
    
    await next();
});
```

---

## 8. GESTIÓN DE SECRETOS

### Problema
Los secretos podrían estar en appsettings.json (riesgo de exposición en repositorio).

### Solución
```csharp
// Program.cs
if (app.Environment.IsProduction())
{
    var azureKeyVaultUrl = builder.Configuration["KeyVault:Url"];
    if (!string.IsNullOrEmpty(azureKeyVaultUrl))
    {
        var credential = new DefaultAzureCredential();
        builder.Configuration.AddAzureKeyVault(
            new Uri(azureKeyVaultUrl),
            credential);
    }
}
```

---

## 9. RATE LIMITING

### Implementación
```csharp
builder.Services.AddRateLimiter(rateLimiterOptions => {
    rateLimiterOptions.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
        partitioner: context => 
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: context.User.FindFirst("sub")?.Value ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                factory: partition => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 100,
                    Window = TimeSpan.FromMinutes(1)
                }));
});

app.UseRateLimiter();
```

---

## 10. AUDITORÍA Y LOGGING DE SEGURIDAD

### Implementar Auditoría
```csharp
public class AuditLog
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Action { get; set; }
    public string Resource { get; set; }
    public string OldValue { get; set; }
    public string NewValue { get; set; }
    public DateTime Timestamp { get; set; }
    public string IpAddress { get; set; }
    public bool Success { get; set; }
}

public interface IAuditService
{
    Task LogAsync(string userId, string action, string resource, 
        string oldValue, string newValue, string ipAddress, bool success);
}
```

---

## 11. PRUEBAS DE SEGURIDAD

### Herramientas Recomendadas
- **SonarQube**: Análisis estático de código
- **OWASP ZAP**: Testing de penetración
- **Snyk**: Vulnerabilidades de dependencias
- **Burp Suite Community**: Testing web

### Unit Tests
```csharp
[TestClass]
public class PasswordHasherTests
{
    [TestMethod]
    public void Hash_ShouldNotReturnPlainPassword()
    {
        var hasher = new PasswordHasher();
        var password = "SecurePassword123!";
        var hash = hasher.Hash(password);
        
        Assert.AreNotEqual(password, hash);
    }
    
    [TestMethod]
    public void Verify_ShouldReturnTrue_ForCorrectPassword()
    {
        var hasher = new PasswordHasher();
        var password = "SecurePassword123!";
        var hash = hasher.Hash(password);
        
        Assert.IsTrue(hasher.Verify(password, hash));
    }
}
```

---

## 12. PLAN DE IMPLEMENTACIÓN

### Fase 1 (Crítica - Inmediato)
- [x] Implementar autenticación JWT
- [x] Agregar decoradores [Authorize]
- [x] Validar todas las entradas
- [x] Implementar hashing de contraseñas

### Fase 2 (Alta - 2 semanas)
- [ ] Implementar RBAC
- [ ] Añadir headers de seguridad HTTP
- [ ] Implementar rate limiting
- [ ] Configurar gestión de secretos

### Fase 3 (Media - 1 mes)
- [ ] Auditoría y logging
- [ ] Pruebas de seguridad
- [ ] Implementar HTTPS en todos los endpoints
- [ ] Documentación de seguridad

---

## Referencias OWASP
1. [OWASP Top 10 2021](https://owasp.org/Top10/)
2. [OWASP API Security](https://owasp.org/www-project-api-security/)
3. [OWASP Cheat Sheet Series](https://cheatsheetseries.owasp.org/)

---

**Documento actualizado:** 18/11/2025
**Prioridad:** CRÍTICA
