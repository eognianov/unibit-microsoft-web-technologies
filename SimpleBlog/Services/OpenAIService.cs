using System.Text.Json;
using OpenAI.Chat;
using OpenAI.Images;

namespace SimpleBlog.Services;

using OpenAI;

public class OpenAIService
{
    private readonly OpenAIClient _client;
    private readonly ImageClient _imageClient;
    private readonly ChatClient _chatClient;
    private readonly IConfiguration config;

    public OpenAIService(IConfiguration configuration)
    {
        config = configuration;
        var apiKey = configuration["OpenAIAPIKey"] ??
                     throw new InvalidOperationException("Configuration value 'OpenAIAPIKey' not found.");
        var textGenarationModel = configuration["TextGenerationModel"];
        var imageGenerationModel = configuration["ImageGenerationModel"];
        _client = new OpenAIClient(apiKey);
        _chatClient = new ChatClient(textGenarationModel, apiKey);
        _imageClient = new ImageClient(imageGenerationModel, apiKey);
    }
    
    public async Task<Byte[]> GenerateImageAsync(string title, string subtitle, string category, string content,
        bool isSatire)
    {
        var prompt = (config["ImageGenerationPrompt"] ??
                      throw new InvalidOperationException("Configuration value 'ImageGenerationPrompt' not found."))
            .Replace("{title}", title)
            .Replace("{subtitle}", subtitle)
            .Replace("{category}", category)
            .Replace("{content}", content)
            .Replace("{satireNote}", isSatire ? "Style: exaggerated, cartoonish satire." : "");

        var options = new ImageGenerationOptions
        {
            Size = GeneratedImageSize.W1024xH1024
        };

        var image = await _imageClient.GenerateImageAsync(prompt, options);

        return image.Value.ImageBytes.ToArray();
    }

    public async Task<GeneratedNews> GenerateNewsAsync(string category)
    {
        var prompt = config["NewsGenerationPrompt"] ??
                     throw new InvalidOperationException("Configuration value 'NewsGenerationPrompt' not found.");

        prompt += $"\n\nКатегория на новината: \"{category}\". Темата трябва да отговаря на тази категория, а стойността на полето \"category\" трябва да е точно тази категория.";

        var completion = await _chatClient.CompleteChatAsync(new UserChatMessage(prompt));
        var json = completion.Value.Content[0].Text;

        return JsonSerializer.Deserialize<GeneratedNews>(json, new JsonSerializerOptions
               {
                   PropertyNameCaseInsensitive = true
               }) ??
               throw new InvalidOperationException("OpenAI returned an empty news response.");
    }
}

public record GeneratedNews(string Title, string Subtitle, string Category, string Content, bool IsSatire);
