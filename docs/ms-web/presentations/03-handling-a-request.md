# Session 03 — Handling a Request

> Syllabus mapping: Lecture 2 (controllers, actions) + the MVC overview from Lecture 1
> · ~180 min

## Learning objectives

By the end of this session students will be able to:

1. Explain separation of concerns and describe the responsibility of each MVC component.
2. Trace the lifecycle of a request through an ASP.NET Core MVC application.
3. Write a controller with several action methods.
4. Return different result types from an action and choose the right one.
5. Accept parameters from the URL path and the query string.
6. Return a correct 404 instead of crashing when a resource does not exist.

## Prerequisites / recap

- Session 01: request/response, methods, status codes.
- Session 02: C# classes, methods, `List<T>`, LINQ, a running `SimpleBlog` project.
- Quick recap question to open with: *"A request arrives at our server for `/articles/42`.
  Somebody has to decide what to do about it. Today, that somebody gets a name."*

## Agenda

| # | Block | Min | Mode |
|---|-------|-----|------|
| 1 | Recap + objectives | 10 | talk |
| 2 | Separation of concerns; the MVC pattern | 30 | talk |
| 3 | The lifecycle of a request | 15 | talk |
| 4 | *Break* | 10 | — |
| 5 | Controllers and actions in ASP.NET Core MVC | 25 | talk + demo |
| 6 | Returning results; getting parameters in | 25 | demo |
| 7 | *Break* | 10 | — |
| 8 | Demo: `ArticlesController` end to end | 10 | demo |
| 9 | Lab: build your own controller | 40 | lab |
| 10 | Wrap-up, homework, Q&A | 5 | talk |

---

## Part 1 — The idea (vendor-neutral) · ~45 min

### 3.1 The problem: one file that does everything

Show (on a slide, as pseudo-code — no framework, no language commitment) what a naïve
page handler looks like when nobody has thought about structure:

```
when a request arrives for /articles/42:
    open the database connection
    run "SELECT * FROM articles WHERE id = 42"
    write "<html><body><h1>" + row.title + "</h1>"
    if user is admin: write "<a href=...>Edit</a>"
    write "</body></html>"
    close the connection
```

Ask the room what is wrong with it. Guide towards:

- The designer who wants to change the layout must edit code that talks to the database.
- The same "is this user an admin" rule is copy-pasted into 40 files.
- You cannot test the logic without a database and a browser.
- You cannot reuse anything: the data-fetching is welded to the HTML.

**Separation of concerns** is the principle: *each part of a program should have one
reason to change.* Layout changes should touch layout code only. Rules changes should
touch rules code only.

### 3.2 MVC as an answer

MVC (Model–View–Controller) is one very common way to apply that principle to a
user-facing application. It predates the web by a decade — it is not a Microsoft idea, and
you will meet it in Java (Spring MVC), Ruby (Rails), Python (Django, with different
names), PHP (Laravel) and elsewhere.

| Component | Responsibility | It must **not** |
|---|---|---|
| **Model** | The data and the rules about that data. "An article has a title; a title is required." | Know that HTML or HTTP exist. |
| **View** | Presentation. Turns data into what the user sees. | Fetch data or make decisions. |
| **Controller** | Receives the request, decides what to do, gathers the data, picks the view. | Contain layout, or the deep business rules. |

**Analogy — the restaurant, continued from Session 01.** The *waiter* (controller) takes
your order, decides it is valid, passes it to the *kitchen* (model) where the food is
actually made, and hands the finished plate to the *presentation* — plating, garnish,
which table it goes to (view). The waiter does not cook. The chef does not choose the
napkin colour.

**The traffic rule that makes it work:**

```
Request → Controller → (asks) Model → (data) → Controller → (hands data to) View → Response
```

Views never talk to the database. Models never write HTML. Everything passes through the
controller. If a student can state that rule at the end of the course, the course worked.

