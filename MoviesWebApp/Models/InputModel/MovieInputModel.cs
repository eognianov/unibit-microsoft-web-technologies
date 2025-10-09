using System.ComponentModel.DataAnnotations;

namespace MoviesWebApp.Models.InputModel;

public class MovieInputModel
{
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
    [MinLength(1)]
    public List<string> Actors { get; set; }
    
    [Required]
    [Range(1900, 2025)]
    public int Year { get; set; }
}