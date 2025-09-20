using System.ComponentModel.DataAnnotations;

namespace ProjectsWebApp.Data.Entities;

public class Student
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [MinLength(5)]
    [MaxLength(100)]
    public string Name { get; set; } = null!;
    
    public string Number { get; set; } = null!;

    public string? ProjectId { get; set; }
    public virtual Project? Project { get; set; }
}