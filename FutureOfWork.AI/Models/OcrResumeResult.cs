namespace FutureOfWork.AI.Models;

/// <summary>
/// Resultado da extração OCR de um currículo
/// </summary>
public class OcrResumeResult
{
    public string Text { get; set; } = string.Empty;
    public ExtractedFields Fields { get; set; } = new();
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    public ResumeClassification Classification { get; set; } = new();
}

/// <summary>
/// Campos estruturados extraídos do currículo
/// </summary>
public class ExtractedFields
{
    public ExtractedField Name { get; set; } = new();
    public ExtractedField Email { get; set; } = new();
    public ExtractedField Phone { get; set; } = new();
    public List<ExtractedSkill> Skills { get; set; } = new();
}

/// <summary>
/// Classificação das seções do currículo
/// </summary>
public class ResumeClassification
{
    public bool HasExperience { get; set; }
    public bool HasEducation { get; set; }
    public bool HasCertifications { get; set; }
    public double OverallConfidence { get; set; }
}

