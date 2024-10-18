using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models;

namespace PresentationLayer.Controllers
{
    public class CorreosController : Controller
    {
        private readonly IEmailSender _emailSender;

        public CorreosController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }
        // Endpoint para enviar correos por una categoría específica
        [HttpPost("enviar-por-categoria/{categoria}")]
        public async Task<IActionResult> EnviarCorreo(List<string> _toEmails, string _category, Dictionary<string, string> dynamicValues)
        {
            try
            {
                _emailSender.SendEmail(_toEmails, _category, dynamicValues);
                return Ok("Correo enviado.");
                return Ok(new { mensaje = "Correos enviados exitosamente" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
