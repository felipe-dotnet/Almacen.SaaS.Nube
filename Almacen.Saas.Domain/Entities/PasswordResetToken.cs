using Almacen.Saas.Domain.Common;

namespace Almacen.Saas.Domain.Entities;
public class PasswordResetToken:BaseEntity
{
    public int UsuarioId { get; set; }
    public string Token { get; set; }= string.Empty;
    public DateTime ExpiraEn { get; set; }
    public virtual Usuario Usuario { get; set; }= null!;
}
