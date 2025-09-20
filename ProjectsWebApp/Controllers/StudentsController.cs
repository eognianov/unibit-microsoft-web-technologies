using Microsoft.AspNetCore.Mvc;
using ProjectsWebApp.Models.InputModels;
using ProjectsWebApp.Models.ViewModels;
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
        var studentViewModels  = new List<StudentViewModel>();
        foreach (var student in students)
        {
            var viewModel = new StudentViewModel
            {
                Id = student.Id,
                Name = student.Name,
                Number = student.Number,
            };
            if (student.Project != null)
            {
                viewModel.Project = new ProjectViewModel
                {
                    Id = student.Project.Id,
                    Name = student.Project.Name,
                    Description = student.Project.Description,
                    RepoUrl = student.Project.RepoUrl,
                };
            }
            
            studentViewModels.Add(viewModel);
        }
        return View(studentViewModels);
    }
    
    public IActionResult Create()
    {
        return View();
    }
    
    public IActionResult Details(string id)
    {
        var student = _studentsService.GetStudentById(id);
        if (student == null)
        {
            return NotFound();
        }
        return View(new StudentViewModel
        {
            Id = student.Id,
            Name = student.Name,
            Number = student.Number,
        });
    }
    
    [HttpPost]
    public IActionResult Create(StudentInputModel student)
    {
        _studentsService.AddStudent(student);
        return RedirectToAction("Index");
    }


    public IActionResult Delete(string id)
    {
        var student = _studentsService.GetStudentById(id);
        if (student == null)
        {
            return NotFound();
        }

        return View(new StudentViewModel
            {
                Id = student.Id,
                Name = student.Name,
                Number = student.Number,
            }
        );
    }
    
    
    [HttpPost]
    public IActionResult PostDelete(string id)
    {
        _studentsService.DeleteStudent(id);
        return RedirectToAction("Index");
    }


    public IActionResult Edit(string id)
    {
        var student = _studentsService.GetStudentById(id);
        if (student == null)
        {
            return NotFound();
        }

        var studentViewModel = new StudentViewModel
        {
            Id = student.Id,
            Name = student.Name,
            Number = student.Number,
        };

        if (student.Project != null)
        {
            studentViewModel.Project = new ProjectViewModel
            {
                Id = student.Project.Id,
                Name = student.Project.Name,
                Description = student.Project.Description,
                RepoUrl = student.Project.RepoUrl,
            };
        }

        return View(studentViewModel);

    }
    
    [HttpPost]
    public IActionResult Edit([FromRoute]string id, StudentInputModel studentInputModel)
    {
        _studentsService.UpdateStudent(id, studentInputModel);
        return RedirectToAction("Details", new {id});
    }
}