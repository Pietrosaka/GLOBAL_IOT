namespace FutureOfWork.AI.Models;

/// <summary>
/// Representa uma habilidade extraída do currículo
/// </summary>
public class ExtractedSkill
{
    public string Name { get; set; } = string.Empty;
    public double Confidence { get; set; }
}

