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
    
    public IActionResult Recipe(int id)
    {
        if (id <= 0)
        {
            return BadRequest("Recipe id must be a positive integer");
        }
        var recipe = _context.Recipes.FirstOrDefault(r => r.Id == id);

        if (recipe == null)
        {
            return NotFound();
        }
        return View(recipe);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}