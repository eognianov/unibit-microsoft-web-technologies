using System.ComponentModel.DataAnnotations;
using ProjectsWebApp.Data.Entities;

namespace ProjectsWebApp.Models.InputModels;

public class StudentInputModel
{
    [Required]
    [MinLength(5)]
    [MaxLength(100)]
    public string Name { get; set; } = null!;
    
    [MinLength(3)]
    [MaxLength(5)]
    public string Number { get; set; } = null!;
    
}