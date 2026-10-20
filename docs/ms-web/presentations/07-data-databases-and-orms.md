# Session 07 — Data, Databases and ORMs

> Syllabus mapping: Lecture 4 (models, ORM, EF Core setup, connection strings) · ~180 min
> Adds a short relational-database primer the syllabus omits but beginners need before an
> ORM can make any sense.

## Learning objectives

By the end of this session students will be able to:

1. Explain why data must be stored outside the running program.
2. Describe the relational model: tables, rows, columns, primary keys, foreign keys.
3. Read simple SQL: `SELECT`, `WHERE`, `ORDER BY`, `INSERT`, `UPDATE`, `DELETE`.
4. Explain what an ORM does and what it costs.
5. Configure EF Core in an ASP.NET Core app: `DbContext`, `DbSet<T>`, connection string,
   dependency injection.
6. Explain what dependency injection is and why the controller does not create its own
   `DbContext`.

## Prerequisites / recap

- Session 02: classes, properties, `List<T>`, LINQ.
- Session 03: the Model in MVC; the static in-memory list; the homework question *"what
  would change if the articles came from a database?"* — collect those answers now.
- Session 06: view models are shaped for the page, not the storage.
- Open by restarting the app after adding an article: *"Where did it go?"*

## Agenda

| # | Block | Min | Mode |
|---|-------|-----|------|
| 1 | Recap + the disappearing-data problem | 10 | talk + demo |
| 2 | Why databases; the relational model | 30 | talk |
| 3 | A working minimum of SQL | 20 | talk + demo |
| 4 | *Break* | 10 | — |
| 5 | The object–relational mismatch; what an ORM is | 20 | talk |
| 6 | EF Core: `DbContext`, `DbSet<T>`, provider, connection string | 30 | demo |
| 7 | *Break* | 10 | — |
| 8 | Dependency injection and the app's service registry | 20 | demo |
| 9 | Lab: wire up the database | 25 | lab |
| 10 | Wrap-up, homework, Q&A | 5 | talk |

---

## Part 1 — The idea (vendor-neutral) · ~70 min

### 7.1 The problem: memory forgets

Demonstrate before explaining. Add an article to the static list at runtime (or just point
at the hard-coded list), stop the app with `Ctrl+C`, restart it. Everything is back to the
starting three.

Why: a `List<Article>` lives in RAM, which belongs to the *process*. When the process ends
— a restart, a deployment, a crash, a server reboot — it is gone. And there is more:

- Two copies of your app on two servers would each have a *different* list.
- Nothing coordinates two users editing at once.
- You cannot ask questions like "how many articles per author" without scanning everything.

So we need storage that **outlives the process**, is **shared**, is **queryable**, and is
**safe under concurrent access**. Files solve the first. A **database management system**
solves all four.

### 7.2 The relational model

**Analogy — the spreadsheet, taken seriously.** A database is a set of tables. A table has
named columns of declared types, and rows of data. That much is a spreadsheet. What a
database adds is *rules that are enforced* and *questions that are answered efficiently*.

```
Articles
┌────┬────────────────────┬─────────┬──────────────┐
│ Id │ Title              │ Author  │ PublishedOn  │
├────┼────────────────────┼─────────┼──────────────┤
│  1 │ How the web works  │ Ada     │ 2026-01-10   │
│  2 │ Getting started    │ Grace   │ 2026-02-02   │
│  3 │ Controllers 101    │ Ada     │ 2026-03-01   │
└────┴────────────────────┴─────────┴──────────────┘
  ▲
  primary key
```

Vocabulary, one line each:

| Term | Meaning |
|---|---|
| **Table** | one kind of thing (articles, users, categories) |
| **Row / record** | one of those things |
| **Column / field** | one named property, with a declared type |
| **Primary key** | the column that uniquely identifies a row — never reused, never null |
| **Foreign key** | a column holding another table's primary key — how tables relate |
| **Schema** | the whole structure: tables, columns, types, keys, constraints |
| **Constraint** | a rule the database itself enforces (`NOT NULL`, `UNIQUE`, …) |

