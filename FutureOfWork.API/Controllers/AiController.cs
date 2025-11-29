using FutureOfWork.AI;
using FutureOfWork.AI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FutureOfWork.API.Controllers;

/// <summary>
/// Controller para endpoints de IA: OCR, classificação e matching
/// </summary>
[ApiController]
[Route("api/v1/ai")]
[Produces("application/json")]
public class AiController : ControllerBase
{
    private readonly IAiService _aiService;
    private readonly ILogger<AiController> _logger;

    public AiController(IAiService aiService, ILogger<AiController> logger)
    {
        _aiService = aiService;
        _logger = logger;
    }

    /// <summary>
    /// Extrai texto e informações estruturadas de um currículo (PDF, PNG, JPEG)
    /// </summary>
    /// <param name="file">Arquivo do currículo</param>
    /// <returns>Resultado da extração OCR com texto e campos estruturados</returns>
    /// <response code="200">Extração realizada com sucesso</response>
    /// <response code="400">Arquivo inválido ou formato não suportado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost("ocr-resume")]
    [ProducesResponseType(typeof(OcrResumeResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OcrResumeResult>> OcrResume(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { error = "Arquivo não fornecido ou vazio" });
        }

        var allowedExtensions = new[] { ".pdf", ".png", ".jpg", ".jpeg", ".tiff", ".bmp" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(fileExtension))
        {
            return BadRequest(new { error = $"Formato não suportado: {fileExtension}" });
        }

        try
        {
            using var stream = file.OpenReadStream();
            var result = await _aiService.ExtractResumeAsync(stream, file.FileName);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar OCR do arquivo: {FileName}", file.FileName);
            return StatusCode(500, new { error = "Erro ao processar arquivo", details = ex.Message });
        }
    }

    /// <summary>
    /// Detecta objetos em uma imagem de portfólio (logos, certificados, etc.)
    /// </summary>
    /// <param name="file">Imagem do portfólio</param>
    /// <returns>Lista de objetos detectados com bounding boxes e scores</returns>
    /// <response code="200">Detecção realizada com sucesso</response>
    /// <response code="400">Arquivo inválido</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost("classify-portfolio")]
    [ProducesResponseType(typeof(ClassifyPortfolioResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ClassifyPortfolioResult>> ClassifyPortfolio(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { error = "Arquivo não fornecido ou vazio" });
        }

        var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".tiff", ".bmp" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(fileExtension))
        {
            return BadRequest(new { error = $"Formato não suportado: {fileExtension}. Use imagens." });
        }

        try
        {
            using var stream = file.OpenReadStream();
            var result = await _aiService.DetectObjectsAsync(stream, file.FileName);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao classificar portfólio: {FileName}", file.FileName);
            return StatusCode(500, new { error = "Erro ao processar imagem", details = ex.Message });
        }
    }

    /// <summary>
    /// Calcula o score de compatibilidade entre um currículo e uma descrição de vaga
    /// </summary>
    /// <param name="request">Requisição com texto do currículo e descrição da vaga</param>
    /// <returns>Score de compatibilidade, features relevantes e sugestões</returns>
    /// <response code="200">Análise realizada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost("match-summary")]
    [ProducesResponseType(typeof(MatchSummaryResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MatchSummaryResult>> MatchSummary([FromBody] MatchSummaryRequest request)
    {
        if (request == null)
        {
            return BadRequest(new { error = "Requisição inválida" });
        }

        if (string.IsNullOrWhiteSpace(request.ResumeText))
        {
            return BadRequest(new { error = "Texto do currículo é obrigatório" });
        }

        if (string.IsNullOrWhiteSpace(request.JobDescription))
        {
            return BadRequest(new { error = "Descrição da vaga é obrigatória" });
        }

        try
        {
            var result = await _aiService.PredictMatchAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao calcular matching");
            return StatusCode(500, new { error = "Erro ao processar matching", details = ex.Message });
        }
    }

    /// <summary>
    /// Retorna informações sobre os modelos de IA disponíveis
    /// </summary>
    /// <returns>Lista de modelos com versões e métricas</returns>
    /// <response code="200">Modelos listados com sucesso</response>
    [HttpGet("models")]
    [ProducesResponseType(typeof(List<ModelInfo>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ModelInfo>>> GetModels()
    {
        try
        {
            var models = await _aiService.GetAvailableModelsAsync();
            return Ok(models);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar modelos");
            return StatusCode(500, new { error = "Erro ao listar modelos", details = ex.Message });
        }
    }
}

