using ProjectsWebApp.Data.Entities;
using ProjectsWebApp.Models.InputModels;

namespace ProjectsWebApp.Services.Contracts;

public interface IProjectsService
{
    void AddProject(ProjectInputModel project);
    List<Project> GetAllProjects();
    Project? GetProjectById(string id);
    void DeleteProject(string id);
    void UpdateProject(string projectId, ProjectInputModel updatedProject);
}