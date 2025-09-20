using ProjectsWebApp.Data.Entities;
using ProjectsWebApp.Models.InputModels;

namespace ProjectsWebApp.Services.Contracts;

public interface IStudentsService
{
    void AddStudent(StudentInputModel student);
    List<Student> GetAllStudents();
    Student? GetStudentById(string id);
    void DeleteStudent(string id);
    void UpdateStudent(string id, StudentInputModel student);
    
    void AddStudentsToProject(string projectId, List<string> studentIds);
}