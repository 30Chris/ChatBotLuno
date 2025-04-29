using Microsoft.AspNetCore.Mvc;
using ChatbotProyec;
using Microsoft.Extensions.Caching.Memory;

namespace ChatbotProyec.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly Chatbot _chatbot;

        public ChatbotController(Chatbot chatbot)
        {
            //_chatbot = new Chatbot();
            _chatbot = chatbot;
        }

        // POST api/chatbot
        [HttpPost]
        public IActionResult ObtenerRespuesta([FromBody] UsuarioInput input)
        {
            if (string.IsNullOrEmpty(input.Mensaje))
            {
                return BadRequest("El mensaje no puede estar vacío.");
            }

            var respuesta = _chatbot.ObtenerRespuesta(input.Mensaje);
            return Ok(new { respuesta });
        }
    }

    public class UsuarioInput
    {
        public required string Mensaje { get; set; }
    }
}
