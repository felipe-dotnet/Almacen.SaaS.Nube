namespace Almacen.Saas.Domain.Services;
public interface IPasswordRecoveryService
{
    Task SolicitarRecuperacionAsync(string email);
    Task<bool> CambiarPasswordAsync(string token, string nuevaPassword);
}
