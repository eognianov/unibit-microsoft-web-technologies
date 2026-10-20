using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Data;
using SimpleBlog.Models;

namespace SimpleBlog.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        var count = _context.Articles.Count();
        var lastArticle = _context.Articles.OrderBy(a=>a.CreatedAt).FirstOrDefault();
        var articles = _context.Articles.ToList();

        var counts = new Dictionary<string, int> { };
        foreach (var article in articles)
        {
            if  (counts.ContainsKey(article.Category))
            {
                counts[article.Category]++;
            }
            else
            {
                counts.Add(article.Category, 1);
            }
        }

        var indexModel = new IndexVieModel
        {
            AllArticlesCount = count,
            LastArticle = lastArticle,
            ArticlesByCategory = counts
        };
        return View(indexModel);

    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}