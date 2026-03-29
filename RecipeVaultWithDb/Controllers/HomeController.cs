using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RecipeVaultWithDb.Data;
using RecipeVaultWithDb.Models;

namespace RecipeVaultWithDb.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public IActionResult Index()
    {
        var lastRecipe = _context.Recipes.OrderByDescending(r=>r.CreatedAt).Take(1).FirstOrDefault();
        return View(lastRecipe);
    }

    public IActionResult Recipes()
    {
        var recipes = _context.Recipes.ToList();
        return View(recipes);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}