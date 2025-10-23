using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesWebApp.Models;

public class Movie
{
    public int Id { get; set; }
    
    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public string Name { get; set; }
    
    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public string Genre { get; set; }
    
    
    public string? Director { get; set; } = String.Empty;
    
    
    [Required]
    [Range(1900, 2025)]
    public int Year { get; set; }
}