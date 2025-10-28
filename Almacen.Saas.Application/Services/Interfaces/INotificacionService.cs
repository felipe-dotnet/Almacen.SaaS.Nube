using Almacen.Saas.Application.Common;
using Almacen.Saas.Application.DTOs.Notificacion;

namespace Almacen.Saas.Application.Services.Interfaces
{
    public interface INotificacionService
    {
        Task<Result<PagedResultDto<NotificacionDto>>> ObtenerTodasAsync(int page, int pageSize);
        Task<Result<int>> CrearAsync(CrearNotificacionDto dto);
        Task<Result> MarcarComoLeidaAsync(int id);
    }
}
