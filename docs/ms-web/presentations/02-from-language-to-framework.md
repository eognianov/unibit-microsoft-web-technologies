# Session 02 — From Language to Framework

> Syllabus mapping: Lecture 1 (.NET / C# / tooling / first project) · ~180 min
> **The densest session of the course.** It carries the entire "may never have
> programmed" load. Pace it ruthlessly; the lab is where the learning happens.

## Learning objectives

By the end of this session students will be able to:

1. Explain what a programming language, a runtime, an SDK and a framework each are, and
   why frameworks exist.
2. Read and write basic C#: variables, types, conditionals, loops, collections, methods,
   classes and properties.
3. Handle an error with `try / catch / finally`.
4. Query a collection with `Where`, `Select`, `OrderBy` and `FirstOrDefault`.
5. Create and run an ASP.NET Core MVC project from the command line.
6. Name the purpose of each top-level folder and file in the generated project.

## Prerequisites / recap

- Session 01: client–server, request/response, HTTP methods, status codes.
- **.NET 10 SDK installed** (homework). Verify at the door: `dotnet --version`.
- Have a fallback for students who failed to install: pair them up, and point them at
  https://dotnet.microsoft.com/platform/try-dotnet for the C# part.

## Agenda

| # | Block | Min | Mode |
|---|-------|-----|------|
| 1 | Recap + objectives; install check | 10 | talk |
| 2 | Language, runtime, SDK, framework — the four words | 20 | talk |
| 3 | C# essentials I: values, decisions, repetition | 30 | talk + live coding |
| 4 | *Break* | 10 | — |
| 5 | C# essentials II: collections, methods, classes, exceptions | 35 | talk + live coding |
| 6 | Querying collections: the four LINQ operators | 15 | live coding |
| 7 | *Break* | 10 | — |
| 8 | Demo: `dotnet new mvc` — creating and running SimpleBlog | 15 | demo |
| 9 | Lab: create, run and explore your own project | 30 | lab |
| 10 | Wrap-up, homework, Q&A | 5 | talk |

---

## Part 1 — The idea (vendor-neutral) · ~100 min

### 2.1 Four words people confuse

Introduce these with no branding at all, in this order:

| Word | What it is | Analogy |
|---|---|---|
| **Language** | The rules for writing instructions a computer can be made to follow. | A human language — grammar and vocabulary. |
| **Runtime** | The program that actually *executes* your instructions on a real machine. | The engine. Your code is the driver's input. |
| **SDK** | Software Development Kit: the tools you need to *build* software — compiler, project templates, command-line tools. Includes a runtime. | A workshop: bench, tools, and a spare engine to test with. |
| **Framework** | A large body of pre-written code that solves the common problems of a *kind* of application, so you only write the parts unique to yours. | A house's foundation, plumbing and wiring. You decorate the rooms. |

**Why frameworks exist.** Ask the room: after Session 01, what would you have to build
yourself to serve a web page?

- Listen on a network port
- Read raw bytes and parse them as an HTTP request
- Work out which of your code should handle this path
- Read files from disk, produce HTML
- Format an HTTP response and write the bytes back
- Handle 1000 people doing this at once, safely
- Do all of it without crashing, and log it when it does

*"None of that is your blog. All of it is the same for every website ever built. A web
framework is that list, already solved, tested by millions of users."*

**The inversion.** With a *library*, you call the code. With a *framework*, the framework
calls **your** code. It owns the main loop; you fill in the interesting parts. This is
why learning a framework is mostly learning *where to put your code* and *what names to
give it* — a theme for the whole course.

**The cost.** Frameworks impose conventions. Follow them and you write very little code.
Fight them and you write a lot. We will follow them.

### 2.2 Now the names: .NET, C#, ASP.NET Core

Map the four words onto the actual stack:

- **C#** — the language. Statically typed, object-oriented, general purpose.
- **.NET** — the platform: the runtime that executes C#, plus a large standard library.
  Cross-platform (Windows, macOS, Linux). Current version: **.NET 10**.
- **.NET SDK** — what you installed. Gives you the `dotnet` command.
- **ASP.NET Core** — the web framework that runs on .NET.
- **ASP.NET Core MVC** — one particular way of structuring a web app with ASP.NET Core.
  That structure is Session 03's topic.

Two commands to show immediately:

```bash
dotnet --version      # which SDK am I running?
dotnet --info         # SDKs, runtimes, OS — the thing to paste when asking for help
```

> Why C# for the web? Statically typed (the compiler catches whole classes of mistakes
> before you run), mature tooling, first-class database support, and it is what industry
> in this region hires for. But note honestly: the *concepts* here are the same in Java,
> Python, PHP and JavaScript. You are not learning one company's product; you are
> learning web development, in one dialect.

