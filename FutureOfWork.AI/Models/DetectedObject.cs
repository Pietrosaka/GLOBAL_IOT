namespace FutureOfWork.AI.Models;

/// <summary>
/// Representa um objeto detectado em uma imagem
/// </summary>
public class DetectedObject
{
    public string Name { get; set; } = string.Empty;
    public BoundingBox BoundingBox { get; set; } = new();
    public double Score { get; set; }
}

/// <summary>
/// Caixa delimitadora do objeto detectado
/// </summary>
public class BoundingBox
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
}

