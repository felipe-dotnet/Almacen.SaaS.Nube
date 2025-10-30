using Almacen.Saas.Application.Common;
using Almacen.Saas.Application.DTOs.Notificacion;

namespace Almacen.Saas.Application.Services.Interfaces
{
    public interface INotificacionService
    {
        Task<Result<PagedResultDto<NotificacionDto>>> ObtenerTodasAsync(int page, int pageSize);
        Task<Result<PagedResultDto<NotificacionDto>>> ObtenerNotificacionesUsuario(int usuarioId, int pageNumber = 1, int pageSize = 10);
        Task<Result<NotificacionDto>> ObtenerNotificacion(int id);
        Task<Result<int>> CrearAsync(CrearNotificacionDto dto);
        Task<Result> MarcarComoLeidaAsync(int id);
        Task<Result> MarcarTodoComoLeidoAsync(int usuarioId);
        Task<Result> EliminarNotificacionAsync(int id);
        Task<Result> EnviarNotificacionEmailAsync(int id);
        Task<Result> EnviarNotificacionWhatsAppAsync(int id);
        Task<Result<int>> ObtenerConteoNoLegidosAsync(int usuarioId);
        Task<Result> LimpiarNotificacionesAntiguasAsync(int diasRetroactivos = 30);
        Task<Result<List<NotificacionDto>>> ObtenerNotificacionesPorTipoAsync(int usuarioId, int tipo);



    }
}
