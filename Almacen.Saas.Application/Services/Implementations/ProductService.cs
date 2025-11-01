using Almacen.Saas.Application.Common;
using Almacen.Saas.Application.DTOs.Producto;
using Almacen.Saas.Application.Services.Interfaces;
using Almacen.Saas.Application.Validators.Producto;
using Almacen.Saas.Domain.Entities;
using Almacen.Saas.Domain.Interfaces;
using Mapster;
using Microsoft.Extensions.Logging;

namespace Almacen.Saas.Application.Services.Implementations;
public class ProductService : IProductoService
{
    #region Constructor
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IUnitOfWork unitOfWork, ILogger<ProductService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    #endregion


    public async Task<Result<ProductoDto>> CrearAsync(CrearProductoDto dto)
    {
        try
        {
            var validator = new CrearProductoValidator();
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<ProductoDto>.FailureResult("Validación fallida", errors);
            }
            var producto = dto.Adapt<Producto>();

            await _unitOfWork.Repository<Producto>().AddAsync(producto);
            await _unitOfWork.SaveChangesAsync();

            var productoDto = producto.Adapt<ProductoDto>();
            return Result<ProductoDto>.SuccessResult(productoDto, "Producto creado exitosamente");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el producto");
            return Result<ProductoDto>.FailureResult("Error al crear el producto", [ex.Message]);
        }

    }

    public async Task<Result<ProductoDto>> ObtenerPorIdAsync(int id)
    {
        try
        {
            var producto = await _unitOfWork.Repository<Producto>().GetByIdAsync(id) ?? throw new Exception($"Producto con ID {id} no encontrado");
            var productoDto = producto.Adapt<ProductoDto>();
            return Result<ProductoDto>.SuccessResult(productoDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el producto por ID");
            return Result<ProductoDto>.FailureResult("Error al obtener el producto", [ex.Message]);
        }
    }

    public async Task<Result<PagedResultDto<ProductoDto>>> ObtenerTodosAsync(int pageNumber = 1, int pageSize = 10)
    {
        try
        {

            // Validar parámetros
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var skip = (pageNumber - 1) * pageSize;

            // Obtener total de registros
            var totalCount = await _unitOfWork.Repository<Producto>().CountAsync();

            // Obtener productos paginados
            var productos = await _unitOfWork.Repository<Producto>()
                .GetPaginatedAsync(skip, pageSize);

            // Mapear a DTO
            var productosDto = productos.Adapt<List<ProductoDto>>();

            var result = new PagedResultDto<ProductoDto>
            {
                Items = productosDto,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return Result<PagedResultDto<ProductoDto>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener productos: {ex.Message}");
            return Result<PagedResultDto<ProductoDto>>.FailureResult(
                "Error al obtener productos", [ex.Message]);
        }
    }

    public async Task<Result<PagedResultDto<ProductoDto>>> ObtenerProductosPaginado(int pageNumber, int pageSize)
    {
        return await ObtenerTodosAsync(pageNumber, pageSize);
    }

    public async Task<Result<ProductoDto>> ActualizarAsync(ActualizarProductoDto dto)
    {
        try
        {
            // Obtener producto existente
            var producto = await _unitOfWork.Repository<Producto>().GetByIdAsync(dto.Id) ?? throw new Exception("Producto no encontrado");

            // Mapear cambios con Mapster
            dto.Adapt(producto);

            // Actualizar
            await _unitOfWork.Repository<Producto>().UpdateAsync(producto);
            await _unitOfWork.SaveChangesAsync();

            var productoDto = producto.Adapt<ProductoDto>();
            return Result<ProductoDto>.SuccessResult(productoDto, "Producto actualizado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al actualizar producto: {ex.Message}");
            return Result<ProductoDto>.FailureResult("Error al actualizar producto", new List<string> { ex.Message });
        }
    }
    public async Task<Result> EliminarAsync(int id)
    {
        try
        {
            var producto = await _unitOfWork.Repository<Producto>().GetByIdAsync(id) ?? throw new Exception("Producto no encontrado");

            // Verificar si el producto tiene detalles de pedidos asociados
            var detalles = await _unitOfWork.Repository<DetallePedido>()
                .FindAsync(d => d.ProductoId == id);

            if (detalles.Any())
            {
                throw new Exception(
                    "No se puede eliminar un producto que tiene pedidos asociados");
            }

            await _unitOfWork.Repository<Producto>().DeleteAsync(producto);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Producto {id} eliminado exitosamente");
            return Result.SuccessResult("Producto eliminado exitosamente");
        }

        catch (Exception ex)
        {
            _logger.LogError($"Error al eliminar producto: {ex.Message}");
            return Result.FailureResult("Error al eliminar producto", [ex.Message]);
        }
    }

    public async Task<Result<List<ProductoDto>>> BuscarPorNombreAsync(string nombre)
    {
        return await _unitOfWork.Repository<Producto>()
            .FindAsync(p => p.Nombre.ToLower().Contains(nombre.ToLower()))
            .ContinueWith(task =>
            {
                var productos = task.Result;
                if (productos.Any())
                {
                    var productosDto = productos.Adapt<List<ProductoDto>>();
                    return Result<List<ProductoDto>>.SuccessResult(productosDto, "Productos encontrados");
                }
                else
                {
                    return Result<List<ProductoDto>>.FailureResult("No se encontraron productos con ese nombre");
                }
            });
    }

    public async Task<Result<bool>> ExisteProductoAsync(string nombre)
    {
        return await _unitOfWork.Repository<Producto>()
            .ExistsAsync(p => p.Nombre.ToLower() == nombre.ToLower())
            ? Result<bool>.SuccessResult(true, "El producto existe")
            : Result<bool>.SuccessResult(false, "El producto no existe");
    }

    public async Task<Result<bool>> VerificarStock(int productoId, int cantidad)
    {
        try
        {
            if (cantidad <= 0)
            {
                throw new Exception("La cantidad debe ser mayor a 0");
            }

            var producto = await _unitOfWork.Repository<Producto>().GetByIdAsync(productoId);

            if (producto == null)
            {
                throw new Exception("Producto no encontrado");
            }

            var tieneStock = producto.Stock >= cantidad;

            if (!tieneStock)
            {
                _logger.LogWarning($"Stock insuficiente para producto {productoId}. Stock disponible: {producto.Stock}, solicitado: {cantidad}");
            }

            return Result<bool>.SuccessResult(tieneStock);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al verificar stock: {ex.Message}");
            return Result<bool>.FailureResult("Error al verificar stock", new List<string> { ex.Message });
        }
    }

    public async Task<Result<List<ProductoDto>>> BuscarProductos(string termino)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(termino))
            {
                throw new Exception("El término de búsqueda es requerido");
            }

            var terminoLower = termino.ToLower().Trim();

            var productos = await _unitOfWork.Repository<Producto>()
                .FindAsync(p => p.Nombre.ToLower().Contains(terminoLower) ||
                                  p.Descripcion.ToLower().Contains(terminoLower));

            var productosDto = productos.Adapt<List<ProductoDto>>();

            _logger.LogInformation($"Búsqueda realizada con término: {termino}. Se encontraron {productos.Count()} productos");
            return Result<List<ProductoDto>>.SuccessResult(productosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al buscar productos: {ex.Message}");
            return Result<List<ProductoDto>>.FailureResult(
                "Error al buscar productos", new List<string> { ex.Message });
        }
    }
}
