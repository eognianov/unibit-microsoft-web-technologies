using System.ComponentModel.DataAnnotations;

namespace FootballTeamsManager.Models;

public class Team
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string City { get; set; } = string.Empty;
    
    public int? Founded { get; set; }
    
    public string? LogoUrl { get; set; }
    
}