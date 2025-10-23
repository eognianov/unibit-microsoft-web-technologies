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
        Console.WriteLine("Before call service");
        
    }

    public Movie GetMovie(int id)
    {
        throw new NotImplementedException();
    }

    public List<Movie> GetMovies()
    {
        var movies = new List<Movie>
        {
            new Movie
            {
                Name = "Inception 1",
                Genre = "Sci-Fi",
                Director = "Christopher Nolan",
                Actors = ["Leonardo DiCaprio", "Ellen Page", "Tom Hardy"],
                Year = 2010
            },
            new Movie
            {
                Name = "The Shawshank Redemption",
                Genre = "Drama",
                Director = "Frank Darabont",
                Actors = ["Tim Robbins", "Morgan Freeman"],
                Year = 1994
            },
            new Movie
            {
                Name = "The Matrix",
                Genre = "Action",
                Director = "The Wachowskis",
                Actors = ["Keanu Reeves", "Laurence Fishburne", "Carrie-Anne Moss"],
                Year = 1999
            }
        };
        return movies;
    }
}