using Microsoft.AspNetCore.Mvc;
using MoviesWebApp.Models.InputModel;
using MoviesWebApp.Models.ViewModels;

namespace MoviesWebApp.Controllers;

public class MoviesController: Controller
{
    public ActionResult Index()
    {
        var indexViewModel = new MoviesIndexViewModel
        {
            Title = "Inception",
            Movies = new List<string>
            {
                "movie 1",
                "movie 2",
            }
        };
        return View(indexViewModel);
    }

    [HttpGet]
    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Create([FromForm]MovieInputModel inputModel)
    {
        if (ModelState.IsValid)
        {
            return Ok($"Movie with name: {inputModel.Name} created");
        }

        return View(inputModel);
    }

    [HttpGet]
    public ActionResult Delete(int id)
    {
        return Ok("Delete Movies page with id " + id);
    }

    [HttpPost]
    public IActionResult DeletePost(int id)
    {  
        return Ok("Deleted Movie with id " + id);
    }

    [HttpGet]
    public ActionResult Edit(int id)
    {
        return Ok("Edit Movies page");
    }

    [HttpPost]
    public IActionResult EditPost(MovieInputModel inputModel)
    {
        
        if (ModelState.IsValid)
        {
            return Ok($"Movie with name: {inputModel.Name} edit");
        }
        
        var errors = ModelState
            .Where(e => e.Value.Errors.Count > 0)
            .ToDictionary(
                e => e.Key,
                e => e.Value.Errors.Select(err => err.ErrorMessage).ToArray()
            );
        
        return BadRequest(new
        {
            Message = "Validation failed",
            Errors = errors
        });
    }
}