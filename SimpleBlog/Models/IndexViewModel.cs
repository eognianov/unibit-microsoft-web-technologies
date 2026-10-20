namespace SimpleBlog.Models;

public class IndexVieModel
{
    public int AllArticlesCount;

    public Dictionary<string, int> ArticlesByCategory { get; set; }

    public Article? LastArticle { get; set; }
}