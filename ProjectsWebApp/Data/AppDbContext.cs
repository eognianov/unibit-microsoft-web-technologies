using Microsoft.EntityFrameworkCore;
using ProjectsWebApp.Data.Entities;

namespace ProjectsWebApp.Data;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<Student> Students { get; set; }
    public DbSet<Project> Projects { get; set; }
}