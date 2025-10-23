using MoviesWebApp.Models;
using MoviesWebApp.Models.InputModel;

namespace MoviesWebApp.Services;


public interface IMovieService
{
    void CreateMovie(Movie movieInputModel);
    Movie GetMovie(int id);
    List<Movie> GetMovies();
}


public class MovieService: IMovieService
{
    private readonly MoviesAppDbContext _context;

    public MovieService(MoviesAppDbContext context)
    {
        _context = context;
    }
    public void CreateMovie(Movie movieInputModel)
    {
        _context.Movies.Add(movieInputModel);
        _context.SaveChanges();
    }

    public Movie GetMovie(int id)
    {
        throw new NotImplementedException();
    }

    public List<Movie> GetMovies()
    {
        var movies = _context.Movies.Where(m=>m.Director=="me").ToList();
        return movies;
    }
}