**Why not one giant table?** Show the duplication problem: storing the author's full name,
bio and photo URL on every article means updating a photo in 200 places, and one typo
creates a second author. Split into two tables, joined by a key:

```
Articles                            Authors
┌────┬───────────┬──────────┐       ┌────┬────────┬────────────┐
│ Id │ Title     │ AuthorId │──────▶│ Id │ Name   │ Bio        │
└────┴───────────┴──────────┘  FK   └────┴────────┴────────────┘
```

That is **normalisation**, stated at the level students need: *store each fact exactly
once.* Note that the relationship types (one-to-many, many-to-many) are a topic for a
database course; we need one-to-many and we will meet it in Session 08's optional
`Category`.

**Types matter.** `PublishedOn` as a real date can be sorted, compared and ranged over.
`PublishedOn` as text cannot — `"10 March"` sorts before `"2 March"`. Say this now; it
prevents a common project mistake.

**Which database?** They differ in scale and features, not in the ideas above:
SQLite (a single file, zero setup), SQL Server / PostgreSQL / MySQL / Oracle (server
processes). We use **SQLite** for the course so every operating system works identically,
and Session 07 shows how little changes to switch.

### 7.3 A working minimum of SQL

SQL is the language for talking to a relational database. It is **declarative**: you
describe the result you want, not the steps.

```sql
-- read
SELECT Title, Author FROM Articles;
SELECT * FROM Articles WHERE Author = 'Ada';
SELECT * FROM Articles WHERE PublishedOn >= '2026-02-01' ORDER BY PublishedOn DESC;
SELECT Author, COUNT(*) FROM Articles GROUP BY Author;

-- write
INSERT INTO Articles (Title, Author, PublishedOn) VALUES ('New post', 'Ada', '2026-03-04');
UPDATE Articles SET Title = 'Edited title' WHERE Id = 3;
DELETE FROM Articles WHERE Id = 3;
```

Put the shapes side by side with Session 02 — this is the slide that makes EF Core
inevitable:

| Question | SQL | LINQ |
|---|---|---|
| filter | `WHERE Author = 'Ada'` | `.Where(a => a.Author == "Ada")` |
| project | `SELECT Title` | `.Select(a => a.Title)` |
| sort | `ORDER BY PublishedOn DESC` | `.OrderByDescending(a => a.PublishedOn)` |
| take one | `SELECT TOP 1` / `LIMIT 1` | `.FirstOrDefault()` |

*"You already know the four questions. Only the notation differs."*

**One warning, delivered early and firmly.** A `DELETE` or `UPDATE` without a `WHERE`
clause applies to **every row**. There is no undo. Say it, write it on the board, and
repeat it in Session 09.

Demo: open the SQLite file with the VS Code **SQLite Viewer** extension (or the `sqlite3`
CLI) and run three of these by hand. Seeing the table with their own eyes changes how
students think about the rest of the course.

### 7.4 The object–relational mismatch

Your program thinks in **objects**: an `Article` with typed properties, references to
other objects, methods. Your database thinks in **tables**: rows, columns, keys, no
behaviour, no inheritance.

Bridging them by hand looks like this:

```
open a connection
write "SELECT Id, Title, Author, PublishedOn FROM Articles WHERE Id = @id"
add the parameter
execute
loop over the result rows:
    new Article {
        Id          = (int)      reader["Id"],
        Title       = (string)   reader["Title"],
        Author      = (string)   reader["Author"],
        PublishedOn = (DateTime) reader["PublishedOn"]
    }
close the connection
```

...for every query, in every direction, in every application ever written. It is
repetitive, and every column name is a string the compiler cannot check.

An **ORM** (Object–Relational Mapper) automates the bridge: you declare how classes map to
tables, and it generates the SQL and the object construction.

