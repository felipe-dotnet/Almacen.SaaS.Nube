using Almacen.Saas.Domain.Common;

namespace Almacen.Saas.Domain.Entities;
public class RefreshToken:BaseEntity
{
    public string Token { get; set; }=string.Empty;
    public string JwtId { get; set; }=string.Empty;
    public int UsuarioId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public Usuario Usuario { get; set; } = new();
}
