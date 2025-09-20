namespace ProjectsWebApp.Models.ViewModels;

public class StudentViewModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = null!;
    public string Number { get; set; } = null!;
    public virtual ProjectViewModel? Project { get; set; }
}