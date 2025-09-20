using Microsoft.AspNetCore.Mvc;
using ProjectsWebApp.Models.InputModels;
using ProjectsWebApp.Models.ViewModels;
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
        var projectsViewModel = _projectsService.GetAllProjects().Select(p => new ProjectViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            RepoUrl = p.RepoUrl,
            Students = p.Students.Select(s=> new StudentViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Number = s.Number,
            }).ToList()
        }).ToList();
        return View(projectsViewModel);
    }
    
    public IActionResult Create()
    {
        return View();
    }
    
    public IActionResult Details(string id)
    {
        var project = _projectsService.GetProjectById(id);
        if (project == null)
        {
            return NotFound();
        }
        return View(new ProjectViewModel
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            RepoUrl = project.RepoUrl,
            Students = project.Students.Select(s=> new StudentViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Number = s.Number,
            }).ToList()
        });
    }
    
    [HttpPost]
    public IActionResult Create(ProjectInputModel project)
    {
        _projectsService.AddProject(project);
        return RedirectToAction("Index");
    }
    

    public IActionResult Delete(string id)
    {
        var project = _projectsService.GetProjectById(id);
        if (project == null)
        {
            return NotFound();
        }
        return View(new ProjectViewModel
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            RepoUrl = project.RepoUrl,
        });
    }
    
    [HttpPost]
    public IActionResult PostDelete(string id)
    {
        _projectsService.DeleteProject(id);
        return RedirectToAction("Index");
    }

    public IActionResult Edit(string id)
    {
        var project = _projectsService.GetProjectById(id);
        if (project == null)
        {
            return NotFound();
        }

        return View(new ProjectViewModel
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            RepoUrl = project.RepoUrl,
        });
    }
    
    
    [HttpPost]
    public IActionResult Edit([FromRoute]string id, ProjectInputModel project)
    {
        _projectsService.UpdateProject(id, project);
        return RedirectToAction("Details", new {id});
    }
}