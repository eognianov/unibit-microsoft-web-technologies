using Microsoft.EntityFrameworkCore;
using ProjectsWebApp.Data.Entities;
using ProjectsWebApp.Models.ViewModels;

namespace ProjectsWebApp.Data;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<Student> Students { get; set; }
    public DbSet<Project> Projects { get; set; }

public DbSet<ProjectsWebApp.Models.ViewModels.StudentViewModel> StudentViewModel { get; set; } = default!;

public DbSet<ProjectsWebApp.Models.ViewModels.ProjectViewModel> ProjectViewModel { get; set; } = default!;
}