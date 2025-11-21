using Almacen.Saas.Domain.Entities;
using Almacen.Saas.Domain.Interfaces;
using Almacen.Saas.Domain.Services;
using Microsoft.Extensions.Configuration;

namespace Almacen.Saas.Application.Services.Utilities
{
    public class PasswordRecoveryService : IPasswordRecoveryService
    {
        private readonly IUnitOfWork _unit;
        private readonly IEmailService _emailService;
        private readonly IPasswordHasher _hashService;
        private readonly IConfiguration _config;

        public PasswordRecoveryService(IUnitOfWork unitOfWork,IEmailService emailService, IPasswordHasher passwordHasher, IConfiguration config) 
        {
            _unit = unitOfWork;
            _emailService = emailService;
            _hashService = passwordHasher;
            _config = config;
        }

        public async Task SolicitarRecuperacionAsync(string email)
        {
            var userRepo = _unit.Repository<Usuario>();
            var tokenRepo = _unit.Repository<PasswordResetToken>();

            var user = await userRepo.GetAsync(u => u.Email == email);
            if (user == null)
            {
                // Opcional: No revelar que el correo no existe
                return;
            }
            var token = Guid.NewGuid().ToString();

            var resetToken = new PasswordResetToken
            {
                UsuarioId = user.Id,
                Token = token,
                ExpiraEn = DateTime.UtcNow.AddHours(1) // Token válido por 1 hora
            };

            await tokenRepo.AddAsync(resetToken);
            await _unit.SaveChangesAsync();

            var resetLinkTemplate = _config["PasswordReset:ResetLinkBase"]??"http://localhost/";
            var resetLink = string.Format(resetLinkTemplate, token, email);

            var msg=new EmailRequest
            {
                Destinatario=email,
                Asunto="Recuperación de Contraseña",
                Cuerpo=$"Haz clic en el siguiente enlace para restablecer tu contraseña</br>: {resetLink}",
                EsHtml=true
            };
            
            await _emailService.EnviarEmailAsync(msg);

        }

        public async Task<bool> CambiarPasswordAsync(string token, string nuevaPassword)
        {
            var tokenRepo = _unit.Repository<PasswordResetToken>();
            var userRepo = _unit.Repository<Usuario>();

            var resetToken = await tokenRepo.GetAsync(t => t.Token == token && t.ExpiraEn > DateTime.UtcNow);
            if (resetToken == null) return false;

            var user = await userRepo.GetByIdAsync(resetToken.UsuarioId);
            
            if(user == null) return false;
            
            user.PasswordHash = _hashService.HashPassword(nuevaPassword);

            await userRepo.UpdateAsync(user);
            await tokenRepo.DeleteAsync(resetToken);
            await _unit.SaveChangesAsync();

            return true;
        }
    }
}
