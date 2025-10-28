using Almacen.Saas.Application.Common;
using Almacen.Saas.Application.DTOs.Factura;

namespace Almacen.Saas.Application.Services.Interfaces;

public interface IFacturaService
{
    Task<Result<PagedResultDto<FacturaDto>>> ObtenerTodasAsync(int page, int pageSize);
    Task<Result<FacturaDto>> ObtenerPorIdAsync(int id);
    Task<Result<int>> CrearAsync(CrearFacturaDto dto);
    Task<Result> EliminarAsync(int id);
}
