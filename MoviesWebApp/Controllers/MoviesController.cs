using Microsoft.AspNetCore.Mvc;
using MoviesWebApp.Models.InputModel;

namespace MoviesWebApp.Controllers;

public class MoviesController: Controller
{
    public ActionResult Index()
    {
        return Ok("All Movies");
    }

    [HttpGet]
    public ActionResult Create()
    {
        return Ok("Create Movies page");
    }

    [HttpPost]
    public ActionResult Create([FromForm]MovieInputModel inputModel)
    {
        if (ModelState.IsValid)
        {
            return Ok($"Movie with name: {inputModel.Name} created");
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