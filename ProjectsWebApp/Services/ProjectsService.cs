using Microsoft.EntityFrameworkCore;
using ProjectsWebApp.Data;
using ProjectsWebApp.Data.Entities;
using ProjectsWebApp.Models.InputModels;
using ProjectsWebApp.Services.Contracts;

namespace ProjectsWebApp.Services;

public class ProjectsService : IProjectsService
{
    private readonly AppDbContext _dbContext;

    public ProjectsService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public void AddProject(ProjectInputModel project)
    {
        var newProject = new Project
        {
            Name = project.Name,
            Description = project.Description,
            RepoUrl = project.RepoUrl
        };
        _dbContext.Projects.Add(newProject);
        _dbContext.SaveChanges();
    }

    public List<Project> GetAllProjects()
    {
        return _dbContext.Projects.ToList();
    }

    public Project? GetProjectById(string id)
    {
        var project = _dbContext.Projects.FirstOrDefault(p => p.Id == id);
        return project;
    }

    public void DeleteProject(string id)
    {
        var project = _dbContext.Projects.FirstOrDefault(p => p.Id == id);
        if (project != null)
        {
            _dbContext.Projects.Remove(project);
            _dbContext.SaveChanges();
        }
    }

    public void UpdateProject(string projectId, ProjectInputModel updateProject)
    {
        var existingProject = _dbContext.Projects.FirstOrDefault(p => p.Id == projectId);
        if (existingProject != null)
        {
            existingProject.Name = updateProject.Name;
            existingProject.Description = updateProject.Description;
            existingProject.RepoUrl = updateProject.RepoUrl;
            _dbContext.SaveChanges();
        }
    }
}