```
Your C# classes  ⇄  [ ORM ]  ⇄  Database tables
```

| You gain | You give up |
|---|---|
| No hand-written mapping code | Some control over the exact SQL |
| Compile-time checking of property names | A layer you must learn |
| One codebase across database engines | Performance surprises if used carelessly |
| Schema changes tracked in source control (Session 08) | |

Be balanced: ORMs are the default in professional .NET, Java and Python work, *and*
knowing SQL remains essential — because when an ORM query is slow, the only way to find
out why is to read the SQL it generated. We will do exactly that in Session 08.

Every ecosystem has one: **Entity Framework Core** (.NET), Hibernate (Java), SQLAlchemy /
Django ORM (Python), Doctrine (PHP), ActiveRecord (Rails).

---

## Part 2 — In practice: EF Core in .NET 10 · ~50 min

### 7.5 Installing the pieces

```bash
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet tool install --global dotnet-ef        # once per machine
```

Open `SimpleBlog.csproj` and show the `<PackageReference>` lines that appeared — connect
to Session 02: *"this file is the project's shopping list; `dotnet add package` writes to
it."*

> **On Windows with SQL Server** the only difference is the provider package
> (`Microsoft.EntityFrameworkCore.SqlServer`), one method name (`UseSqlServer`) and the
> connection string. Everything else in this session and the next is identical. Say this
> explicitly so nobody thinks the course is SQLite-specific.

### 7.6 The entity class

The `Article` class from Session 03 becomes the table definition. Nothing about it is
database-specific yet:

```csharp
namespace SimpleBlog.Models;

public class Article
{
    public int Id { get; set; }                   // convention: 'Id' → primary key
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public string Summary { get; set; } = "";
    public string Body { get; set; } = "";
    public DateTime PublishedOn { get; set; }
}
```

Conventions EF Core applies with no configuration:

- A property named `Id` (or `ArticleId`) becomes the **primary key**.
- An integer key is **generated by the database** on insert — do not set it yourself.
- Property names become column names; C# types map to database types.
- `string` is nullable in the database unless told otherwise — Session 08 fixes that with
  data annotations.

**Domain model vs. view model**, revisited from Session 06: `Article` is what we *store*;
`ArticleListViewModel` is what a *page needs*. They change for different reasons, which is
exactly why they are different classes.

### 7.7 The `DbContext`

```csharp
// Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using SimpleBlog.Models;

namespace SimpleBlog.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Article> Articles => Set<Article>();
}
```

Explain it as **the database, expressed as a C# object**:

- The class ≈ the database.
- Each `DbSet<T>` ≈ one table. Its name (`Articles`) becomes the table name.
- The constructor takes **options** — which provider, which connection string — rather
  than deciding for itself. That is what makes it configurable and testable, and it leads
  directly to §7.9.
- A `DbSet<T>` is queryable exactly like the `List<T>` from Session 02 — `Where`,
  `OrderBy`, `FirstOrDefault` all work. That is the ORM's central trick, and Session 08
  shows what it really does with them.

### 7.8 The connection string

`appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=simpleblog.db"
  },
  "Logging": { "LogLevel": { "Default": "Information" } },
  "AllowedHosts": "*"
}
```

A **connection string** is the address and credentials of the database — *"which database,
where, and how do I get in."* It varies by provider:

```
SQLite     Data Source=simpleblog.db
LocalDB    Server=(localdb)\\mssqllocaldb;Database=SimpleBlog;Trusted_Connection=True
SQL Server Server=.\\SQLEXPRESS;Database=SimpleBlog;Trusted_Connection=True;TrustServerCertificate=True
```

**Security note, delivered now and repeated in Session 10:** connection strings for real
databases contain passwords. `appsettings.json` is committed to source control. Real
projects keep secrets in user-secrets, environment variables or a key vault — never in a
committed file. Mention `dotnet user-secrets init` as the local answer.

Connect to Session 02: `appsettings.json` was "configuration" on a folder tour; now it has
a job.

