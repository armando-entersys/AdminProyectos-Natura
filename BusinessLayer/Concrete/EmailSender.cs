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
        public async Task SendEmailAsync(List<string> _toEmails,string _category, Dictionary<string, string> dynamicValues)
        {
            var emailMessage = new MimeMessage();
            // Leer destinatarios, asunto y cuerpo desde appsettings.json

            var asuntoTemplate = _configuration[$"CategoriasDeCorreo:{_category}:Asunto"];
            var cuerpoTemplate = _configuration[$"CategoriasDeCorreo:{_category}:Cuerpo"];
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


            

            using var client = new MailKit.Net.Smtp.SmtpClient();
          

            try
            {
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);

               
                
                await client.SendAsync(emailMessage);
            }
            catch (Exception ex)
            {
                // Manejar errores
                throw new InvalidOperationException("Error al enviar el correo electrónico", ex);
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}
