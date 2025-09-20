using System.ComponentModel.DataAnnotations;

namespace ProjectsWebApp.Data.Entities;

public class Project
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [MaxLength(100)]
    [MinLength(3)]
    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }

    [Url]
    [RegularExpression("github\\.com/.*")]
    public string? RepoUrl { get; set; }
    
    public List<Student> Students { get; set; } = new List<Student>();
}