using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Data;
using SimpleBlog.Models;

namespace SimpleBlog.Controllers;

public class ArticlesController : Controller
{
    private static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp"];
    private const long MaxImageSize = 5 * 1024 * 1024;

    private readonly IWebHostEnvironment _environment;
    private readonly ApplicationDbContext _context;

    public ArticlesController(IWebHostEnvironment environment, ApplicationDbContext context)
    {
        _environment = environment;
        _context = context;
    }

    private static readonly List<Article> _articles = new List<Article>
{
    new()
    {
        Id = Guid.NewGuid(),
        Title = "The Future of Artificial Intelligence",
        Body = "Artificial intelligence is changing the way we work, communicate, and solve problems. From intelligent assistants to automated workflows, AI tools are becoming part of everyday life. As the technology continues to evolve, businesses must find ways to use it responsibly while keeping people at the center of innovation.",
        Author = "Emma Wilson",
        CreatedAt = new DateTime(2026, 10, 10, 9, 30, 0, DateTimeKind.Utc)
    },
    new()
    {
        Id = Guid.NewGuid(),
        Title = "Why Every Developer Should Learn C#",
        Body = "C# is a versatile programming language used to build web applications, desktop software, games, and cloud services. Its modern syntax, strong type system, and extensive ecosystem make it a great choice for beginners and experienced developers alike. With .NET, developers can build applications that run across multiple platforms.",
        Author = "Daniel Carter",
        CreatedAt = new DateTime(2026, 10, 9, 14, 15, 0, DateTimeKind.Utc)
    },
    new()
    {
        Id = Guid.NewGuid(),
        Title = "Simple Habits for a More Productive Day",
        Body = "Productivity does not always require complicated systems or long working hours. Planning your priorities, taking regular breaks, and limiting distractions can make a significant difference. Start with a short list of important tasks each morning and focus on completing them one at a time.",
        Author = "Olivia Martin",
        CreatedAt = new DateTime(2026, 10, 8, 8, 45, 0, DateTimeKind.Utc)
    },
    new()
    {
        Id = Guid.NewGuid(),
        Title = "A Beginner's Guide to Cloud Computing",
        Body = "Cloud computing allows individuals and organizations to access computing resources over the internet. Instead of maintaining physical servers, teams can use cloud platforms to host applications, store files, and process data. This approach offers flexibility, scalability, and new opportunities for innovation.",
        Author = "James Anderson",
        CreatedAt = new DateTime(2026, 10, 7, 11, 20, 0, DateTimeKind.Utc)
    },
    new()
    {
        Id = Guid.NewGuid(),
        Title = "The Art of Writing Clean Code",
        Body = "Clean code is easier to understand, test, and maintain. Meaningful variable names, small methods, and clear responsibilities help developers communicate their intentions through code. Although writing clean code takes practice, the long-term benefits often outweigh the initial effort.",
        Author = "Sophia Bennett",
        CreatedAt = new DateTime(2026, 10, 6, 16, 0, 0, DateTimeKind.Utc)
    },
    new()
    {
        Id = Guid.NewGuid(),
        Title = "How Remote Work Is Changing Teams",
        Body = "Remote work has transformed how teams collaborate and organize their schedules. Video calls, shared documents, and asynchronous communication make it possible to work from almost anywhere. Successful remote teams establish clear expectations, document decisions, and make time for informal conversations.",
        Author = "Noah Thompson",
        CreatedAt = new DateTime(2026, 10, 5, 10, 10, 0, DateTimeKind.Utc)
    },
    new()
    {
        Id = Guid.NewGuid(),
        Title = "Getting Started with Web APIs",
        Body = "Web APIs allow different applications to exchange information through well-defined endpoints. Developers commonly use HTTP methods such as GET, POST, PUT, and DELETE to work with resources. A well-designed API provides predictable responses, useful error messages, and consistent data formats.",
        Author = "Ava Richardson",
        CreatedAt = new DateTime(2026, 10, 4, 13, 40, 0, DateTimeKind.Utc)
    },
    new()
    {
        Id = Guid.NewGuid(),
        Title = "The Importance of Digital Privacy",
        Body = "Digital privacy has become an important consideration as more personal information is shared online. Using strong passwords, enabling multi-factor authentication, and reviewing application permissions can reduce unnecessary risks. Individuals and organizations should understand how their data is collected, stored, and used.",
        Author = "Lucas Mitchell",
        CreatedAt = new DateTime(2026, 10, 3, 7, 50, 0, DateTimeKind.Utc)
    },
    new()
    {
        Id = Guid.NewGuid(),
        Title = "Building Your First Personal Website",
        Body = "A personal website is a useful way to showcase projects, experience, and interests. Start with a simple homepage, an introduction, and a portfolio section. Focus on readable typography, responsive layouts, and straightforward navigation before adding advanced features.",
        Author = "Mia Cooper",
        CreatedAt = new DateTime(2026, 10, 2, 15, 25, 0, DateTimeKind.Utc)
    },
    new()
    {
        Id = Guid.NewGuid(),
        Title = "Understanding Database Indexes",
        Body = "Database indexes help queries find records without scanning every row in a table. They can significantly improve read performance, especially when filtering or sorting large datasets. However, indexes also require storage and can slow down writes, so they should be chosen according to actual query patterns.",
        Author = "Ethan Brooks",
        CreatedAt = new DateTime(2026, 10, 1, 12, 5, 0, DateTimeKind.Utc)
    },
    new()
    {
        Id = Guid.NewGuid(),
        Title = "Why Reading Still Matters",
        Body = "Reading encourages curiosity, strengthens vocabulary, and introduces readers to different perspectives. Whether you prefer fiction, biographies, or technical books, regular reading can become a rewarding habit. Even fifteen minutes a day can create opportunities to learn something new.",
        Author = "Grace Parker",
        CreatedAt = new DateTime(2026, 9, 28, 9, 0, 0, DateTimeKind.Utc)
    },
    new()
    {
        Id = Guid.NewGuid(),
        Title = "Exploring the World of Open Source",
        Body = "Open-source software allows developers to inspect, use, and contribute to publicly available code under its license terms. Communities improve projects by fixing bugs, adding features, and sharing knowledge. Contributing to an open-source project can also help developers build practical skills and collaborate with others.",
        Author = "Benjamin Scott",
        CreatedAt = new DateTime(2026, 9, 25, 17, 35, 0, DateTimeKind.Utc)
    },
    new()
    {
        Id = Guid.NewGuid(),
        Title = "Designing Better User Experiences",
        Body = "A good user experience makes an application easy to understand and pleasant to use. Clear labels, consistent layouts, useful feedback, and accessible controls help users complete tasks with confidence. Testing designs with real users often reveals problems that developers might otherwise overlook.",
        Author = "Lily Foster",
        CreatedAt = new DateTime(2026, 9, 22, 10, 45, 0, DateTimeKind.Utc)
    },
    new()
    {
        Id = Guid.NewGuid(),
        Title = "The Basics of Cybersecurity",
        Body = "Cybersecurity focuses on protecting systems, networks, and information from unauthorized access and attacks. Regular updates, secure authentication, backups, and employee awareness are important parts of a security strategy. Even small improvements can reduce exposure to common threats.",
        Author = "Henry Adams",
        CreatedAt = new DateTime(2026, 9, 20, 14, 50, 0, DateTimeKind.Utc)
    },
    new()
    {
        Id = Guid.NewGuid(),
        Title = "How to Build Better Learning Habits",
        Body = "Learning effectively requires more than simply spending hours with a textbook. Short study sessions, active recall, and spaced repetition can help information stay memorable. Setting realistic goals and reviewing progress regularly makes it easier to maintain motivation over time.",
        Author = "Chloe Evans",
        CreatedAt = new DateTime(2026, 9, 18, 8, 15, 0, DateTimeKind.Utc)
    },
    new()
    {
        Id = Guid.NewGuid(),
        Title = "Why Automated Testing Saves Time",
        Body = "Automated tests help developers verify that an application behaves as expected after changes are made. Unit tests check individual components, while integration tests examine how different parts work together. A reliable test suite can catch regressions early and make refactoring less stressful.",
        Author = "Alexander Reed",
        CreatedAt = new DateTime(2026, 9, 15, 11, 30, 0, DateTimeKind.Utc)
    },
    new()
    {
        Id = Guid.NewGuid(),
        Title = "Finding Inspiration in Everyday Life",
        Body = "Creative ideas often emerge from ordinary experiences. A conversation, a walk through a new neighborhood, or an unexpected challenge can inspire a fresh perspective. Keeping notes and allowing time for reflection can help transform small observations into meaningful projects.",
        Author = "Ella Morgan",
        CreatedAt = new DateTime(2026, 9, 12, 16, 20, 0, DateTimeKind.Utc)
    },
    new()
    {
        Id = Guid.NewGuid(),
        Title = "The Benefits of Regular Exercise",
        Body = "Regular physical activity can support energy levels, mobility, and overall well-being. Finding an activity you enjoy makes it easier to build a consistent routine. Walking, cycling, swimming, and strength training are all options that can be adapted to different preferences and abilities.",
        Author = "Jack Turner",
        CreatedAt = new DateTime(2026, 9, 10, 7, 40, 0, DateTimeKind.Utc)
    },
    new()
    {
        Id = Guid.NewGuid(),
        Title = "A Practical Introduction to Git",
        Body = "Git helps developers track changes to files and collaborate on software projects. Commits record snapshots of work, branches support independent development, and merges bring changes together. Learning a few essential commands can make everyday development more organized and reliable.",
        Author = "Amelia Collins",
        CreatedAt = new DateTime(2026, 9, 7, 13, 10, 0, DateTimeKind.Utc)
    },
    new()
    {
        Id = Guid.NewGuid(),
        Title = "Creating a Balanced Digital Lifestyle",
        Body = "Digital tools make communication and entertainment more accessible, but constant notifications can interrupt concentration. Setting screen-time boundaries, disabling unnecessary alerts, and making space for offline activities can help create a healthier balance. The goal is to use technology intentionally rather than automatically.",
        Author = "William Hayes",
        CreatedAt = new DateTime(2026, 9, 4, 18, 5, 0, DateTimeKind.Utc)
    }
};
    public IActionResult Index()
    {
        var articles = _context.Articles.ToList();
        return View(articles);
    }
    
