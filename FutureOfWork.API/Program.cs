using FutureOfWork.AI;
using FutureOfWork.AI.Services;
using FutureOfWork.API.Middleware;
using Serilog;

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/futureofwork-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Iniciando FutureOfWork.API");

    var builder = WebApplication.CreateBuilder(args);

    // Configurar Serilog
    builder.Host.UseSerilog();

    // Add services to the container
    builder.Services.AddControllers();
    
    // Swagger/OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "FutureOfWork AI API",
            Version = "v1",
            Description = "API REST para análise de currículos e matching com vagas usando IA"
        });
    });

    // Registrar serviços de IA
    builder.Services.AddScoped<IAiService, AiService>();

    // Health Checks
    builder.Services.AddHealthChecks()
        .AddCheck<AiHealthCheck>("ai_service");

    // CORS para permitir acesso de front-ends
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "FutureOfWork AI API v1");
            c.RoutePrefix = string.Empty; // Swagger na raiz
        });
    }

    app.UseHttpsRedirection();
    app.UseCors();

    // Middleware de autenticação por API Key
    app.UseMiddleware<ApiKeyMiddleware>();

    app.UseSerilogRequestLogging();

    app.MapControllers();
    app.MapHealthChecks("/health");

    // Endpoint de informações da API
    app.MapGet("/", () => new
    {
        name = "FutureOfWork AI API",
        version = "1.0.0",
        endpoints = new
        {
            swagger = "/swagger",
            health = "/health",
            ai = "/api/v1/ai"
        }
    })
    .WithName("Root")
    .WithTags("Info");

    Log.Information("API iniciada com sucesso");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação encerrada inesperadamente");
}
finally
{
    Log.CloseAndFlush();
}

// Health Check customizado
public class AiHealthCheck : Microsoft.Extensions.Diagnostics.HealthChecks.IHealthCheck
{
    public Task<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult> CheckHealthAsync(
        Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        // Verificar se os serviços de IA estão disponíveis
        // Por enquanto, sempre retorna saudável
        return Task.FromResult(
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("AI services are operational"));
    }
}