### 7.9 Dependency injection

Ask the naïve question: why not just write `new ApplicationDbContext()` in the controller?

Because then the controller decides *which* database, *when* it opens, *when* it closes,
and every controller repeats that decision. Change the provider and you edit forty files.
Test a controller and you need a real database.

**The principle — inversion of control.** A class states what it *needs*; something else
decides what it *gets*. Same inversion as Session 02's framework and Session 05's layout.

**Analogy — the restaurant, once more.** The waiter does not go to the market. Supplies
arrive; the waiter uses them.

Registration, in `Program.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using SimpleBlog.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
```

Read it as a sentence: *"whenever something needs an `ApplicationDbContext`, build one
using SQLite and the `DefaultConnection` string."*

Consumption, in the controller:

```csharp
public class ArticlesController : Controller
{
    private readonly ApplicationDbContext _context;

    public ArticlesController(ApplicationDbContext context)   // asked for, not created
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var articles = _context.Articles
            .OrderByDescending(a => a.PublishedOn)
            .ToList();

        return View(articles);
    }
}
```

This is **constructor injection**: the controller declares a dependency as a constructor
parameter, and the framework supplies it when it creates the controller for that request
(Session 03, step 4 of the lifecycle). Nobody writes `new ArticlesController(...)` — the
framework does.

**Lifetimes**, briefly and concretely:

| Lifetime | One instance per | `DbContext` uses |
|---|---|---|
| Transient | every request for it | |
| **Scoped** | **one HTTP request** | ✅ — `AddDbContext` registers scoped by default |
| Singleton | the whole application | |

Scoped is right because a `DbContext` tracks the changes you make and is not safe to share
between simultaneous requests.

> Running the app **now** will fail: the database file does not exist and has no tables.
> That is the correct place to stop — Session 08 opens with migrations, which create it.
> Show the error deliberately so the next session has a reason to exist.

---

## Live demo checkpoints

| # | State | How to reach it |
|---|---|---|
| 1 | Data loss shown | restart the app; the in-memory list resets |
| 2 | A real table inspected and queried by hand | SQLite Viewer / `sqlite3`; run `SELECT`, `WHERE`, `ORDER BY` |
| 3 | SQL and LINQ compared side by side | the §7.3 table |
| 4 | EF Core packages installed | `dotnet add package ...`, then open `.csproj` |
| 5 | `ApplicationDbContext` with `DbSet<Article>` created | `Data/ApplicationDbContext.cs` |
| 6 | Connection string added | `appsettings.json` |
| 7 | `AddDbContext` registered | `Program.cs` |
| 8 | Controller taking the context by constructor injection | replace the static list |
| 9 | The "no such table" failure shown deliberately | `dotnet run` → the error |

**SimpleBlog state after this session:** EF Core installed and configured; `Article` is an
entity; `ApplicationDbContext` exposes `Articles`; the connection string is in
`appsettings.json`; `ArticlesController` receives the context by injection and queries it.
The database file itself does not exist yet — Session 08.

## Common misconceptions

| Misconception | Correction |
|---|---|
| "A `List<T>` is fine, it keeps my data." | Only while the process lives. Restart = gone. |
| "A database is just a spreadsheet." | It also enforces rules, answers queries efficiently, and handles many users at once. |
| "Store dates as text, it's simpler." | Then you cannot sort or compare them correctly. Use date types. |
| "An ORM means I never need SQL." | When a query is slow, reading the generated SQL is the only way to find out why. |
| "`DbContext` is the database." | It is a C# object representing a *session* with the database. |
| "Set the `Id` before inserting." | The database generates it. Leave it 0. |
| "`new ApplicationDbContext()` in the controller is simpler." | It hard-codes the provider, leaks connections, and makes testing impossible. |
| "One `DbContext` for the whole app is efficient." | It tracks changes and is not safe across concurrent requests. Scoped per request. |
| "SQLite is a toy; the real course is SQL Server." | Only the provider and connection string differ. Everything you learn transfers. |
| "Connection strings are fine in `appsettings.json`." | Not with a password in it. That file is committed. |

