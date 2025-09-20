using Microsoft.AspNetCore.Mvc;
using ProjectsWebApp.Models.InputModels;
using ProjectsWebApp.Services.Contracts;

namespace ProjectsWebApp.Controllers;

public class ProjectsController : Controller
{
    private readonly IProjectsService _projectsService;

    public ProjectsController(IProjectsService projectsService)
    {
        _projectsService = projectsService;
    }
    
    public IActionResult Index()
    {
        var projects = _projectsService.GetAllProjects();
        return Ok(Json(projects));
    }
    
    public IActionResult Create(ProjectInputModel project)
    {
        _projectsService.AddProject(project);
        return Created();
    }
    
    public IActionResult Delete(string id)
    {
        _projectsService.DeleteProject(id);
        return Ok();
    }

    public IActionResult Update(ProjectInputModel project)
    {
        _projectsService.UpdateProject(project);
        return Ok();
    }
}