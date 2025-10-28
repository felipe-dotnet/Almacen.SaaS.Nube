using Almacen.Saas.Application.Common;
using Almacen.Saas.Application.DTOs.Factura;

namespace Almacen.Saas.Application.Services.Interfaces;

public interface IFacturaService
{
    Task<Result<FacturaDto>> CrearAsync(CrearFacturaDto dto);
    Task<Result<PagedResultDto<FacturaDto>>> ObtenerTodasAsync(int page, int pageSize);
    Task<Result<PagedResultDto<FacturaDto>>> ObtenerFacturasPaginado(int pageNumber, int pageSize);
    Task<Result<List<FacturaDto>>> ObtenerFacturasPorPedido(int pedidoId);
    Task<Result<FacturaDto>> ObtenerPorIdAsync(int id);
    Task<Result<byte[]>> GenerarPDFFactura(int id);
    Task<Result> EnviarFacturaPorEmail(int id);
    // Task<Result> EliminarAsync(int id);
    Task<Result> AnularFactura(int id);
    //Task<Result<FacturaDto>> ActualizarFactura(int id, CrearFacturaDto dto);

}
