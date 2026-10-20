using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Services;

namespace SimpleBlog.Controllers;

[ApiController]
[Route("api/generator")]
public class GeneratorController: Controller
{
    private readonly OpenAIService _openAiService;

    public GeneratorController(OpenAIService openAiService)
    {
        _openAiService = openAiService;
    }

    [HttpPost("news")]
    public async Task<ActionResult<GeneratedNews>> GenerateNews(GenerateNewsRequest request)
    {
        var news = await _openAiService.GenerateNewsAsync(request.Category);
        return Ok(news);
    }

    [HttpPost("image")]
    public async Task<IActionResult> GenerateImage(GenerateImageRequest request)
    {
        var image = await _openAiService.GenerateImageAsync(request.Title, request.Subtitle, request.Category,
            request.Content, request.IsSatire);
        return File(image, "image/png");
    }
}

public record GenerateNewsRequest(string Category);

public record GenerateImageRequest(string Title, string Subtitle, string Category, string Content, bool IsSatire);
