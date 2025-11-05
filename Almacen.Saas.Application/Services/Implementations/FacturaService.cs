using Almacen.Saas.Application.Common;
using Almacen.Saas.Application.DTOs.Factura;
using Almacen.Saas.Application.Services.Interfaces;
using Almacen.Saas.Domain.Entities;
using Almacen.Saas.Domain.Enums;
using Almacen.Saas.Domain.Interfaces;
using Mapster;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens.Experimental;

namespace Almacen.Saas.Application.Services.Implementations;
public class FacturaService: IFacturaService
{
	#region Constructor
	private readonly IUnitOfWork _unitOfWork;
	private readonly ILogger<FacturaService> _logger;

    public FacturaService(IUnitOfWork unitOfWork, ILogger<FacturaService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    #endregion

    public async Task<Result<FacturaDto>> CrearAsync(CrearFacturaDto dto)
    {
        try
        {
            // 1. Validar que el pedido existe
            var pedido = await _unitOfWork.Repository<Pedido>().GetByIdAsync(dto.PedidoId) ?? throw new Exception("Pedido no encontrado");

            // 2. Validar que el pedido no esté cancelado
            if (pedido.Estado == Domain.Enums.EstadoPedido.Cancelado)
            {
                throw new Exception("No se puede crear factura de un pedido cancelado");
            }

            // 3. Verificar que la factura no exista ya
            var facturaExistente = await _unitOfWork.Repository<Factura>()
                .GetAsync(f => f.PedidoId == dto.PedidoId);

            if (facturaExistente != null)
            {
                throw new Exception("Ya existe una factura para este pedido");
            }

            // 4. Obtener usuario para datos de facturación
            var usuario = await _unitOfWork.Repository<Usuario>().GetByIdAsync(pedido.UsuarioId);

            if (usuario == null)
            {
                throw new Exception("Usuario no encontrado");
            }

            // 5. Crear factura
            var factura = new Factura
            {
                FolioFiscal= Guid.NewGuid().ToString(),
                PedidoId = pedido.Id,
                FechaEmision = DateTime.UtcNow,
                Email = usuario.Email??string.Empty,
                Subtotal = pedido.Total,
                IVA = CalcularIVA(pedido.Total),
                Total = CalcularTotalFactura(pedido.Total, 0),
                XmlUrl = string.Empty,
                PdfUrl = string.Empty,
                CreadoPor = usuario.Nombre,
                Pedido = pedido
            };

            await _unitOfWork.Repository<Factura>().AddAsync(factura);
            await _unitOfWork.SaveChangesAsync();

            // 6. Crear notificación
            var notificacion = new Notificacion
            {
                UsuarioId = pedido.UsuarioId,
                Tipo = TipoNotificacion.FacturaCreada,
                Mensaje = $"Se ha generado la factura #{factura.Id.ToString()} para tu pedido #{pedido.Id}",
                FechaCreacion = DateTime.UtcNow,
                Leida = false
            };

            await _unitOfWork.Repository<Notificacion>().AddAsync(notificacion);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Factura #{factura.Id} creada para pedido #{dto.PedidoId}");

            var facturaDto = factura.Adapt<FacturaDto>();
            return Result<FacturaDto>.SuccessResult(facturaDto, "Factura creada exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al crear factura: {ex.Message}");
            return Result<FacturaDto>.FailureResult("Error al crear factura", [ex.Message]);
        }
    }

    public async Task<Result<FacturaDto>> ObtenerPorIdAsync(int id)
    {
        try
        {
            var factura = await _unitOfWork.Repository<Factura>().GetByIdAsync(id) ?? throw new Exception($"Factura con ID {id} no encontrada");
            var facturaDto = factura.Adapt<FacturaDto>();
            return Result<FacturaDto>.SuccessResult(facturaDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener factura: {ex.Message}");
            return Result<FacturaDto>.FailureResult("Error al obtener factura", new List<string> { ex.Message });
        }
    }

    public async Task<Result<PagedResultDto<FacturaDto>>> ObtenerTodasAsync(int pageNumber=1, int pageSize=10)
    {
        try
        {
            // Validar parámetros
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var skip = (pageNumber - 1) * pageSize;

            // Obtener total de registros
            var totalCount = await _unitOfWork.Repository<Factura>().CountAsync();

            // Obtener facturas paginadas
            var facturas = await _unitOfWork.Repository<Factura>()
                .GetPaginatedAsync(skip, pageSize);

            // Mapear a DTO
            var facturasDto = facturas.Adapt<List<FacturaDto>>();

            var result = new PagedResultDto<FacturaDto>
            {
                Items = facturasDto,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return Result<PagedResultDto<FacturaDto>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener facturas: {ex.Message}");
            return Result<PagedResultDto<FacturaDto>>.FailureResult(
                "Error al obtener facturas", [ex.Message]);
        }
    }

    public async Task<Result<PagedResultDto<FacturaDto>>> ObtenerFacturasPaginado(int pageNumber, int pageSize)
    {
        return await ObtenerTodasAsync(pageNumber, pageSize);
    }

    public async Task<Result<List<FacturaDto>>> ObtenerFacturasPorPedido(int pedidoId)
    {
        try
        {
            // Verificar que el pedido existe
            var pedido = await _unitOfWork.Repository<Pedido>().GetByIdAsync(pedidoId);

            if (pedido == null)
            {
                throw new Exception("Pedido no encontrado");
            }

            var facturas = await _unitOfWork.Repository<Factura>()
                .FindAsync(f => f.PedidoId == pedidoId);

            var facturasDto = facturas.Adapt<List<FacturaDto>>();

            _logger.LogInformation($"Se obtuvieron {facturas.Count()} facturas para pedido {pedidoId}");
            return Result<List<FacturaDto>>.SuccessResult(facturasDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener facturas por pedido: {ex.Message}");
            return Result<List<FacturaDto>>.FailureResult(
                "Error al obtener facturas", [ex.Message]);
        }
    }

    public async Task<Result<byte[]>> GenerarPDFFactura(int id)
    {
        try
        {
            var factura = await _unitOfWork.Repository<Factura>().GetByIdAsync(id) ?? throw new Exception("Factura no encontrada");

            // TODO: Implementar generación de PDF
            // Puedes usar iTextSharp, SelectPdf, QuestPDF, etc.
            // Por ahora retornamos un array vacío como placeholder
            var pdfBytes = Array.Empty<byte>();

            _logger.LogInformation($"PDF generado para factura #{factura.Id}");
            return Result<byte[]>.SuccessResult(pdfBytes, "PDF generado exitosamente");
        }      
        catch (Exception ex)
        {
            _logger.LogError($"Error al generar PDF: {ex.Message}");
            return Result<byte[]>.FailureResult("Error al generar PDF", [ex.Message]);
        }
    }
    public async Task<Result> AnularFactura(int id)
    {
        try
        {
            var factura = await _unitOfWork.Repository<Factura>().GetByIdAsync(id) ?? throw new Exception("Factura no encontrada");

            // Validar estado
            if (factura.Activo == false)
            {
                throw new Exception("La factura ya está anulada");
            }

            if (factura.Activo == true)
            {
                throw new Exception("No se puede anular una factura pagada");
            }

            factura.Activo = false;
            factura.FechaModificacion = DateTime.UtcNow;

            await _unitOfWork.Repository<Factura>().UpdateAsync(factura);
            await _unitOfWork.SaveChangesAsync();

            // Crear notificación
            var pedido = await _unitOfWork.Repository<Pedido>().GetByIdAsync(factura.PedidoId);

            if (pedido != null)
            {
                var notificacion = new Notificacion
                {
                    UsuarioId = pedido.UsuarioId,
                    Tipo = TipoNotificacion.FacturaCancelada,
                    Mensaje = $"La factura #{factura.Id} ha sido cancelada",
                    FechaCreacion = DateTime.UtcNow,
                    Leida = false
                };

                await _unitOfWork.Repository<Notificacion>().AddAsync(notificacion);
                await _unitOfWork.SaveChangesAsync();
            }

            _logger.LogInformation($"Factura #{factura.Id} anulada");
            return Result.SuccessResult("Factura anulada exitosamente");
        }       
        catch (Exception ex)
        {
            _logger.LogError($"Error al anular factura: {ex.Message}");
            return Result.FailureResult("Error al anular factura", new List<string> { ex.Message });
        }
    }
    public async Task<Result> EnviarFacturaPorEmail(int id)
    {
        try
        {
            var factura = await _unitOfWork.Repository<Factura>().GetByIdAsync(id) ?? throw new Exception("Factura no encontrada");

            // TODO: Implementar envío de email
            // Usar SendGrid, Gmail SMTP, o cualquier servicio de email
            // Por ahora solo registramos la acción

            factura.FechaModificacion = DateTime.UtcNow;
            await _unitOfWork.Repository<Factura>().UpdateAsync(factura);
            await _unitOfWork.SaveChangesAsync();

            // Crear notificación
            var pedido = await _unitOfWork.Repository<Pedido>().GetByIdAsync(factura.PedidoId);

            if (pedido != null)
            {
                var notificacion = new Notificacion
                {
                    UsuarioId = pedido.UsuarioId,
                    Tipo = TipoNotificacion.FacturaEnviada,
                    Mensaje = $"Tu factura #{factura.Id.ToString()} ha sido enviada a {factura.Email}",
                    FechaCreacion = DateTime.UtcNow,
                    Leida = false
                };

                await _unitOfWork.Repository<Notificacion>().AddAsync(notificacion);
                await _unitOfWork.SaveChangesAsync();
            }

            _logger.LogInformation($"Factura #{factura.Id} enviada a {factura.Email}");
            return Result.SuccessResult("Factura enviada exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al enviar factura: {ex.Message}");
            return Result.FailureResult("Error al enviar factura", [ex.Message]);
        }
    }

    private static decimal CalcularTotalFactura(decimal total, decimal value)
    {
        decimal iva = CalcularIVA(total);
        var totalCalculo = (value + iva) - 0;
        return totalCalculo < 0 ? 0 : total;
    }

    private static decimal CalcularIVA(decimal total)
    {
        const decimal ivaPorcentaje = 0.16m; // 16%
        return total * ivaPorcentaje;
    }

    private static string GenerarNumeroFactura()
    {
        var fecha = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = new Random().Next(10000, 99999);
        return $"FAC-{fecha}-{random}";
    }
}
