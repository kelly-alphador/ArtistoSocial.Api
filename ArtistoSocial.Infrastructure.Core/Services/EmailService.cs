using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ArtistoSocial.Domaine.Core.DTO.Email;
using ArtistoSocial.Domaine.Core.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ArtistoSocial.Infrastructure.Core.Services
{
    public class EmailService:IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtpClient = new SmtpClient(_emailSettings.SmtpServer)
                {
                    Port = _emailSettings.SmtpPort,
                    Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.SenderPassword),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email envoyé avec succès à {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de l'envoi de l'email à {toEmail}");
                throw;
            }
        }

        public async Task SendEmailConfirmationAsync(string toEmail, string confirmationLink)
        {
            var subject = "Confirmez votre adresse email";
            var body = $@"
            <html>
            <body>
                <h2>Bienvenue!</h2>
                <p>Merci de vous être inscrit. Veuillez cliquer sur le lien ci-dessous pour confirmer votre adresse email:</p>
                <p><a href='{confirmationLink}' style='background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Confirmer mon email</a></p>
                <p>Si vous n'arrivez pas à cliquer sur le bouton, copiez et collez ce lien dans votre navigateur:</p>
                <p>{confirmationLink}</p>
                <p>Ce lien expire dans 24 heures.</p>
            </body>
            </html>";

            await SendEmailAsync(toEmail, subject, body);
        }
    }
}
