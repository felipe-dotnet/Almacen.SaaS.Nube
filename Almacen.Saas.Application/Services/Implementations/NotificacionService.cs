using Almacen.Saas.Application.Common;
using Almacen.Saas.Application.DTOs.Notificacion;
using Almacen.Saas.Application.Services.Interfaces;
using Almacen.Saas.Domain.Entities;
using Almacen.Saas.Domain.Enums;
using Almacen.Saas.Domain.Interfaces;
using Mapster;
using Microsoft.Extensions.Logging;

namespace Almacen.Saas.Application.Services.Implementations;
public class NotificacionService : INotificacionService
{
    #region Constructor

    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<NotificacionService> _logger;

    public NotificacionService(IUnitOfWork unitOfWork, ILogger<NotificacionService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    #endregion

    public async Task<Result<int>> CrearAsync(CrearNotificacionDto dto)
    {
        try
        {
            // 1. Validar que el usuario existe
            var usuario = await _unitOfWork.Repository<Usuario>().GetByIdAsync(dto.UsuarioId) ?? throw new Exception("Usuario no encontrado");


            if (string.IsNullOrWhiteSpace(dto.Mensaje))
            {
                throw new Exception("El mensaje es requerido");
            }

            // 2. Crear notificación
            var notificacion = new Notificacion
            {
                UsuarioId = dto.UsuarioId,
                Tipo = (TipoNotificacion)dto.TipoNotificacion,
                Mensaje = dto.Mensaje,
                FechaCreacion = DateTime.UtcNow,
                Leida = false
            };

            await _unitOfWork.Repository<Notificacion>().AddAsync(notificacion);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Notificación creada para usuario {dto.UsuarioId}");

            var notificacionDto = notificacion.Adapt<NotificacionDto>();
            return Result<int>.SuccessResult(notificacionDto.Id, "Notificación creada exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al crear notificación: {ex.Message}");
            return Result<int>.FailureResult("Error al crear notificación", [ex.Message]);
        }
    }

    public async Task<Result<NotificacionDto>> ObtenerNotificacion(int id)
    {
        try
        {
            var notificacion = await _unitOfWork.Repository<Notificacion>().GetByIdAsync(id) ?? throw new Exception($"Notificación con ID {id} no encontrada");
            var notificacionDto = notificacion.Adapt<NotificacionDto>();
            return Result<NotificacionDto>.SuccessResult(notificacionDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener notificación: {ex.Message}");
            return Result<NotificacionDto>.FailureResult("Error al obtener notificación", [ex.Message]);
        }
    }

    public async Task<Result<PagedResultDto<NotificacionDto>>> ObtenerNotificacionesUsuario(int usuarioId, int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            // Validar que el usuario existe
            var usuario = await _unitOfWork.Repository<Usuario>().GetByIdAsync(usuarioId) ?? throw new Exception("Usuario no encontrado");

            // Validar parámetros
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var skip = (pageNumber - 1) * pageSize;

            // Obtener total de registros
            var totalCount = await _unitOfWork.Repository<Notificacion>()
                .CountAsync(n => n.UsuarioId == usuarioId);

            // Obtener notificaciones paginadas, ordenadas por fecha descendente
            var notificaciones = await _unitOfWork.Repository<Notificacion>()
                .FindAsync(n => n.UsuarioId == usuarioId);

            // Ordenar y paginar en memoria (en producción, mejor en BD)
            var notificacionesPaginadas = notificaciones
                .OrderByDescending(n => n.FechaCreacion)
                .Skip(skip)
                .Take(pageSize)
                .ToList();

            // Mapear a DTO
            var notificacionesDto = notificacionesPaginadas.Adapt<List<NotificacionDto>>();

            var result = new PagedResultDto<NotificacionDto>
            {
                Items = notificacionesDto,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return Result<PagedResultDto<NotificacionDto>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener notificaciones: {ex.Message}");
            return Result<PagedResultDto<NotificacionDto>>.FailureResult(
                "Error al obtener notificaciones", [ex.Message]);
        }
    }

    public async Task<Result<PagedResultDto<NotificacionDto>>> ObtenerTodasAsync(int pageNumber, int pageSize)
    {
        try
        {
            // Validar parámetros
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var skip = (pageNumber - 1) * pageSize;

            // Obtener total de registros
            var totalCount = await _unitOfWork.Repository<Notificacion>().CountAsync();

            // Obtener notificaciones paginadas
            var notificaciones = await _unitOfWork.Repository<Notificacion>()
                .GetPaginatedAsync(skip, pageSize);

            // Mapear a DTO
            var notificacionesDto = notificaciones.Adapt<List<NotificacionDto>>();

            var result = new PagedResultDto<NotificacionDto>
            {
                Items = notificacionesDto,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return Result<PagedResultDto<NotificacionDto>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener notificaciones: {ex.Message}");
            return Result<PagedResultDto<NotificacionDto>>.FailureResult(
                "Error al obtener notificaciones", [ex.Message]);
        }
    }

    public async Task<Result> MarcarComoLeidaAsync(int id)
    {
        try
        {
            var notificacion = await _unitOfWork.Repository<Notificacion>().GetByIdAsync(id);

            if (notificacion == null)
            {
                throw new Exception("Notificación no encontrada");
            }

            if (notificacion.Leida)
            {
                return Result.SuccessResult("La notificación ya estaba marcada como leída");
            }

            notificacion.Leida = true;
            notificacion.FechaLectura = DateTime.UtcNow;

            await _unitOfWork.Repository<Notificacion>().UpdateAsync(notificacion);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Notificación #{id} marcada como leída");
            return Result.SuccessResult("Notificación marcada como leída");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al marcar notificación: {ex.Message}");
            return Result.FailureResult("Error al marcar notificación", new List<string> { ex.Message });
        }
    }

    public async Task<Result> MarcarTodoComoLeidoAsync(int usuarioId)
    {
        try
        {
            // Validar que el usuario existe
            var usuario = await _unitOfWork.Repository<Usuario>().GetByIdAsync(usuarioId);

            if (usuario == null)
            {
                throw new Exception("Usuario no encontrado");
            }

            // Obtener todas las notificaciones no leídas del usuario
            var notificacionesNoLeidas = await _unitOfWork.Repository<Notificacion>()
                .FindAsync(n => n.UsuarioId == usuarioId && !n.Leida);

            if (!notificacionesNoLeidas.Any())
            {
                return Result.SuccessResult("No hay notificaciones sin leer");
            }

            // Marcar todas como leídas
            foreach (var notificacion in notificacionesNoLeidas)
            {
                notificacion.Leida = true;
                notificacion.FechaLectura = DateTime.UtcNow;
            }

            await _unitOfWork.Repository<Notificacion>().UpdateRangeAsync(notificacionesNoLeidas);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"{notificacionesNoLeidas.Count()} notificaciones marcadas como leídas para usuario {usuarioId}");
            return Result.SuccessResult($"{notificacionesNoLeidas.Count()} notificaciones marcadas como leídas");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al marcar todo como leído: {ex.Message}");
            return Result.FailureResult("Error al marcar notificaciones", [ex.Message]);
        }
    }

    public async Task<Result> EliminarNotificacionAsync(int id)
    {
        try
        {
            var notificacion = await _unitOfWork.Repository<Notificacion>().GetByIdAsync(id);

            if (notificacion == null)
            {
                throw new Exception("Notificación no encontrada");
            }

            await _unitOfWork.Repository<Notificacion>().DeleteAsync(notificacion);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Notificación #{id} eliminada");
            return Result.SuccessResult("Notificación eliminada exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al eliminar notificación: {ex.Message}");
            return Result.FailureResult("Error al eliminar notificación", [ex.Message]);
        }
    }

    public async Task<Result> EnviarNotificacionEmailAsync(int id)
    {
        try
        {
            var notificacion = await _unitOfWork.Repository<Notificacion>().GetByIdAsync(id) ?? throw new Exception("Notificación no encontrada");

            // Obtener usuario para obtener email
            var usuario = await _unitOfWork.Repository<Usuario>().GetByIdAsync(notificacion.UsuarioId) ?? throw new Exception("Usuario no encontrado");

            // TODO: Implementar envío de email usando SendGrid, SMTP, etc.
            // Ejemplo con SMTP:
            // using (var client = new SmtpClient("smtp.gmail.com", 587))
            // {
            //     client.Credentials = new NetworkCredential("tu-email@gmail.com", "tu-contraseña");
            //     client.EnableSsl = true;
            //     var mailMessage = new MailMessage("tu-email@gmail.com", usuario.Email)
            //     {
            //         Subject = notificacion.Tipo,
            //         Body = notificacion.Mensaje
            //     };
            //     await client.SendMailAsync(mailMessage);
            // }

            notificacion.FechaCreacion = DateTime.UtcNow;
            await _unitOfWork.Repository<Notificacion>().UpdateAsync(notificacion);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Notificación #{id} enviada por email a {usuario.Email}");
            return Result.SuccessResult("Notificación enviada por email exitosamente");
        }

        catch (Exception ex)
        {
            _logger.LogError($"Error al enviar notificación: {ex.Message}");
            return Result.FailureResult("Error al enviar notificación", new List<string> { ex.Message });
        }
    }

    public async Task<Result> EnviarNotificacionWhatsAppAsync(int id)
    {
        try
        {
            var notificacion = await _unitOfWork.Repository<Notificacion>().GetByIdAsync(id) ?? throw new Exception("Notificación no encontrada");

            // Obtener usuario
            var usuario = await _unitOfWork.Repository<Usuario>().GetByIdAsync(notificacion.UsuarioId) ?? throw new Exception("Usuario no encontrado");

            // TODO: Implementar envío de WhatsApp usando Twilio
            // Ejemplo con Twilio:
            // var accountSid = "tu-account-sid";
            // var authToken = "tu-auth-token";
            // TwilioClient.Init(accountSid, authToken);
            // var message = await MessageResource.CreateAsync(
            //     body: notificacion.Mensaje,
            //     from: new Twilio.Types.PhoneNumber("+1234567890"),
            //     to: new Twilio.Types.PhoneNumber(usuario.Telefono)
            // );

            notificacion.FechaModificacion = DateTime.UtcNow;
            await _unitOfWork.Repository<Notificacion>().UpdateAsync(notificacion);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Notificación #{id} enviada por WhatsApp a {usuario.Telefono}");
            return Result.SuccessResult("Notificación enviada por WhatsApp exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al enviar WhatsApp: {ex.Message}");
            return Result.FailureResult("Error al enviar WhatsApp", new List<string> { ex.Message });
        }
    }
    public async Task<Result<int>> ObtenerConteoNoLegidosAsync(int usuarioId)
    {
        try
        {
            // Validar que el usuario existe
            var usuario = await _unitOfWork.Repository<Usuario>().GetByIdAsync(usuarioId) ?? throw new Exception("Usuario no encontrado");

            // Contar notificaciones no leídas
            var conteo = await _unitOfWork.Repository<Notificacion>()
                .CountAsync(n => n.UsuarioId == usuarioId && !n.Leida);

            return Result<int>.SuccessResult(conteo);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener conteo: {ex.Message}");
            return Result<int>.FailureResult("Error al obtener conteo", new List<string> { ex.Message });
        }
    }

    public async Task<Result> LimpiarNotificacionesAntiguasAsync(int diasRetroactivos = 30)
    {
        try
        {
            if (diasRetroactivos <= 0)
            {
                throw new Exception("Los días deben ser mayor a 0");
            }

            var fechaLimite = DateTime.UtcNow.AddDays(-diasRetroactivos);

            var notificacionesAntiguas = await _unitOfWork.Repository<Notificacion>()
                .FindAsync(n => n.FechaCreacion < fechaLimite && n.Leida);

            if (!notificacionesAntiguas.Any())
            {
                return Result.SuccessResult("No hay notificaciones antiguas para eliminar");
            }

            await _unitOfWork.Repository<Notificacion>().DeleteRangeAsync(notificacionesAntiguas);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"{notificacionesAntiguas.Count()} notificaciones antiguas eliminadas");
            return Result.SuccessResult($"{notificacionesAntiguas.Count()} notificaciones eliminadas");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al limpiar notificaciones: {ex.Message}");
            return Result.FailureResult("Error al limpiar notificaciones", new List<string> { ex.Message });
        }
    }

    public async Task<Result<List<NotificacionDto>>> ObtenerNotificacionesPorTipoAsync(int usuarioId, int tipo)
    {
        try
        {

            var notificaciones = await _unitOfWork.Repository<Notificacion>()
                .FindAsync(n => n.UsuarioId == usuarioId && n.Tipo == (TipoNotificacion)tipo);

            var notificacionesDto = notificaciones.Adapt<List<NotificacionDto>>();

            return Result<List<NotificacionDto>>.SuccessResult(notificacionesDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener notificaciones: {ex.Message}");
            return Result<List<NotificacionDto>>.FailureResult(
                "Error al obtener notificaciones", [ex.Message]);
        }
    }

}
