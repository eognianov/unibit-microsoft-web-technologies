using Microsoft.AspNetCore.Mvc;
using MoviesWebApp.Models;
using MoviesWebApp.Models.InputModel;
using MoviesWebApp.Services;

namespace MoviesWebApp.Controllers;

public class MoviesController: Controller
{
    private readonly IMovieService _movieService;
    private readonly MoviesAppDbContext _dbContext;

    public MoviesController(IMovieService movieService, MoviesAppDbContext dbContext)
    {
        _movieService = movieService;
        _dbContext = dbContext;
    }

    public ActionResult Index()
    {
        var movies = _movieService.GetMovies();
        return View(movies);
    }

    [HttpGet]
    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Create([FromForm]Movie movieInput)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _dbContext.Movies.Add(movieInput);
                _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        return View(movieInput);
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