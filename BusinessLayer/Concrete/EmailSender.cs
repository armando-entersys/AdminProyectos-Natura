using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BusinessLayer.Concrete
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<EmailSettings> emailSettings, IConfiguration configuration, ILogger<EmailSender> logger)
        {
            _emailSettings = emailSettings.Value;
            _configuration = configuration;
            _logger = logger;
        }
        private string ReemplazarMarcadores(string texto, Dictionary<string, string> placeholders)
        {
            foreach (var placeholder in placeholders)
            {
                texto = texto.Replace($"{{{placeholder.Key}}}", placeholder.Value);
            }
            return texto;
        }
        public void SendEmail(List<string> _toEmails, string _category, Dictionary<string, string> dynamicValues)
        {
            var emailMessage = new MimeMessage();

            // Lee el archivo HTML correspondiente a la plantilla
            string htmlTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", $"{_category}.html");
            string htmlContent = File.ReadAllText(htmlTemplatePath);




            // Leer destinatarios, asunto y cuerpo desde appsettings.json

            var asuntoTemplate = _configuration[$"CategoriasDeCorreo:{_category}:Asunto"];
            var cuerpoTemplate = htmlContent;
            // Reemplazar las variables dinámicas en el asunto y cuerpo
            foreach (var key in dynamicValues.Keys)
            {
                asuntoTemplate = asuntoTemplate.Replace($"{{{key}}}", dynamicValues[key]);
                cuerpoTemplate = cuerpoTemplate.Replace($"{{{key}}}", dynamicValues[key]);
            }

            emailMessage.Subject = asuntoTemplate;

            var bodyBuilder = new BodyBuilder { HtmlBody = cuerpoTemplate };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            foreach (var destinatario in _toEmails)
            {
                emailMessage.To.Add(new MailboxAddress(destinatario, destinatario));
            }
            // Asignar el campo From
            emailMessage.From.Add(new MailboxAddress("Natura", "ajcortest@gmail.com"));
            emailMessage.Sender = new MailboxAddress("Natura", "ajcortest@gmail.com");
           

            using var client = new MailKit.Net.Smtp.SmtpClient();
          

            try
            {
                 client.Connect(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                 client.Authenticate(_emailSettings.Username, _emailSettings.Password);



                client.Send(emailMessage);
                _logger.LogInformation($"Email enviado exitosamente a {string.Join(", ", _toEmails)} - Categoría: {_category}");
            }
            catch (Exception ex)
            {
                // Registrar error pero NO lanzar excepción para no bloquear la operación
                _logger.LogWarning(ex, $"No se pudo enviar email a {string.Join(", ", _toEmails)} - Categoría: {_category}. " +
                    $"Verifique la configuración SMTP en docker-compose.yml o appsettings.json. " +
                    $"Error: {ex.Message}");
                // NO lanzar excepción - permitir que la operación continúe sin email
            }
            finally
            {
                if (client.IsConnected)
                {
                    client.Disconnect(true);
                }
            }
        }
    }
}
