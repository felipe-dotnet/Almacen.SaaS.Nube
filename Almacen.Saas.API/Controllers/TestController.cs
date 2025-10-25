using Almacen.Saas.Domain.Entities;
using Almacen.Saas.Infraestructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Almacen.Saas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TestController> _logger;

        public TestController(
        IUnitOfWork unitOfWork,
        ILogger<TestController> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new
            {
                status = "OK",
                timestamp = DateTime.UtcNow,
                message = "API funcionando correctamente"
            });
        }

        [HttpGet("database")]
        public async Task<IActionResult> TestDatabase()
        {
            try
            {
                // Probar conexión contando usuarios
                var usuariosCount = await _unitOfWork.Repository<Usuario>().CountAsync();
                var productosCount = await _unitOfWork.Repository<Producto>().CountAsync();

                return Ok(new
                {
                    status = "Connected",
                    usuarios = usuariosCount,
                    productos = productosCount,
                    message = "Conexión a base de datos exitosa"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al conectar con la base de datos");
                return StatusCode(500, new
                {
                    status = "Error",
                    message = ex.Message
                });
            }
        }

        [HttpPost("seed-test-data")]
        public async Task<IActionResult> SeedTestData()
        {
            try
            {
                // Crear un producto de prueba
                var producto = new Producto
                {
                    Nombre = "Manzana Roja",
                    Descripcion = "Manzanas frescas importadas",
                    Precio = 35.50m,
                    Stock = 100,
                    StockMinimo = 20,
                    UnidadMedida = Domain.Enums.UnidadMedida.Kilogramo,
                    Disponible = true,
                    CreadoPor = "Sistema"
                };

                await _unitOfWork.Repository<Producto>().AddAsync(producto);
                await _unitOfWork.SaveChangesAsync();

                return Ok(new
                {
                    message = "Datos de prueba creados",
                    producto = new
                    {
                        producto.Id,
                        producto.Nombre,
                        producto.Precio
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear datos de prueba");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