**Benefits**, stated plainly:
- Change the design without touching logic, and vice versa.
- Two people can work on the same feature at once.
- Logic can be tested without a browser.
- New developers know where to look — the structure tells them.

**The honest cost:** more files. A "hello world" is 3 files instead of 1. The payoff
arrives at about file 20.

### 3.3 The lifecycle of a request

Draw this on the board and keep it visible all session — refer back to it whenever
something is confusing:

```
1. Request arrives                 GET /articles/42
2. Middleware pipeline             logging, static files, HTTPS redirect, ...
3. Routing                         "which controller + action handles this path?"   → Session 04
4. Controller is created           the framework constructs it for this one request
5. Model binding                   URL/query/form values → action method parameters
6. Action method runs              YOUR code: decide, fetch, choose
7. A result is returned            a view, a redirect, a 404, raw data
8. View is rendered (if any)       template + data → HTML                            → Session 05
9. Response is written             status line + headers + body                      → Session 01
```

Two things to stress:

- **Steps 1–5 and 7–9 are the framework's job. Step 6 is yours.** That is the inversion
  from Session 02: you fill in the interesting part, the framework does the rest.
- A **new controller instance is created for every request** and thrown away afterwards.
  Do not store anything in it expecting it to survive — the web is stateless (Session 01).

---

## Part 2 — In practice: .NET 10 MVC · ~50 min

### 3.4 What a controller looks like

```csharp
// Controllers/ArticlesController.cs
using Microsoft.AspNetCore.Mvc;

namespace SimpleBlog.Controllers;

public class ArticlesController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
```

Every line is a convention worth naming:

