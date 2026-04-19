using System.Diagnostics;
using FootballTeamsManager.Data;
using Microsoft.AspNetCore.Mvc;
using FootballTeamsManager.Models;

namespace FootballTeamsManager.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext dababase;

    public HomeController(ApplicationDbContext context)
    {
        dababase = context;
    }
    public IActionResult Index()
    {
        var result = dababase.Teams.First(t=> t.Id == 2);
        return View(result);
    }
}