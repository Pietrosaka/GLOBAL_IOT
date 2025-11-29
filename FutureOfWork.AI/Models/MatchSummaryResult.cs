namespace FutureOfWork.AI.Models;

/// <summary>
/// Resultado da análise de matching entre currículo e vaga
/// </summary>
public class MatchSummaryResult
{
    public int CompatibilityScore { get; set; } // 0-100
    public List<string> RelevantFeatures { get; set; } = new();
    public string Summary { get; set; } = string.Empty;
    public List<string> Suggestions { get; set; } = new();
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Informações sobre um modelo de IA disponível
/// </summary>
public class ModelInfo
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // OCR, Classification, ObjectDetection, Matching
    public Dictionary<string, double> Metrics { get; set; } = new();
}

