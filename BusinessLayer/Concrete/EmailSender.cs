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

namespace BusinessLayer.Concrete
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly IConfiguration _configuration;

        public EmailSender(IOptions<EmailSettings> emailSettings, IConfiguration configuration)
        {
            _emailSettings = emailSettings.Value;
            _configuration = configuration;
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
            }
            catch (Exception ex)
            {
                // Manejar errores
                throw new InvalidOperationException("Error al enviar el correo electrónico", ex);
            }
            finally
            {
                 client.Disconnect(true);
            }
        }
    }
}
