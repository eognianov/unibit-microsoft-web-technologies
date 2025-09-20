using Microsoft.AspNetCore.Mvc;
using ProjectsWebApp.Models.InputModels;
using ProjectsWebApp.Services.Contracts;

namespace ProjectsWebApp.Controllers;

public class StudentsController : Controller
{
    private readonly IStudentsService _studentsService;

    public StudentsController(IStudentsService studentsService)
    {
        _studentsService = studentsService;
    }

    public IActionResult Index()
    {
        var students = _studentsService.GetAllStudents();
        return Ok(Json(students));
    }
    
    public IActionResult Create(StudentInputModel student)
    {
        _studentsService.AddStudent(student);
        return Created();
    }
    
    public IActionResult Delete(string id)
    {
        _studentsService.DeleteStudent(id);
        return NoContent();
    }

    public IActionResult Update(string id, StudentInputModel studentInputModel)
    {
        _studentsService.UpdateStudent(id, studentInputModel);
        return Ok();
    }
}