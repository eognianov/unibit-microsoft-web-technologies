using System.ComponentModel.DataAnnotations;
using ProjectsWebApp.Data.Entities;

namespace ProjectsWebApp.Models.InputModels;

public class ProjectInputModel
{
    [Required]
    [MaxLength(100)]
    [MinLength(3)]
    public string Name { get; set; } = null!;
 
    [MaxLength(4000)]
    [MinLength(20)]
    public string? Description { get; set; }

    [MaxLength(1000)]
    [Url]
    [RegularExpression("github\\.com/.*")]
    public string? RepoUrl { get; set; }
    
    public virtual List<Student> Students { get; set; } = new List<Student>();
}