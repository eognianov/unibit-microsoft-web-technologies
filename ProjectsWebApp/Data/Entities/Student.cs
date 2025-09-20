using System.ComponentModel.DataAnnotations;

namespace ProjectsWebApp.Data.Entities;

public class Student
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [MinLength(5)]
    [MaxLength(100)]
    public string Name { get; set; } = null!;
    
    [MinLength(3)]
    [MaxLength(5)]
    public string Number { get; set; } = null!;

    [MinLength(30)]
    [MaxLength(500)]
    public string? ProjectId { get; set; }
    public virtual Project? Project { get; set; }
}