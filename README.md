# Almacen.SaaS.Nube

Sistema de gestión de almacén e inventario desarrollado como SaaS (Software as a Service) utilizando .NET y arquitectura limpia (Clean Architecture) con patrones DDD.

## 📋 Descripción

**Almacen.SaaS.Nube** es una solución integral para la gestión de almacenes e inventarios en la nube, diseñada con las mejores prácticas de desarrollo de software empresarial. El sistema permite administrar productos, pedidos, movimientos de inventario, facturación y notificaciones de forma eficiente y escalable.

## 🏗️ Arquitectura

El proyecto implementa **Clean Architecture** (Arquitectura Limpia) separando las responsabilidades en capas bien definidas:

```
Almacen.SaaS.Nube/
│
├── Almacen.Saas.API/                 # Capa de Presentación (API REST)
├── Almacen.Saas.Application/         # Capa de Aplicación
├── Almacen.Saas.Domain/              # Capa de Dominio
├── Almacen.Saas.Infraestructure/     # Capa de Infraestructura
└── Almacen.Sass.Web/                 # Capa de Presentación Web
```

### Capas del Sistema

#### 🎯 **Almacen.Saas.API**
Capa de presentación que expone endpoints RESTful para interactuar con el sistema.

**Controladores principales:**
- `ProductosController` - Gestión de productos
- `PedidosController` - Gestión de pedidos
- `UsuariosController` - Gestión de usuarios
- `BaseController` - Controlador base con funcionalidades comunes

**Características:**
- Endpoints RESTful
- Validación de datos de entrada
- Manejo centralizado de errores
- Documentación con Swagger/OpenAPI

#### 🔧 **Almacen.Saas.Application**
Capa de lógica de negocio que orquesta las operaciones del sistema.

**Estructura:**
```
Application/
├── Common/                    # Clases comunes y utilitarias
│   ├── PagedResultDto.cs     # Paginación de resultados
│   ├── Result.cs             # Patrón Result para manejo de respuestas
│   └── ServiceException.cs   # Excepciones personalizadas
│
├── DTOs/                      # Data Transfer Objects
│   ├── Dashboard/            # DTOs para métricas y dashboard
│   ├── Factura/              # DTOs de facturación
│   ├── MovimientoInventario/ # DTOs de movimientos
│   ├── Notificacion/         # DTOs de notificaciones
│   ├── Pedido/               # DTOs de pedidos
│   ├── Producto/             # DTOs de productos
│   └── Usuario/              # DTOs de usuarios
│
├── Mappings/                  # Perfiles de AutoMapper
│
├── Services/                  # Servicios de aplicación
│   ├── Interfaces/
│   │   ├── IDashboardService.cs
│   │   ├── IFacturaService.cs
│   │   ├── IMovimientoInventarioService.cs
│   │   ├── INotificacionService.cs
│   │   ├── IPedidoService.cs
│   │   ├── IProductoService.cs
│   │   └── IUsuarioService.cs
│   ├── Implementations/       # Implementaciones de servicios
│   └── Utilities/             # Utilidades compartidas
│
└── Validators/                # Validadores con FluentValidation
```

**Servicios implementados:**
- `IDashboardService` - Métricas y estadísticas del sistema
- `IProductoService` - Gestión de productos y catálogo
- `IPedidoService` - Procesamiento de pedidos
- `IUsuarioService` - Administración de usuarios
- `IFacturaService` - Generación y gestión de facturas
- `IMovimientoInventarioService` - Control de stock e inventario
- `INotificacionService` - Sistema de notificaciones

#### 🏛️ **Almacen.Saas.Domain**
Capa central que contiene las entidades de negocio y reglas de dominio.

**Estructura:**
```
Domain/
├── Common/                    # Clases base y comunes
├── Entities/                  # Entidades de dominio
│   ├── Producto.cs
│   ├── Pedido.cs
│   ├── DetallePedido.cs
│   ├── Usuario.cs
│   ├── Factura.cs
│   ├── MovimientoInventario.cs
│   └── Notificacion.cs
│
├── Enums/                     # Enumeraciones del dominio
├── Interfaces/                # Contratos del dominio
│   ├── IRepository.cs        # Repositorio genérico
│   └── IUnitOfWork.cs        # Patrón Unit of Work
│
├── Services/                  # Servicios de dominio
│   ├── IEmailService.cs
│   ├── IWhatsAppService.cs
│   ├── IPasswordHasher.cs
│   └── INotificacionChannelService.cs
│
└── Settings/                  # Configuraciones del dominio
```

**Entidades principales:**
- **Producto** - Artículos del inventario
- **Pedido** - Órdenes de compra/venta
- **DetallePedido** - Líneas de pedido
- **Usuario** - Usuarios del sistema
- **Factura** - Documentos de facturación
- **MovimientoInventario** - Entradas/salidas de stock
- **Notificacion** - Registro de notificaciones enviadas

#### 🗄️ **Almacen.Saas.Infraestructure**
Capa de infraestructura que implementa persistencia y servicios externos.

**Estructura:**
```
Infraestructure/
├── Data/
│   ├── ApplicationDbContext.cs    # Contexto de Entity Framework
│   └── Configurations/             # Configuraciones de entidades
│
├── Repositories/
│   ├── Repository.cs               # Implementación del repositorio genérico
│   └── UnitOfWork.cs               # Implementación de Unit of Work
│
└── Migrations/                     # Migraciones de base de datos
```

