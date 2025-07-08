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
using System.Runtime.Intrinsics.X86;

namespace ArtistoSocial.Infrastructure.Core.Services
{
    public class EmailService:IEmailService
    {
        //Pour acceder aux parametre SMTP(serveur, port, email, mot de passe, nom d'expediteur) de l'application
        private readonly EmailSettings _emailSettings;
        //Pour enregistrer les logs(les informations, les erreurs, etc.)
        private readonly ILogger<EmailService> _logger;
        //IOPtions<EmailSettings> emailSettings recoit la configuration injectee automatiquement dans appSettings
        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            //emailSettings.value contient le vrai valeur de la configuration SMTP
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                //ici on creer un client SMTP pour qui se connecte aux serveur SMTP
                //notre client SMTP va preparer notre email et envoyer notre email aux serveur SMTP qui en voye notre email
                var smtpClient = new SmtpClient(_emailSettings.SmtpServer)
                {
                    Port = _emailSettings.SmtpPort,
                    Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.SenderPassword),
                    //active la connexion sécurisée(TLS / SSL)
                    EnableSsl = true,
                };
                //creation du message Email
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };
                //on ajoute l'email de destinataire
                mailMessage.To.Add(toEmail);
                //Envoye une email SendMailAsync( existe deja dans SmtpClient)
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
            // $@"" = insérer des variables + écrire du texte multilignes facilement.
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
