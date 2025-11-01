using CrudSandbox.Models;
using Microsoft.EntityFrameworkCore;

namespace CrudSandbox;

public class MovieDbContext : DbContext
{
    
    public DbSet<Movie> Movies { get; set; }
    
    
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionString = "Server=127.0.0.1;Port=3306;Database=cshaphwcaq;User Id=dev;Password=dddeeevvv;";
        optionsBuilder.UseMySQL(connectionString);
    }
    
}