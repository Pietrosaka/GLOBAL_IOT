namespace FutureOfWork.AI.Models;

/// <summary>
/// Representa um campo extraído de um documento com seu valor e nível de confiança
/// </summary>
public class ExtractedField
{
    public string Value { get; set; } = string.Empty;
    public double Confidence { get; set; }
}

