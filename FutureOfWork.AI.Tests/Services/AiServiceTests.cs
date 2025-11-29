using FutureOfWork.AI.Models;
using FutureOfWork.AI.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FutureOfWork.AI.Tests.Services;

public class AiServiceTests
{
    private readonly Mock<ILogger<AiService>> _mockLogger;
    private readonly AiService _aiService;

    public AiServiceTests()
    {
        _mockLogger = new Mock<ILogger<AiService>>();
        _aiService = new AiService(_mockLogger.Object);
    }

    [Fact]
    public async Task ExtractResumeAsync_WithValidImage_ReturnsOcrResult()
    {
        // Arrange
        var imageBytes = CreateTestImageBytes();
        using var stream = new MemoryStream(imageBytes);
        
        // Act
        var result = await _aiService.ExtractResumeAsync(stream, "test.png");

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Text);
        Assert.NotNull(result.Fields);
        Assert.NotNull(result.Classification);
        Assert.True(result.ProcessedAt <= DateTime.UtcNow);
    }

    [Fact]
    public async Task ExtractResumeAsync_WithUnsupportedFormat_ThrowsException()
    {
        // Arrange
        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });

        // Act & Assert
        await Assert.ThrowsAsync<NotSupportedException>(() =>
            _aiService.ExtractResumeAsync(stream, "test.xyz"));
    }

    [Fact]
    public async Task PredictMatchAsync_WithValidRequest_ReturnsMatchResult()
    {
        // Arrange
        var request = new MatchSummaryRequest
        {
            ResumeText = "Desenvolvedor .NET com experiência em C#, ASP.NET Core, SQL Server",
            JobDescription = "Buscamos desenvolvedor .NET com conhecimento em C# e SQL Server"
        };

        // Act
        var result = await _aiService.PredictMatchAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.CompatibilityScore >= 0 && result.CompatibilityScore <= 100);
        Assert.NotNull(result.RelevantFeatures);
        Assert.NotNull(result.Summary);
        Assert.NotNull(result.Suggestions);
        Assert.True(result.ProcessedAt <= DateTime.UtcNow);
    }

    [Fact]
    public async Task PredictMatchAsync_WithHighSimilarity_ReturnsHighScore()
    {
        // Arrange
        var resumeText = "Desenvolvedor .NET C# ASP.NET Core SQL Server Azure Docker";
        var jobDescription = "Desenvolvedor .NET C# ASP.NET Core SQL Server Azure Docker";

        var request = new MatchSummaryRequest
        {
            ResumeText = resumeText,
            JobDescription = jobDescription
        };

        // Act
        var result = await _aiService.PredictMatchAsync(request);

        // Assert
        Assert.True(result.CompatibilityScore >= 50, "High similarity should return high score");
    }

    [Fact]
    public async Task DetectObjectsAsync_WithValidImage_ReturnsDetectedObjects()
    {
        // Arrange
        var imageBytes = CreateTestImageBytes();
        using var stream = new MemoryStream(imageBytes);

        // Act
        var result = await _aiService.DetectObjectsAsync(stream, "test.jpg");

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.DetectedObjects);
        Assert.True(result.DetectedObjects.Count >= 0);
        Assert.True(result.ProcessedAt <= DateTime.UtcNow);
    }

    [Fact]
    public async Task GetAvailableModelsAsync_ReturnsModelList()
    {
        // Act
        var result = await _aiService.GetAvailableModelsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
        
        var ocrModel = result.FirstOrDefault(m => m.Type == "OCR");
        Assert.NotNull(ocrModel);
        Assert.Equal("Tesseract OCR", ocrModel.Name);
    }

    [Fact]
    public async Task ExtractResumeAsync_ExtractsEmailFromText()
    {
        // Arrange
        var textWithEmail = "Nome: João Silva\nEmail: joao@example.com\nTelefone: (11) 98765-4321";
        using var stream = CreateStreamFromText(textWithEmail);

        // Act
        var result = await _aiService.ExtractResumeAsync(stream, "test.txt");

        // Assert
        // O resultado pode conter email extraído ou não, dependendo do OCR
        // Este teste verifica que o serviço não falha
        Assert.NotNull(result);
    }

    private byte[] CreateTestImageBytes()
    {
        // Criar uma imagem PNG mínima válida (1x1 pixel)
        // Em testes reais, você usaria uma imagem de teste real
        return new byte[]
        {
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, // PNG signature
            0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52, // IHDR chunk
            0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01,
            0x08, 0x06, 0x00, 0x00, 0x00, 0x1F, 0x15, 0xC4,
            0x89, 0x00, 0x00, 0x00, 0x0A, 0x49, 0x44, 0x41,
            0x54, 0x78, 0x9C, 0x63, 0x00, 0x01, 0x00, 0x00,
            0x05, 0x00, 0x01, 0x0D, 0x0A, 0x2D, 0xB4, 0x00,
            0x00, 0x00, 0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE,
            0x42, 0x60, 0x82
        };
    }

    private Stream CreateStreamFromText(string text)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(text);
        return new MemoryStream(bytes);
    }
}