## Check for understanding

1. Name two problems a database solves that a `List<Article>` cannot.
2. What is a primary key, and why must it never be reused?
3. Write the SQL for "the three newest articles by Ada." Now write the LINQ.
4. What is an ORM bridging, exactly? Name one thing you gain and one you give up.
5. `_context.Articles` — what is it, and what can you do to it that you already learned in
   Session 02?
6. Why does the controller take `ApplicationDbContext` as a constructor parameter instead
   of creating one? Give two reasons.
7. Why is `DbContext` registered as *scoped* rather than *singleton*?

## Lab task (in class) · 25 min

Working in your own `SimpleBlog`:

1. Install `Microsoft.EntityFrameworkCore.Sqlite` and
   `Microsoft.EntityFrameworkCore.Design`, plus the `dotnet-ef` tool. Show the new lines
   in `.csproj`.
2. Move `Article` into a clean entity shape: `Id`, `Title`, `Author`, `Summary`, `Body`,
   `PublishedOn`.
3. Create `Data/ApplicationDbContext.cs` with a `DbSet<Article> Articles`.
4. Add `ConnectionStrings:DefaultConnection` to `appsettings.json`.
5. Register the context in `Program.cs` with `AddDbContext` + `UseSqlite`.
6. Change `ArticlesController` to receive the context by constructor injection and delete
   the static list. Rewrite `Index` and `Details` to query `_context.Articles` with LINQ —
   keeping `FirstOrDefault` + `NotFound()` from Session 03.
7. Run the app and **record the exact error**. Explain in one sentence what is missing.
8. Using the SQL viewer, write three `SELECT` statements against any sample database and
   record their results.

**Acceptance criteria:** the project compiles; no `new ApplicationDbContext()` anywhere; no
static list remains; the controller's queries use LINQ; the student can state precisely
why the app fails at runtime and what Session 08 must provide.

## Homework

1. Compare your Session 03 homework answer ("what would change if articles came from a
   database?") with what actually changed. What did you predict correctly?
2. Write the SQL for: all articles ordered by title; articles published in 2026; the count
   of articles per author; deleting article 5. For the last one, write down what happens if
   you forget the `WHERE`.
3. Research and write 150 words: what would change in `Program.cs` and `appsettings.json`
   to use SQL Server LocalDB instead of SQLite? How many files would you touch?
4. Sketch a second table, `Categories`, and show how `Articles` would reference it. Which
   column is the foreign key?

## Glossary (EN → BG)

| English | Български |
|---|---|
| persistence | постоянно съхранение |
| database / DBMS | база данни / система за управление на БД |
| table / row / column | таблица / ред / колона |
| primary key / foreign key | първичен ключ / външен ключ |
| schema / constraint | схема / ограничение |
| normalisation | нормализация |
| query | заявка |
| ORM (object–relational mapper) | обектно-релационно съответствие |
| entity | същност / ентити |
| connection string | низ за връзка |
| provider | доставчик |
| dependency injection | вграждане на зависимости |
| inversion of control | обръщане на управлението |
| service lifetime (scoped) | живот на услугата (в обхвата на заявка) |
| constructor injection | вграждане през конструктор |

## References

- EF Core — getting started: https://learn.microsoft.com/ef/core/get-started/overview/first-app
- `DbContext` configuration: https://learn.microsoft.com/ef/core/dbcontext-configuration/
- Connection strings: https://learn.microsoft.com/ef/core/miscellaneous/connection-strings
- Dependency injection in ASP.NET Core: https://learn.microsoft.com/aspnet/core/fundamentals/dependency-injection
- Safe storage of secrets in development: https://learn.microsoft.com/aspnet/core/security/app-secrets
- SQL tutorial (W3Schools, for the SQL primer): https://www.w3schools.com/sql/
