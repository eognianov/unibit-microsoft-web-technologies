namespace ProjectsWebApp.Models.ViewModels;

public class ProjectViewModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? RepoUrl { get; set; }
    
    public virtual List<StudentViewModel> Students { get; set; } = new List<StudentViewModel>();
}