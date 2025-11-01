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
            
            _movieService.CreateMovie(movieInput);
        }

        return Ok($"Movie created @{movieInput.Id}");
    }

    [HttpGet]
    public ActionResult Delete(int id)
    {
        var movie = _dbContext.Movies.Find(id);
        return View(movie);
    }

    [HttpPost]
    public IActionResult DeletePost(int id)
    {  
        var movie = _dbContext.Movies.Find(id);
        _dbContext.Movies.Remove(movie);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public ActionResult Edit(int id)
    {
        
        var movie = _dbContext.Movies.Find(id);
        return View("Update", movie);
    }

    [HttpPost]
    public IActionResult EditPost(Movie inputModel, int id)
    {

        var movie = _dbContext.Movies.Find(inputModel.Id);
        movie.Year = inputModel.Year;
        movie.Name = inputModel.Name;

        _dbContext.SaveChanges();
        return RedirectToAction("Edit", movie.Id);
    }

    
    [HttpGet]
    public ActionResult Details(int id)
    {
        var movie = _dbContext.Movies.Find(id);
        return View(movie);
    }
}