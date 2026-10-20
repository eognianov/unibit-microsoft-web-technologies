using System.ComponentModel.DataAnnotations;

namespace SimpleBlog.Models;

public class Article
{
    public Guid Id { get; set; }
    
    [MinLength(2)]
    [MaxLength(100)]
    public string Title { get; set; }
    public string? Subtitle { get; set; }
    public string Body { get; set; }
    public string Author { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public string? ImagePath { get; set; }

    public string? Category { get; set; }

    public bool IsSatire { get; set; }
}