### 2.3 C# essentials I — live coding

Work in a scratch console project, projected, typing as you talk. Create it once:

```bash
dotnet new console -n CsBasics -f net10.0
cd CsBasics
dotnet run
```

Show the whole file first — it is three lines — then explain that .NET 10 lets you write
statements at the top level without ceremony, and that we will meet the ceremony later.

**Values and types**

```csharp
string title = "Hello, world";
int views = 42;
double rating = 4.5;
bool isPublished = true;
DateTime publishedOn = new DateTime(2026, 3, 1);

var author = "Ada";        // the compiler infers 'string' — same thing, less typing
```

Points to make:
- A **variable** is a named box. A **type** says what may go in the box.
- C# is **statically typed**: `views = "many";` will not compile. Demonstrate the error —
  the red squiggle is a friend, not a failure.
- `var` is not "no type", it is "you work it out".

**String interpolation** (used constantly for the rest of the course):

```csharp
Console.WriteLine($"{title} by {author}, {views} views");
```

**Decisions**

```csharp
if (views > 1000)
    Console.WriteLine("Popular");
else if (views > 100)
    Console.WriteLine("Doing fine");
else
    Console.WriteLine("Needs promotion");

string label = views > 1000 ? "hot" : "normal";

switch (status)
{
    case "draft":     Console.WriteLine("Not visible"); break;
    case "published": Console.WriteLine("Live");        break;
    default:          Console.WriteLine("Unknown");     break;
}
```

**Repetition**

```csharp
for (int i = 0; i < 5; i++)
    Console.WriteLine($"Article {i}");

int n = 0;
while (n < 3) { Console.WriteLine(n); n++; }

foreach (var word in new[] { "web", "http", "csharp" })
    Console.WriteLine(word);
```

> `foreach` is the one they will use in views for the rest of the course. Spend the extra
> minute on it.

### 2.4 C# essentials II — live coding

**Collections**

```csharp
string[] tags = { "dotnet", "web", "mvc" };          // fixed size
tags[0] = "csharp";

List<string> titles = new List<string>();            // grows
titles.Add("First post");
titles.Add("Second post");
titles.Remove("First post");
Console.WriteLine(titles.Count);

Dictionary<string, int> viewsByTitle = new()
{
    ["First post"]  = 120,
    ["Second post"] = 45
};
Console.WriteLine(viewsByTitle["First post"]);
```

Frame them by *the question they answer*:
- **Array** — "a fixed row of boxes, numbered from 0."
- **`List<T>`** — "a row of boxes that grows and shrinks."
- **`Dictionary<K,V>`** — "a lookup table: give me the value filed under this key."

The `<T>` notation deserves a sentence: *"`List<string>` is a list **of strings**. The
angle brackets say what kind of thing is inside."*

**Methods**

```csharp
string Shorten(string text, int maxLength)
{
    if (text.Length <= maxLength) return text;
    return text.Substring(0, maxLength) + "...";
}

Console.WriteLine(Shorten("A very long article title indeed", 10));
```

- A method is a named, reusable block: **inputs (parameters)** → **output (return type)**.
- `void` means "returns nothing".
- Why: name a thing once, use it everywhere; fix it in one place.

**Classes, objects and properties** — the concept the whole rest of the course rests on:

```csharp
class Article
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public DateTime PublishedOn { get; set; }

    public bool IsRecent() => PublishedOn > DateTime.Now.AddDays(-7);
}

var a = new Article { Id = 1, Title = "Hello", Author = "Ada", PublishedOn = DateTime.Now };
Console.WriteLine($"{a.Title} — recent? {a.IsRecent()}");
```

- A **class** is a blueprint; an **object** is one thing built from it.
- A **property** is a named piece of data on that thing (`{ get; set; }` = readable and
  writable).
- Analogy: `Article` is the *form*; each filled-in form is an object.
- This exact class becomes our database table in Session 08 — say so now.

**Exceptions**

```csharp
try
{
    var text = File.ReadAllText("missing.txt");
    Console.WriteLine(text);
}
catch (FileNotFoundException ex)
{
    Console.WriteLine($"No such file: {ex.Message}");
}
finally
{
    Console.WriteLine("This always runs.");
}
```

- Some failures cannot be prevented: the file is gone, the network is down, the user
  typed nonsense. An **exception** is the language's way of saying "I cannot continue
  here."
