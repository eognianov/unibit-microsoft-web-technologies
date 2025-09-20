using ProjectsWebApp.Data;
using ProjectsWebApp.Data.Entities;
using ProjectsWebApp.Models.InputModels;
using ProjectsWebApp.Services.Contracts;

namespace ProjectsWebApp.Services;

public class StudentsService: IStudentsService
{
    private readonly AppDbContext _dbContext;

    public StudentsService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public void AddStudent(StudentInputModel student)
    {
        var newStudent = new Student
        {
            Name = student.Name,
            Number = student.Number
        };
        _dbContext.Students.Add(newStudent);
        _dbContext.SaveChanges();
    }

    public List<Student> GetAllStudents()
    {
        return _dbContext.Students.ToList();
    }

    public Student? GetStudentById(string id)
    {
        var student = _dbContext.Students.FirstOrDefault(s => s.Id == id);
        return student;
    }

    public void DeleteStudent(string id)
    {
        var student = _dbContext.Students.FirstOrDefault(s => s.Id == id);
        if (student != null)
        {
            _dbContext.Students.Remove(student);
            _dbContext.SaveChanges();
        }
    }

    public void UpdateStudent(string id, StudentInputModel updatedStudent)
    {
        var existingStudent = GetStudentById(id);
        if (existingStudent != null)
        {
            existingStudent.Name = updatedStudent.Name;
            existingStudent.Number = updatedStudent.Number;
            _dbContext.SaveChanges();
        }
    }

    public void AddStudentToProject(string projectId, string studentId)
    {
        var student = _dbContext.Students.FirstOrDefault(s => s.Id == studentId);
        var project = _dbContext.Projects.FirstOrDefault(p => p.Id == projectId);
        if (student != null && project != null)
        {
            student.ProjectId = projectId;
            _dbContext.SaveChanges();
        }
    }
}