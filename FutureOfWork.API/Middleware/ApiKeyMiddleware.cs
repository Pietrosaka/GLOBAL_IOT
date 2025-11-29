using Microsoft.Extensions.Configuration;

namespace FutureOfWork.API.Middleware;

/// <summary>
/// Middleware para autenticação via API Key
/// </summary>
public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private const string API_KEY_HEADER = "X-API-Key";

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Permitir acesso ao Swagger e health checks sem API Key
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
        if (path.StartsWith("/swagger") || 
            path.StartsWith("/health") || 
            path == "/" ||
            path.StartsWith("/favicon"))
        {
            await _next(context);
            return;
        }

        // Obter API Key da configuração
        var apiKey = _configuration["ApiSettings:ApiKey"] ?? "FutureOfWork-API-Key-2024";

        // Verificar API Key para endpoints de IA
        if (!context.Request.Headers.TryGetValue(API_KEY_HEADER, out var extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { error = "API Key não fornecida. Use o header X-API-Key" });
            return;
        }

        // Validar API Key
        if (!extractedApiKey.Equals(apiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { error = "API Key inválida" });
            return;
        }

        await _next(context);
    }
}

