using ProjectsWebApp.Data.Entities;
using ProjectsWebApp.Models.InputModels;
using ProjectsWebApp.Services.Contracts;

namespace ProjectsWebApp.Services;

public class ProjectsService : IProjectsService
{
    public void AddProject(ProjectInputModel project)
    {
        throw new NotImplementedException();
    }

    public List<Project> GetAllProjects()
    {
        throw new NotImplementedException();
    }

    public Project GetProjectById(string id)
    {
        throw new NotImplementedException();
    }

    public void DeleteProject(string id)
    {
        throw new NotImplementedException();
    }

    public void UpdateProject(ProjectInputModel project)
    {
        throw new NotImplementedException();
    }

    public void AddStudentsToProject(string projectId, List<string> studentIds)
    {
        throw new NotImplementedException();
    }
}