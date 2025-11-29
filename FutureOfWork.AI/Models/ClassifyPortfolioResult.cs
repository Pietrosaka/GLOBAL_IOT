namespace FutureOfWork.AI.Models;

/// <summary>
/// Resultado da classificação de portfólio/imagem
/// </summary>
public class ClassifyPortfolioResult
{
    public List<DetectedObject> DetectedObjects { get; set; } = new();
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}

