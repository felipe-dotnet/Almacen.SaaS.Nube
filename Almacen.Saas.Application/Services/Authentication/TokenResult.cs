namespace Almacen.Saas.Application.Services.Authentication;
public class TokenResult
{
    public string AccessToken { get; set; }=string.Empty;
    public string Jti { get; set; }=string.Empty;
}