    [HttpGet]
    public IActionResult Create()
    {
       ViewData["Categories"] = new List<string> { "България", "Студенти", "Левски/ЦСКА", "Унибит", "Mac vs PC", "Технологии" };
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(Article article, IFormFile? image)
    {
        if (image is { Length: > 0 })
        {
            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!AllowedImageExtensions.Contains(extension))
            {
                ModelState.AddModelError(string.Empty, "Позволени са само JPG, PNG, GIF и WEBP изображения.");
            }
            else if (image.Length > MaxImageSize)
            {
                ModelState.AddModelError(string.Empty, "Изображението трябва да е до 5 MB.");
            }
            else if (ModelState.IsValid)
            {
                var uploadsDir = Path.Combine(_environment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsDir);

                var fileName = $"{Guid.NewGuid()}{extension}";
                await using var stream = System.IO.File.Create(Path.Combine(uploadsDir, fileName));
                await image.CopyToAsync(stream);

                article.ImagePath = $"/uploads/{fileName}";
            }
        }

        if (ModelState.IsValid)
        {
            article.Id = Guid.NewGuid();
            _context.Articles.Add(article);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        return View(article);
    }

    public IActionResult Details(Guid id)
    {
        Article? article = _context.Articles.FirstOrDefault(a => a.Id == id);
        if (article == null)
        {
            return NotFound();
        }
        ViewData["Title"] = $"Details for + {article.Title}";
        ViewBag.ArticleCount = 60;
        return View(article);
            
    }
    
}