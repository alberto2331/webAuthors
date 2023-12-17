using WebAuthorApi;

namespace WebApiAuthors.Middlewares
{
    public class LogHTTPResponse
    {
        private readonly RequestDelegate siguiente;
        private readonly ILogger<LogHTTPResponse> iLogger;

        public LogHTTPResponse(RequestDelegate siguiente, ILogger<LogHTTPResponse> iLogger)
        {
            this.siguiente = siguiente;
            this.iLogger = iLogger;
        }

        public async Task InvokeAsync(HttpContext contexto)
        {
            using (var ms = new MemoryStream())
            {
                var cuerpoOriginalRespuesta = contexto.Response.Body;
                contexto.Response.Body = ms;
                await siguiente(contexto);
                ms.Seek(0, SeekOrigin.Begin);
                string respuesta = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);
                await ms.CopyToAsync(cuerpoOriginalRespuesta);
                contexto.Response.Body = cuerpoOriginalRespuesta;
                iLogger.LogInformation(respuesta);
            }
        }
    }
}