**Tecnologías:**
- Entity Framework Core para ORM
- SQL Server / PostgreSQL (configurable)
- Patrón Repository + Unit of Work
- Migrations para versionado de BD

#### 🌐 **Almacen.Sass.Web**
Interfaz web del sistema para usuarios finales.

## 🚀 Características Principales

### ✅ Gestión de Productos
- Registro y edición de productos
- Control de stock y existencias
- Categorización de productos
- Precios y costos

### 📦 Gestión de Pedidos
- Creación de pedidos
- Seguimiento de estado
- Detalles de pedido con múltiples productos
- Historial completo

### 📊 Dashboard Analítico
- Métricas en tiempo real
- Estadísticas de ventas
- Inventario bajo stock
- Reportes personalizables

### 👥 Gestión de Usuarios
- Autenticación segura
- Hash de contraseñas
- Roles y permisos
- Gestión de sesiones

### 💰 Facturación
- Generación automática de facturas
- Detalles de facturación
- Historial de transacciones

### 📱 Sistema de Notificaciones
- **Email** - Notificaciones por correo electrónico
- **WhatsApp** - Integración con WhatsApp Business API
- Canales configurables
- Templates personalizables

### 📈 Movimientos de Inventario
- Entradas y salidas de stock
- Trazabilidad completa
- Ajustes de inventario
- Auditoría de movimientos

## 🛠️ Tecnologías y Patrones

### Stack Tecnológico
- **.NET 6/7/8** - Framework principal
- **C#** - Lenguaje de programación
- **Entity Framework Core** - ORM
- **SQL Server** - Base de datos
- **AutoMapper** - Mapeo de objetos
- **FluentValidation** - Validación de datos
- **MediatR** - Patrón Mediator (opcional)

### Patrones de Diseño
- **Clean Architecture** - Separación de responsabilidades
- **Domain-Driven Design (DDD)** - Diseño guiado por el dominio
- **Repository Pattern** - Abstracción de acceso a datos
- **Unit of Work** - Transacciones atómicas
- **DTO Pattern** - Transferencia de datos
- **Result Pattern** - Manejo de respuestas
- **Dependency Injection** - Inyección de dependencias
- **CQRS** - Separación de comandos y consultas (si aplica)

### Principios SOLID
- ✅ Single Responsibility
- ✅ Open/Closed
- ✅ Liskov Substitution
- ✅ Interface Segregation
- ✅ Dependency Inversion

## 📦 Instalación y Configuración

### Prerrequisitos
```bash
- .NET SDK 6.0 o superior
- SQL Server 2019+ o PostgreSQL 12+
- Visual Studio 2022 o JetBrains Rider
- Git
```

### Pasos de Instalación

1. **Clonar el repositorio**
```bash
git clone https://github.com/felipe-dotnet/Almacen.SaaS.Nube.git
cd Almacen.SaaS.Nube
```

2. **Restaurar dependencias**
```bash
dotnet restore
```

3. **Configurar la cadena de conexión**

Editar `appsettings.json` en `Almacen.Saas.API`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AlmacenSaasDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "Username": "tu-email@gmail.com",
    "Password": "tu-password"
  },
  "WhatsAppSettings": {
    "ApiUrl": "https://api.whatsapp.com",
    "ApiKey": "tu-api-key"
  }
}
```

4. **Aplicar migraciones**
```bash
cd Almacen.Saas.Infraestructure
dotnet ef database update
```

5. **Ejecutar el proyecto**
```bash
cd ../Almacen.Saas.API
dotnet run
```

La API estará disponible en: `https://localhost:5001` o `http://localhost:5000`

## 📚 Uso de la API

### Swagger Documentation
Una vez ejecutado el proyecto, acceder a:
```
https://localhost:5001/swagger
```

### Ejemplos de Endpoints

#### Productos
```http
GET    /api/productos              # Listar productos
GET    /api/productos/{id}         # Obtener producto por ID
POST   /api/productos              # Crear producto
PUT    /api/productos/{id}         # Actualizar producto
DELETE /api/productos/{id}         # Eliminar producto
```

#### Pedidos
```http
GET    /api/pedidos                # Listar pedidos
GET    /api/pedidos/{id}           # Obtener pedido por ID
POST   /api/pedidos                # Crear pedido
PUT    /api/pedidos/{id}           # Actualizar estado
```

#### Usuarios
```http
GET    /api/usuarios               # Listar usuarios
POST   /api/usuarios/register      # Registrar usuario
POST   /api/usuarios/login         # Iniciar sesión
```

## 🧪 Testing

```bash
# Ejecutar tests unitarios
dotnet test

# Con cobertura
dotnet test /p:CollectCoverage=true
```

## 📈 Roadmap

- [ ] Implementación de MediatR para CQRS
- [ ] Autenticación JWT completa
- [ ] Integración con servicios de pago
- [ ] Reportes en PDF
- [ ] Exportación a Excel
- [ ] Módulo de análisis predictivo
- [ ] App móvil (Xamarin/MAUI)
- [ ] Integración con ERP externos

## 🤝 Contribuciones

Las contribuciones son bienvenidas. Por favor:

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📄 Licencia

Este proyecto es privado y confidencial.

## 👥 Autores

- **Felipe** - [@felipe-dotnet](https://github.com/felipe-dotnet)
- **Uribe** - [@uribe2211](https://github.com/uribe2211)

## 📞 Contacto

Para consultas o soporte, contactar a través de GitHub Issues.

---

⭐ Si este proyecto te resulta útil, considera darle una estrella en GitHub!