- Unhandled, it stops the program — and in a web app, that is the **500** from
  Session 01. Draw the connection explicitly.
- Do **not** catch what you cannot handle. An empty `catch { }` hides bugs; it does not
  fix them.

### 2.5 Querying collections — the four LINQ operators

Motivate first: *"Given a list of 500 articles, show me this year's, newest first, titles
only."* Write it with loops (10+ lines, on the board), then:

```csharp
var articles = new List<Article> { /* ... */ };

var recent   = articles.Where(a => a.PublishedOn.Year == 2026);
var titles   = articles.Select(a => a.Title);
var ordered  = articles.OrderBy(a => a.PublishedOn);
var first    = articles.FirstOrDefault(a => a.Author == "Ada");

var result = articles
    .Where(a => a.PublishedOn.Year == 2026)
    .OrderByDescending(a => a.PublishedOn)
    .Select(a => a.Title)
    .ToList();
```

| Operator | Question it answers |
|---|---|
| `Where` | which ones match? (filter — same count or fewer) |
| `Select` | what do I want *from* each one? (transform — same count) |
| `OrderBy` / `OrderByDescending` | in what order? |
| `FirstOrDefault` | give me one, or `null` if there is none |

The lambda `a => a.Title` needs one careful sentence: *"read the arrow as 'goes to'. For
each article `a`, go to its `Title`. It is a tiny method without a name."*

Two warnings that pay off later:
- `First` throws when nothing matches; `FirstOrDefault` returns `null`. Prefer the latter
  and check it — this is exactly the 404-vs-500 decision in Session 03.
- The same four operators will run against a **database** in Session 08, unchanged. That
  is the payoff for learning them now.

---

## Part 2 — In practice: the first MVC project · ~15 min

### 2.6 Creating SimpleBlog

```bash
dotnet new mvc -n SimpleBlog -f net10.0
cd SimpleBlog
dotnet run
```

Read the output aloud: the listening URL, the environment, the content root. Open the URL
in the browser. **A website, in three commands, with no code written.** That is the
framework from §2.1 doing its job.

Then, with DevTools open (Session 01 muscle memory): show the document request, its
200, its `Content-Type`, and the HTML in the Response panel. *"That HTML did not exist as
a file. Our program produced it."*

Also show `dotnet watch` — it rebuilds and reloads on save; students should use it all
course:

```bash
dotnet watch
```

### 2.7 What is in the box

Tour the folders, saying **who is responsible for what** and which session covers it:

| Path | What it is | Covered in |
|---|---|---|
| `Program.cs` | The starting point. Builds the app, registers services, defines the pipeline, runs it. | 03, 04, 07 |
| `Controllers/` | The classes that handle requests. | 03 |
| `Views/` | The HTML templates. | 05 |
| `Models/` | The data shapes. | 06, 07 |
| `wwwroot/` | Static files served as-is: CSS, JS, images. | 05, 06 |
| `appsettings.json` | Configuration: settings, connection strings. | 07 |
| `SimpleBlog.csproj` | The project file: target framework and package references. | — |
| `obj/`, `bin/` | Build output. Never edit; never commit. | — |

Then open `Program.cs` and read it as prose, not as code:

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();   // "I am an MVC app"

var app = builder.Build();

app.UseStaticFiles();                          // "serve wwwroot as-is"
app.UseRouting();                              // "work out who handles this URL"

