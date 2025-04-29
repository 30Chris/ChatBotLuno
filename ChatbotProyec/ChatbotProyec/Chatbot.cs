using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;




namespace ChatbotProyec
{
    public class Chatbot
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _env;
        private Dictionary<string, string> respuestas;
        private string nombreUsuario;
        private readonly string archivoChatbot;

      
        public Chatbot(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env)
        {
            _httpContextAccessor = httpContextAccessor;
            _env = env;


            // Obtener la ruta física del archivo dentro de wwwroot
            archivoChatbot = Path.Combine(_env.WebRootPath ?? "wwwroot", "Procesos", "Archivos", "chatbot_data.txt");

            respuestas = new Dictionary<string, string>();
            nombreUsuario = string.Empty;

        }

        // Método para procesar el mensaje del usuario
        public string ProcesarMensaje(string mensaje)
        {
            mensaje = mensaje.ToLower().Trim();

            // Verificar si la pregunta está en el diccionario
            if (respuestas.ContainsKey(mensaje))
            {
                return respuestas[mensaje];
            }
            else
            {
                return "No tengo una respuesta para eso. ¿Quieres agregar una respuesta? Envíame el formato: agregar|pregunta|respuesta";
            }
        }

        // Método para agregar nuevas preguntas y respuestas al archivo
        public string AgregarPreguntaRespuesta(string pregunta, string respuesta)
        {
            pregunta = pregunta.ToLower().Trim();

            if (!respuestas.ContainsKey(pregunta))
            {
                respuestas[pregunta] = respuesta;
                GuardarPreguntaEnArchivo(pregunta, respuesta);
                return "¡Pregunta y respuesta guardadas correctamente!";
            }
            else
            {
                return "Esa pregunta ya está registrada.";
            }
        }

        public string ObtenerRespuesta(string entrada)
        {

            string[] parte = entrada.Split('|');

            if (parte.Length == 3 && parte[0].ToLower() == "agregar")
            {
                string pregunta = parte[1].Trim();
                string respuesta = parte[2].Trim();

                GuardarPreguntaEnArchivo(pregunta, respuesta);
                return "¡Pregunta y respuesta guardadas correctamente!";
            }
            else
            {
                CargarRespuestasDesdeArchivo();
            }

            string mensaje = entrada.ToLower();

            if (mensaje.Contains("me llamo"))
            {
                var partes = mensaje.Split(new[] { "me llamo" }, StringSplitOptions.None);
                nombreUsuario = partes[1].Trim();

                if (_httpContextAccessor.HttpContext?.Session != null)
                {
                    _httpContextAccessor.HttpContext.Session.SetString("nombreUsuario", nombreUsuario);
                }


                return $"¡Mucho gusto, {nombreUsuario}! Ahora que te conozco, ¿en qué puedo ayudarte?";
            }

            if (mensaje.Contains("recuerdas mi nombre?"))
            {
                if (_httpContextAccessor.HttpContext?.Session != null)
                {
                   
                    if(nombreUsuario != "")
                    {
                        return $"Te llamas {nombreUsuario}.";

                    }
                    else
                    {
                        return "Aún no me has dicho tu nombre. ¿Cómo te llamas?";
                    }

                }
               

            }

            if (Regex.IsMatch(mensaje, @"\b(qué|hora|hora es)\b"))
            {
                return "La hora actual es: " + DateTime.Now.ToString("HH:mm");
            }

            if (Regex.IsMatch(mensaje, @"\b(qué|fecha|hoy)\b"))
            {
                return "Hoy es: " + DateTime.Now.ToString("dd/MM/yyyy");
            }

            if (Regex.IsMatch(mensaje, @"^\d+\s*[\+\-\*/]\s*\d+$"))
            {
                try
                {
                    var result = new System.Data.DataTable().Compute(mensaje, null);
                    return "El resultado es: " + result.ToString();
                }
                catch
                {
                    return "Lo siento, no pude realizar esa operación.";
                }
            }

            if (respuestas.ContainsKey(mensaje))
            {
                return respuestas[mensaje];
            }
            else
            {
                return "No tengo una respuesta para eso. ¿Quieres agregar una respuesta? Envíame el formato: agregar|pregunta|respuesta";
            }
        }

        // Método para cargar las preguntas y respuestas desde un archivo de texto
        private void CargarRespuestasDesdeArchivo()
        {
            if (File.Exists(archivoChatbot))
            {
                foreach (var linea in File.ReadAllLines(archivoChatbot))
                {
                    var partes = linea.Split('|');
                    if (partes.Length == 2)
                    {
                        respuestas[partes[0].ToLower().Trim()] = partes[1].Trim();
                    }
                }
            }
            else
            {
                //// Si no existe, lo creamos con respuestas básicas
                string? directorio = Path.GetDirectoryName(archivoChatbot);
                if (!string.IsNullOrEmpty(directorio))
                {
                    Directory.CreateDirectory(directorio);
                }

                File.WriteAllLines(archivoChatbot, new string[]
                {
                "hola|¡Hola! ¿Cómo puedo ayudarte hoy?",
                "adios|¡Hasta luego! Que tengas un buen día.",
                "como estas?|Estoy bien, gracias por preguntar. ¿Y tú?",
                "cual es tu nombre?|Soy Luno, creado para ayudarte.",
                "que puedes hacer?|Puedo responder preguntas simples por ahora."
                });

                CargarRespuestasDesdeArchivo();
            }
        }


        // Método para guardar una nueva pregunta y respuesta en el archivo de texto
        private void GuardarPreguntaEnArchivo(string pregunta, string respuesta)
        {
            using (StreamWriter sw = File.AppendText(archivoChatbot))
            {
                sw.WriteLine($"{pregunta}|{respuesta}");
            }
        }

    }
}
