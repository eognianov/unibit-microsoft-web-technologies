namespace SimpleBlog.Models;

public class Article
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public string Author { get; set; }
    public DateTime CreatedAt { get; set; }
}