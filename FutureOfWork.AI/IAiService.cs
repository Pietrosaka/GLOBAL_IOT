using FutureOfWork.AI.Models;

namespace FutureOfWork.AI;

/// <summary>
/// Interface para serviços de IA: OCR, classificação e matching
/// </summary>
public interface IAiService
{
    /// <summary>
    /// Extrai texto e informações estruturadas de um currículo (PDF, PNG, JPEG)
    /// </summary>
    Task<OcrResumeResult> ExtractResumeAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Detecta objetos em uma imagem de portfólio (logos, certificados, etc.)
    /// </summary>
    Task<ClassifyPortfolioResult> DetectObjectsAsync(Stream imageStream, string fileName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calcula o score de compatibilidade entre um currículo e uma descrição de vaga
    /// </summary>
    Task<MatchSummaryResult> PredictMatchAsync(MatchSummaryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retorna informações sobre os modelos disponíveis
    /// </summary>
    Task<List<ModelInfo>> GetAvailableModelsAsync(CancellationToken cancellationToken = default);
}