| Element | Rule | Consequence if broken |
|---|---|---|
| Location | in the `Controllers/` folder | (convention only — works elsewhere, but don't) |
| Class name | ends with `Controller` | not discovered as a controller |
| Base class | inherits `Controller` | loses `View()`, `RedirectToAction()`, `NotFound()`, … |
| Access | `public` | not reachable |
| Action | a `public` method | not reachable as an action |

**The name-stripping rule:** the class `ArticlesController` is reached at the URL segment
`/Articles`. The word `Controller` is removed. This trips people up once, then never
again.

### 3.5 Actions and what they return

An **action** is a public method on a controller. It is the unit of "one thing the app
can do".

`IActionResult` is a *promise to produce a response* — it lets one method return different
kinds of outcome. The common ones:

```csharp
public IActionResult Index()      => View();                       // 200 + rendered HTML
public IActionResult Missing()    => NotFound();                   // 404
public IActionResult Denied()     => Forbid();                     // 403
public IActionResult Old()        => RedirectToAction("Index");    // 302 to another action
public IActionResult Raw()        => Content("plain text");        // 200 text/plain
public IActionResult Data()       => Json(new { id = 1 });         // 200 application/json
```

Tie every single one back to Session 01: *these are the status codes from week one; you
are now the one choosing them.* Show each in DevTools as you go.

`View()` overloads worth knowing today:

```csharp
return View();                    // renders Views/Articles/Index.cshtml  (by action name)
return View("Details");           // renders Views/Articles/Details.cshtml
return View(article);             // same view, with data attached       → Session 05/06
```

> The views do not exist yet. Running this now gives a helpful error listing the paths it
> searched — **show that error deliberately**. It teaches the convention better than a
> slide does. We create the views in Session 05; today we return `Content(...)` so we can
> see results immediately without views.

### 3.6 Getting data *into* an action

**Model binding** is the framework filling your method's parameters from the request.
Name matching is all it takes — the parameter name must match the route value or query
key.

```csharp
// GET /Articles/Details/42          (route value 'id')
public IActionResult Details(int id) => Content($"Article {id}");

// GET /Articles/Search?term=http&page=2   (query string)
public IActionResult Search(string term, int page = 1)
    => Content($"Searching '{term}', page {page}");

// GET /Articles/Filter?author=Ada         (optional value)
public IActionResult Filter(string? author)
    => Content(author is null ? "All articles" : $"By {author}");
```

Points to land:

- The **name** is the contract: `?term=http` fills `string term`.
- **Types are converted for you**: `?page=2` becomes `int 2`. If it cannot convert
  (`?page=abc`), you get `0` and a model-state error — validation is Session 09.
- **Default values** make a parameter optional.
- `string?` marks a value that may legitimately be missing.
- Where values can come from: route path, query string, and (Session 09) the form body.

### 3.7 Demo: `ArticlesController` end to end

Build it live, in this order, running after each step:

```csharp
using Microsoft.AspNetCore.Mvc;

namespace SimpleBlog.Controllers;

public class ArticlesController : Controller
{
    // A temporary in-memory list. In Session 07 this becomes a real database.
    private static readonly List<Article> _articles = new()
    {
        new Article { Id = 1, Title = "How the web works",  Author = "Ada",   PublishedOn = new DateTime(2026, 1, 10) },
        new Article { Id = 2, Title = "Getting started",    Author = "Grace", PublishedOn = new DateTime(2026, 2, 2)  },
        new Article { Id = 3, Title = "Controllers 101",    Author = "Ada",   PublishedOn = new DateTime(2026, 3, 1)  },
    };

    // GET /Articles
    public IActionResult Index()
        => Content(string.Join("\n", _articles.Select(a => $"{a.Id}: {a.Title}")));

    // GET /Articles/Details/2
    public IActionResult Details(int id)
    {
        var article = _articles.FirstOrDefault(a => a.Id == id);
        if (article is null)
            return NotFound();

        return Content($"{article.Title} — {article.Author}");
    }

    // GET /Articles/ByAuthor?author=Ada
    public IActionResult ByAuthor(string? author)
    {
        var matches = string.IsNullOrWhiteSpace(author)
            ? _articles
            : _articles.Where(a => a.Author == author).ToList();

        return Content($"{matches.Count} article(s)");
    }

    // GET /Articles/About
    public IActionResult About() => Content("A tiny blog built during this course.");
}
```

with, in `Models/Article.cs`:

```csharp
namespace SimpleBlog.Models;

public class Article
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public DateTime PublishedOn { get; set; }
}
```

**The teaching moment is `Details`.** Show both versions:

```csharp
var article = _articles.First(a => a.Id == id);   // id=999 → exception → 500
var article = _articles.FirstOrDefault(...);      // id=999 → null → we return 404
```

Run both with `/Articles/Details/999` and show the status code in DevTools. This is
Session 01's "whose bug is it?" made concrete, and it justifies `FirstOrDefault` from
Session 02. *A missing article is not a crash — it is a 404.*

Then show `dotnet watch` picking up each change, and hit every action in the browser with
DevTools open, reading the status codes aloud.

---

## Live demo checkpoints

| # | State | How to reach it |
|---|---|---|
| 1 | `Models/Article.cs` created | copy the class above |
| 2 | Empty `ArticlesController` responding at `/Articles` | `Index` returning `Content("hi")` |
| 3 | The "view not found" error shown deliberately | change `Index` to `return View();` and reload |
| 4 | `Details(int id)` working for `/Articles/Details/2` | model binding from the route |
| 5 | `/Articles/Details/999` returning **404**, not 500 | swap `First` → `FirstOrDefault` + `NotFound()` |
| 6 | `ByAuthor?author=Ada` reading the query string | model binding from the query |
| 7 | Status codes verified in DevTools | 200 / 404 / 302 side by side |

**SimpleBlog state after this session:** `Models/Article.cs` plus an `ArticlesController`
with `Index`, `Details`, `ByAuthor` and `About`, backed by a static in-memory list, all
returning plain text. No views yet.

## Common misconceptions

| Misconception | Correction |
|---|---|
| "MVC is a Microsoft technology." | It is a 1970s UI pattern used across every major web ecosystem. |
| "The Model is the database." | The model is the data and its rules. It may be *stored* in a database — a separate concern. |
| "The controller draws the page." | It chooses a view and hands it data. Rendering is the view's job. |
| "The class name in the URL includes `Controller`." | `ArticlesController` is reached at `/Articles`. |
| "One controller per page." | One controller per *area of responsibility*, with many actions. |
| "The controller object lives as long as the app." | A fresh instance per request, discarded after. |
| "A missing record should throw." | A missing record is a 404. An exception is a 500 — a different meaning entirely. |
| "Parameters are matched by position." | They are matched **by name**. |

## Check for understanding

1. State the traffic rule of MVC in one sentence. Which component is allowed to talk to
   which?
2. A request for `/Articles/Details/7` arrives. List the steps between "the bytes reach
   the server" and "HTML is sent back". Which one is your code?
3. `ArticlesController.About()` — what URL reaches it, with the default route?
4. Two students write `Details(int id)` and `Details(int articleId)`. `/Articles/Details/7`
   works for one and not the other. Why?
5. A user requests an article that was deleted. What should the status code be, and which
   LINQ operator makes that easy to implement?
6. Why does a fresh controller instance per request make sense, given Session 01?

## Lab task (in class) · 40 min

Working in your own `SimpleBlog`:

1. Create `Models/Article.cs` with `Id`, `Title`, `Author`, `Summary`, `PublishedOn`.
2. Create `Controllers/ArticlesController.cs` with a `static List<Article>` of at least
   five articles by at least two different authors.
3. Implement these actions, each returning `Content(...)` for now:
   - `Index()` — a numbered list of all titles.
   - `Details(int id)` — one article's full details; **404** when the id does not exist.
   - `ByAuthor(string? author)` — articles by that author; all of them when the parameter
     is missing. Use LINQ.
   - `Recent(int days = 7)` — articles published within the last *n* days.
   - `About()` and `Contact()` — static text.
4. Visit every action in the browser and record its status code from DevTools.
5. Make `Details` deliberately fail with a 500 (use `First`), observe it, then fix it back
   to a 404 and note the difference in what the user sees.

**Acceptance criteria:** all six actions respond at the expected URLs; `Details` with an
unknown id returns 404 (verified in DevTools, not just "an error page"); `ByAuthor` and
`Recent` are implemented with LINQ and behave correctly when their parameter is omitted.

## Homework

1. Add `Search(string? term)` that matches `term` case-insensitively against both `Title`
   and `Summary`, and reports how many matches were found. Decide and justify: what should
   an empty `term` do?
2. Add `Count()` returning `Json(new { total = ... })`. Open it in the browser and note
   the `Content-Type` header — how does it differ from your other actions?
3. Write, in three sentences, what would have to change in your controller if the articles
   came from a database instead of a list. (Keep this answer — we check it in Session 07.)
4. Read the "view was not found" error message again and write down every path the
   framework searched. What does that tell you about where views must live?

## Glossary (EN → BG)

| English | Български |
|---|---|
| separation of concerns | разделяне на отговорностите |
| pattern (design) | шаблон / модел за проектиране |
| controller | контролер |
| action (method) | действие / екшън метод |
| request lifecycle | жизнен цикъл на заявката |
| middleware / pipeline | междинен слой / конвейер |
| model binding | обвързване на модела |
| convention over configuration | конвенция вместо конфигурация |
| to inherit / base class | наследяване / базов клас |
| instance (per request) | инстанция (за всяка заявка) |
| redirect | пренасочване |

## References

- ASP.NET Core MVC overview: https://learn.microsoft.com/aspnet/core/mvc/overview
- Handling requests with controllers: https://learn.microsoft.com/aspnet/core/mvc/controllers/actions
- Action return types: https://learn.microsoft.com/aspnet/core/web-api/action-return-types
- Model binding: https://learn.microsoft.com/aspnet/core/mvc/models/model-binding