app.MapControllerRoute(                        // "here is the URL shape"
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();                                     // "start listening"
```

Say plainly: *"You are not expected to understand every line today. You are expected to
know that this file is where the app is assembled, and that we will come back to each
line as we need it."* Point at `MapControllerRoute` — "Session 04 is entirely about that
one string."

---

## Live demo checkpoints

| # | State | How to reach it |
|---|---|---|
| 1 | Scratch console project running | `dotnet new console -n CsBasics -f net10.0` → `dotnet run` |
| 2 | C# basics demonstrated live | types → control flow → collections → class → try/catch → LINQ |
| 3 | SimpleBlog created | `dotnet new mvc -n SimpleBlog -f net10.0` |
| 4 | SimpleBlog running in the browser | `dotnet run`, open the printed URL |
| 5 | Hot reload working | `dotnet watch`, edit `Views/Home/Index.cshtml`, save, browser updates |
| 6 | Project structure toured | walk the folder table above |

**SimpleBlog state after this session:** the untouched `dotnet new mvc` template, running
locally. No custom controllers, no data, no database.

## Common misconceptions

| Misconception | Correction |
|---|---|
| "C# and .NET are the same thing." | C# is the language; .NET is the platform that runs it. F# and VB also run on .NET. |
| "I need Visual Studio / Windows." | The SDK and the `dotnet` CLI are the whole toolchain. Any editor, any OS. |
| "`var` means the variable has no type." | It has exactly one type; the compiler infers it. |
| "A compiler error means I broke something badly." | It means the compiler caught a mistake *before* it could hurt. That is the feature. |
| "A class and an object are the same." | Class = blueprint. Object = one thing built from it. |
| "`List` and array are interchangeable." | Arrays are fixed-length; `List<T>` grows. |
| "`catch` fixes the error." | It gives you a chance to respond. An empty `catch` hides bugs. |
| "LINQ is a database thing." | It queries *any* collection. It also happens to work on databases (Session 08). |
| "The template's pages are files in `wwwroot`." | They are produced by code and templates. Only CSS/JS/images live in `wwwroot`. |

## Check for understanding

1. What is the difference between the SDK and the runtime? Which do you need to *build*
   an app, and which to *run* one?
2. "With a framework, the framework calls your code." What does that mean in practice for
   how you write a web app?
3. Which collection would you use for: five fixed menu items; a growing list of article
   titles; looking up a view count by article title?
4. What does `articles.Where(a => a.Author == "Ada").Select(a => a.Title)` produce — how
   many items, and of what type?
5. `First` vs `FirstOrDefault` — which one throws when nothing matches, and why does the
   choice matter for a website?
6. Name the folder where each of these belongs: a stylesheet; a class that handles a
   request; an HTML template; the database connection string.

## Lab task (in class) · 30 min

**Part A — C# warm-up** (in the scratch console project)

1. Create a `List<Article>` with at least five articles (make up titles, authors, dates).
2. Print every article's title and author with a `foreach`.
3. Write a method `string Summarize(Article a)` returning `"Title — Author (yyyy-MM-dd)"`.
4. Using LINQ, print: only articles by one specific author; all titles ordered by date
   descending; the first article published before a given date (handle "none found"
   without crashing).

**Part B — Your own MVC project**

5. `dotnet new mvc -n SimpleBlog -f net10.0`, then `dotnet run`, and open it in a browser.
6. With DevTools open, record the status code and `Content-Type` of the main document.
7. Find `Views/Home/Index.cshtml`, change the visible heading to your own text, save with
   `dotnet watch` running, and confirm the browser updates.
8. Write down, in your own words, one sentence per item for: `Program.cs`, `Controllers/`,
   `Views/`, `wwwroot/`, `appsettings.json`.

**Acceptance criteria:** Part A runs without errors and the LINQ queries produce the right
results including the "none found" case; the MVC app runs, shows the student's own
heading, and the five one-sentence descriptions are correct.

## Homework

1. Extend `Summarize` so that titles longer than 30 characters are truncated with `"..."`
   (reuse the `Shorten` method from the lecture).
2. Add a `Dictionary<string, int>` of view counts and print the three most-viewed titles
   using LINQ.
3. Deliberately break your code three ways and write down the exact error message each
   time: use an undeclared variable; assign a string to an `int`; index a list past its
   end. Which of the three is a *compile-time* error and which is a *runtime* error?
4. Read `Program.cs` and mark every line you do not yet understand. Bring the list.

## Glossary (EN → BG)

| English | Български |
|---|---|
| language / runtime | език / среда за изпълнение |
| SDK (software development kit) | комплект за разработка |
| framework | рамка / фреймуърк |
| compiler / compile-time error | компилатор / грешка при компилация |
| runtime error | грешка по време на изпълнение |
| variable / type | променлива / тип |
| statically typed | статично типизиран |
| collection | колекция |
| method / parameter / return value | метод / параметър / връщана стойност |
| class / object / instance | клас / обект / инстанция |
| property | свойство |
| exception / to throw / to catch | изключение / хвърляне / прихващане |
| query (LINQ) | заявка |
| lambda expression | ламбда израз |
| template (project) | шаблон |
| hot reload | горещо презареждане |

## References

- .NET download and install: https://dotnet.microsoft.com/download
- C# tour of the language: https://learn.microsoft.com/dotnet/csharp/tour-of-csharp/
- `dotnet new` CLI reference: https://learn.microsoft.com/dotnet/core/tools/dotnet-new
- LINQ basics: https://learn.microsoft.com/dotnet/csharp/linq/
- ASP.NET Core project structure: https://learn.microsoft.com/aspnet/core/fundamentals/
