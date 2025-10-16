using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MoviesWebApp.Models;

namespace MoviesWebApp.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Info()
    {
        return Ok("Info page");
    }
    
}