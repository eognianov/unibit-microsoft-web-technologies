using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MoviesWebApp.Models;

namespace MoviesWebApp.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return Ok("Index page");
    }

    public IActionResult Info()
    {
        return Ok("Info page");
    }
    
}