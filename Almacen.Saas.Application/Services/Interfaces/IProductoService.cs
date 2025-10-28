using Almacen.Saas.Application.Common;
using Almacen.Saas.Application.DTOs.Producto;

namespace Almacen.Saas.Application.Services.Interfaces;
public interface IProductoService
{
    Task<Result<PagedResultDto<ProductoDto>>> ObtenerTodosAsync(int pageNumber, int pageSize);
    Task<Result<ProductoDto>> ObtenerPorIdAsync(int id);
    Task<Result<ProductoDto>> CrearAsync(CrearProductoDto dto);
    Task<Result<ProductoDto>> ActualizarAsync(ActualizarProductoDto dto);
    Task<Result> EliminarAsync(int id);
    Task<Result<List<ProductoDto>>> BuscarPorNombreAsync(string nombre);
    Task<Result<bool>> ExisteProductoAsync(string nombre);
    Task<Result<PagedResultDto<ProductoDto>>> ObtenerProductosPaginado(int pageNumber, int pageSize);
    Task<Result<bool>> VerificarStock(int productoId, int cantidad);
    Task<Result<List<ProductoDto>>> BuscarProductos(string termino);
}
