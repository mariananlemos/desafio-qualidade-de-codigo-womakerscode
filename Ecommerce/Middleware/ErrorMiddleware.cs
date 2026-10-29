using System.Net;
using System.Text.Json;

namespace Ecommerce.Middleware;

public class ErrorMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorMiddleware> _logger;

    public ErrorMiddleware(RequestDelegate next, ILogger<ErrorMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado: {Message}", ex.Message);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var resposta = new
            {
                mensagem = "Erro interno no servidor. Tente novamente mais tarde.",
                status = 500
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(resposta));
        }
    }
}