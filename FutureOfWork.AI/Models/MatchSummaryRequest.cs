namespace FutureOfWork.AI.Models;

/// <summary>
/// Requisição para análise de matching entre currículo e vaga
/// </summary>
public class MatchSummaryRequest
{
    public string? CandidateId { get; set; }
    public string ResumeText { get; set; } = string.Empty;
    public string JobDescription { get; set; } = string.Empty;
}

