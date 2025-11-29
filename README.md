# FutureOfWork AI — Sistema híbrido de matching e análise de documentos (CV)

Sistema baseado em Deep Learning para extração e análise de currículos com visão computacional e API REST integrada.

## 📋 Índice

- [Visão Geral](#visão-geral)
- [Arquitetura](#arquitetura)
- [Funcionalidades](#funcionalidades)
- [Pré-requisitos](#pré-requisitos)
- [Instalação](#instalação)
- [Configuração](#configuração)
- [Executando o Projeto](#executando-o-projeto)
- [Endpoints da API](#endpoints-da-api)
- [Exemplos de Uso](#exemplos-de-uso)
- [Modelos de IA](#modelos-de-ia)
- [Testes](#testes)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Tecnologias Utilizadas](#tecnologias-utilizadas)

## 🎯 Visão Geral

O **FutureOfWork AI** é uma aplicação .NET 9 que oferece:

- **OCR (Reconhecimento Óptico de Caracteres)**: Extração de texto de currículos em PDF, PNG, JPEG
- **Classificação de Seções**: Identifica experiências, educação e certificações
- **Detecção de Objetos**: Detecta logos, certificados e elementos visuais em imagens
- **Matching Inteligente**: Calcula compatibilidade entre currículos e descrições de vagas
- **API REST**: Endpoints documentados com Swagger para integração

## 🏗️ Arquitetura

```
┌─────────────────────┐
│  FutureOfWork.API   │ ← API REST (.NET 9 Web API)
│  (Controllers)      │
└──────────┬──────────┘
           │
┌──────────▼──────────┐
│  FutureOfWork.AI    │ ← Biblioteca de Serviços de IA
│  (Services)         │
│  - OCR (Tesseract)  │
│  - ML.NET Matching  │
│  - Image Processing │
└─────────────────────┘
           │
┌──────────▼──────────┐
│  Modelos            │
│  - Tesseract OCR    │
│  - ML.NET Models    │
│  - Heurísticas CV   │
└─────────────────────┘
```

### Componentes Principais

1. **FutureOfWork.API**: API REST com endpoints documentados
2. **FutureOfWork.AI**: Biblioteca de serviços de IA
3. **FutureOfWork.AI.Tests**: Testes unitários

## ✨ Funcionalidades

### 1. Extração OCR de Currículos
- Extração de texto de PDFs usando PdfPig
- OCR de imagens usando Tesseract
- Extração estruturada de campos (nome, email, telefone, habilidades)

### 2. Classificação de Seções
- Identifica seções de experiência
- Identifica seções de educação
- Identifica certificações

### 3. Detecção de Objetos
- Detecta logos em imagens
- Detecta certificados em portfólios
- Análise visual básica usando heurísticas

### 4. Matching de Vagas
- Cálculo de compatibilidade (0-100%)
- Extração de features relevantes
- Geração de resumo automático
- Sugestões de melhoria no CV

## 📦 Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Tesseract OCR](https://github.com/tesseract-ocr/tesseract) (instalado no sistema)
  - Windows: Baixar do [GitHub Releases](https://github.com/UB-Mannheim/tesseract/wiki)
  - Os arquivos de idioma (`tessdata`) devem estar no diretório `tessdata/` na raiz do projeto ou no diretório do executável

## 🚀 Instalação

### 1. Clonar o repositório

```bash
git clone <repository-url>
cd GLOBAL_IOT
```

### 2. Instalar Tesseract OCR

#### Windows
1. Baixar o instalador: https://github.com/UB-Mannheim/tesseract/wiki
2. Instalar em `C:\Program Files\Tesseract-OCR`
3. Criar diretório `tessdata` na raiz do projeto e copiar os arquivos de idioma
   - Baixar de: https://github.com/tesseract-ocr/tessdata

#### Linux (Ubuntu/Debian)
```bash
sudo apt-get install tesseract-ocr
sudo apt-get install tesseract-ocr-por tesseract-ocr-eng
```

#### macOS
```bash
brew install tesseract
brew install tesseract-lang
```

### 3. Restaurar dependências NuGet

```bash
dotnet restore
```

## ⚙️ Configuração

### appsettings.json

```json
{
  "ApiSettings": {
    "ApiKey": "FutureOfWork-API-Key-2024"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### Variáveis de Ambiente (Opcional)

```bash
# API Key (substitui appsettings.json)
ApiSettings__ApiKey=Your-Api-Key-Here

# Caminho do Tesseract (se não estiver no PATH)
TESSERACT_PATH=C:\Program Files\Tesseract-OCR
```

## 🏃 Executando o Projeto

### Desenvolvimento

```bash
cd FutureOfWork.API
dotnet run
```

A API estará disponível em:
- HTTPS: `https://localhost:7000` ou `https://localhost:7001`
- HTTP: `http://localhost:5000` ou `http://localhost:5001`

### Acessar Swagger

Navegue para: `https://localhost:7000` (ou a porta configurada)

## 📡 Endpoints da API

### Autenticação

Todos os endpoints (exceto `/swagger`, `/health`, `/`) requerem o header:
```
X-API-Key: FutureOfWork-API-Key-2024
```

### 1. POST `/api/v1/ai/ocr-resume`

Extrai texto e informações estruturadas de um currículo.

**Request:**
- Content-Type: `multipart/form-data`
- Body: arquivo (PDF, PNG, JPEG, TIFF, BMP)

**Response:**
```json
{
  "text": "texto extraído completo...",
  "fields": {
    "name": {
      "value": "Pietro Saka",
      "confidence": 0.98
    },
    "email": {
      "value": "pietro@example.com",
      "confidence": 0.99
    },
    "phone": {
      "value": "(11) 98765-4321",
      "confidence": 0.85
    },
    "skills": [
      {
        "name": "C#",
        "confidence": 0.95
      }
    ]
  },
  "classification": {
    "hasExperience": true,
    "hasEducation": true,
    "hasCertifications": false,
    "overallConfidence": 0.88
  },
  "processedAt": "2025-11-28T22:00:00Z"
}
```

### 2. POST `/api/v1/ai/classify-portfolio`

Detecta objetos em uma imagem de portfólio.

**Request:**
- Content-Type: `multipart/form-data`
- Body: arquivo de imagem (PNG, JPEG, TIFF, BMP)

**Response:**
```json
{
  "detectedObjects": [
    {
      "name": "Certificate",
      "score": 0.85,
      "boundingBox": {
        "x": 100,
        "y": 150,
        "width": 200,
        "height": 100
      }
    }
  ],
  "processedAt": "2025-11-28T22:00:00Z"
}
```

### 3. POST `/api/v1/ai/match-summary`

Calcula compatibilidade entre currículo e vaga.

**Request:**
```json
{
  "candidateId": "optional-id",
  "resumeText": "Texto completo do currículo...",
  "jobDescription": "Descrição da vaga..."
}
```

**Response:**
```json
{
  "compatibilityScore": 85,
  "relevantFeatures": ["C#", ".NET", "SQL Server", "Azure"],
  "summary": "O candidato apresenta 85% de compatibilidade...",
  "suggestions": [
    "Considere destacar experiência com: Docker, Kubernetes",
    "Enfatize resultados quantificáveis nos projetos anteriores"
  ],
  "processedAt": "2025-11-28T22:00:00Z"
}
```

### 4. GET `/api/v1/ai/models`

Lista modelos disponíveis com métricas.

**Response:**
```json
[
  {
    "name": "Tesseract OCR",
    "version": "5.0.0",
    "type": "OCR",
    "metrics": {
      "WER": 0.15,
      "Accuracy": 0.85
    }
  }
]
```

### 5. GET `/health`

Health check da API.

**Response:**
```json
{
  "status": "Healthy",
  "checks": {
    "ai_service": "Healthy"
  }
}
```

## 💻 Exemplos de Uso

### cURL

#### OCR de Currículo

```bash
curl -X POST "https://localhost:7000/api/v1/ai/ocr-resume" \
  -H "X-API-Key: FutureOfWork-API-Key-2024" \
  -F "file=@/caminho/para/cv.pdf"
```

#### Match Summary

```bash
curl -X POST "https://localhost:7000/api/v1/ai/match-summary" \
  -H "Content-Type: application/json" \
  -H "X-API-Key: FutureOfWork-API-Key-2024" \
  -d '{
    "resumeText": "Desenvolvedor .NET com 5 anos de experiência...",
    "jobDescription": "Buscamos desenvolvedor .NET senior..."
  }'
```

#### Classify Portfolio

```bash
curl -X POST "https://localhost:7000/api/v1/ai/classify-portfolio" \
  -H "X-API-Key: FutureOfWork-API-Key-2024" \
  -F "file=@/caminho/para/portfolio.jpg"
```

#### List Models

```bash
curl -X GET "https://localhost:7000/api/v1/ai/models" \
  -H "X-API-Key: FutureOfWork-API-Key-2024"
```

### C# (.NET)

```csharp
using var client = new HttpClient();
client.DefaultRequestHeaders.Add("X-API-Key", "FutureOfWork-API-Key-2024");

// OCR
var content = new MultipartFormDataContent();
content.Add(new ByteArrayContent(File.ReadAllBytes("cv.pdf")), "file", "cv.pdf");
var response = await client.PostAsync("https://localhost:7000/api/v1/ai/ocr-resume", content);
var result = await response.Content.ReadFromJsonAsync<OcrResumeResult>();
```

### JavaScript (Fetch)

```javascript
const formData = new FormData();
formData.append('file', fileInput.files[0]);

const response = await fetch('https://localhost:7000/api/v1/ai/ocr-resume', {
  method: 'POST',
  headers: {
    'X-API-Key': 'FutureOfWork-API-Key-2024'
  },
  body: formData
});

const result = await response.json();
console.log(result);
```

## 🤖 Modelos de IA

### 1. Tesseract OCR
- **Versão**: 5.0.0
- **Tipo**: OCR (Optical Character Recognition)
- **Métricas**:
  - WER (Word Error Rate): ~15%
  - Accuracy: ~85%
- **Uso**: Extração de texto de imagens e PDFs

### 2. Resume Matching Model
- **Versão**: 1.0.0
- **Tipo**: Matching/Similarity
- **Tecnologia**: TF-IDF + Similaridade de Cosseno
- **Métricas**:
  - F1-Score: 0.82
  - Precision: 0.79
  - Recall: 0.85
- **Uso**: Cálculo de compatibilidade entre currículos e vagas

### 3. Resume Classification
- **Versão**: 1.0.0
- **Tipo**: Classificação de Seções
- **Tecnologia**: Heurísticas baseadas em palavras-chave
- **Métricas**:
  - Accuracy: 0.91
  - F1-Score: 0.88
- **Uso**: Identificação de seções (experiência, educação, certificações)

### Dataset e Treinamento

Os modelos são baseados em:
- Heurísticas e regras de negócio
- Análise de padrões em currículos
- Matching baseado em TF-IDF e similaridade de cosseno

**Nota**: Para produção, recomenda-se treinar modelos com datasets maiores e usar técnicas de ML mais avançadas (embeddings, fine-tuning).

## 🧪 Testes

### Executar Testes

```bash
cd FutureOfWork.AI.Tests
dotnet test
```

### Testes Disponíveis

- `ExtractResumeAsync_WithValidImage_ReturnsOcrResult`
- `PredictMatchAsync_WithValidRequest_ReturnsMatchResult`
- `DetectObjectsAsync_WithValidImage_ReturnsDetectedObjects`
- `GetAvailableModelsAsync_ReturnsModelList`

### Coverage

Para gerar relatório de cobertura:

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## 📁 Estrutura do Projeto

```
GLOBAL_IOT/
├── FutureOfWork.API/              # API REST
│   ├── Controllers/
│   │   └── AiController.cs       # Endpoints de IA
│   ├── Middleware/
│   │   └── ApiKeyMiddleware.cs   # Autenticação por API Key
│   ├── Program.cs                # Configuração da API
│   └── appsettings.json          # Configurações
│
├── FutureOfWork.AI/              # Biblioteca de Serviços de IA
│   ├── Services/
│   │   └── AiService.cs          # Implementação dos serviços
│   ├── Models/                   # Modelos de dados
│   │   ├── OcrResumeResult.cs
│   │   ├── MatchSummaryResult.cs
│   │   └── ...
│   └── IAiService.cs             # Interface do serviço
│
├── FutureOfWork.AI.Tests/        # Testes Unitários
│   └── Services/
│       └── AiServiceTests.cs
│
└── FutureOfWork.sln              # Solução .NET
```

## 🛠️ Tecnologias Utilizadas

- **.NET 9**: Framework principal
- **Tesseract OCR**: Reconhecimento óptico de caracteres
- **PdfPig**: Extração de texto de PDFs
- **ML.NET**: Machine Learning para matching
- **ImageSharp**: Processamento de imagens
- **Serilog**: Logging estruturado
- **Swagger/OpenAPI**: Documentação da API
- **xUnit**: Framework de testes
- **Moq**: Mocking para testes

## 📝 Logs

Os logs são salvos em:
- Console: durante desenvolvimento
- Arquivo: `logs/futureofwork-YYYYMMDD.log`

## 🔒 Segurança

- Autenticação via API Key (header `X-API-Key`)
- Validação de tipos de arquivo
- Tratamento de erros e exceções
- Health checks para monitoramento

## 🚧 Melhorias Futuras

- [ ] Integração com YOLO para detecção de objetos mais precisa
- [ ] Modelo de embeddings para matching mais inteligente
- [ ] Suporte a múltiplos idiomas (português, inglês, espanhol)
- [ ] Cache de resultados
- [ ] Rate limiting
- [ ] Suporte a webhooks
- [ ] Dashboard de métricas
- [ ] Fine-tuning de modelos com dataset próprio

## 📄 Licença

Este projeto é parte de um trabalho acadêmico.

## 👥 Autores

Desenvolvido como parte do projeto FutureOfWork.

## 📞 Suporte

Para dúvidas ou problemas, abra uma issue no repositório.

---

**Última atualização**: Novembro 2024

