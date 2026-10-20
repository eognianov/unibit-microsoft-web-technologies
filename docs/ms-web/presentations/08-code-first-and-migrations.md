# Session 08 — Code-First and Migrations

> Syllabus mapping: Lecture 4 (Code-First, data annotations, migrations, seeding, showing
> the list) · ~180 min

## Learning objectives

By the end of this session students will be able to:

1. Explain the Code-First approach and how a schema can be derived from classes.
2. Constrain a model with data annotations and predict the resulting columns.
3. Create, apply, inspect and roll back a migration.
4. Seed a database with initial data.
5. Query the database with LINQ and read the SQL that EF Core generated.
6. Display a list of records from the database in a Bootstrap-styled view.

## Prerequisites / recap

- Session 07: entity, `DbContext`, `DbSet<T>`, connection string, DI — and the deliberate
  failure at the end.
- Session 02: LINQ operators. Session 06: view models and Bootstrap cards/tables.
- Open on that failure: *"The app asks for a table that does not exist. Today we create it
  — without writing a line of SQL, and in a way that our teammates get too."*

## Agenda

| # | Block | Min | Mode |
|---|-------|-----|------|
| 1 | Recap + the missing table | 10 | talk |
| 2 | Schema-first vs. code-first; the schema-change problem | 25 | talk |
| 3 | Describing constraints on the model | 20 | talk + demo |
| 4 | *Break* | 10 | — |
| 5 | Migrations: create, inspect, apply, roll back | 35 | demo |
| 6 | Seeding initial data | 15 | demo |
| 7 | *Break* | 10 | — |
| 8 | Querying: LINQ in, SQL out | 20 | demo |
| 9 | Lab: your database, your article list | 30 | lab |
| 10 | Wrap-up, homework, Q&A | 5 | talk |

---

## Part 1 — The idea (vendor-neutral) · ~45 min

### 8.1 Two directions

There are two ways to end up with a database and matching classes:

| Approach | You write | Generated | Suits |
|---|---|---|---|
| **Database-first** | the schema (SQL) | classes | an existing database you inherited |
| **Code-first** | the classes | the schema | new applications |

Neither is "correct"; they answer different situations. This course uses **code-first**,
because we are building something new and because it makes the classes — the thing we edit
daily — the single source of truth.

### 8.2 The real problem: schemas change

The naïve picture ("generate the database from the classes") works exactly once. Then
reality:

- Week 1: `Article` has `Title` and `Body`. You create the database.
- Week 2: you add `Summary`.
- Week 3: you rename `Author` and make `Title` required.

Now ask the room the questions that make migrations necessary:

1. Your database already has 500 rows. Can you delete and recreate it? (No — that is
   people's data.)
2. Your teammate pulled your code. How does *their* database learn about `Summary`?
3. The production server has the old schema. How does it get updated, at deploy time,
   safely?
4. The change was wrong. How do you go back?

**The answer, in the abstract:** record every schema change as an ordered, versioned,
repeatable script, stored in source control next to the code that needs it. Each one knows
how to move the schema **forward**, and how to move it **back**.

```
schema v0  ──[001_Initial]──▶  v1  ──[002_AddSummary]──▶  v2  ──[003_RequireTitle]──▶  v3
           ◀──────────────────     ◀────────────────────      ◀──────────────────────
```

The database itself remembers which of these it has already applied, in a small
bookkeeping table. That is why running "apply migrations" twice is safe.

This is not an EF Core invention: Rails migrations, Django migrations, Flyway and Liquibase
(Java), Alembic (Python) are the same idea. The vocabulary transfers.

**Two rules that prevent nearly all migration pain:**

1. **A migration that has been shared or deployed is history. Never edit it** — add a new
   one.
2. **Read the generated migration before applying it.** Tools guess; a rename can be
   guessed as "drop a column and add another", which silently destroys data.

### 8.3 Describing the rules on the model

The schema needs more than property names. Which columns may be empty? How long may a
title be? What is a date versus a timestamp? What is the key?

Two ways to say it, in every ORM:

- **Convention** — the tool assumes sensible defaults (`Id` is the key).
- **Explicit declaration** — you annotate or configure the exceptions.

And note that these declarations pay **twice**: they shape the database *and*, in
Session 09, they validate what users submit. One statement, two jobs — which is exactly why
the model is the right place for it.

---

## Part 2 — In practice: EF Core in .NET 10 · ~70 min

### 8.4 Data annotations

```csharp
using System.ComponentModel.DataAnnotations;

namespace SimpleBlog.Models;

public class Article
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    [Display(Name = "Article title")]
    public string Title { get; set; } = "";

    [Required]
    [MaxLength(100)]
    public string Author { get; set; } = "";

    [MaxLength(500)]
    public string? Summary { get; set; }

    [Required]
    [DataType(DataType.MultilineText)]
    public string Body { get; set; } = "";

    [DataType(DataType.Date)]
    [Display(Name = "Published on")]
    public DateTime PublishedOn { get; set; }
}
```

| Annotation | Effect on the database | Effect elsewhere |
|---|---|---|
| `[Key]` | primary key (redundant when the property is `Id`) | — |
| `[Required]` | `NOT NULL` | validation + client-side rules (S09) |
| `[MaxLength(n)]` | `nvarchar(n)` instead of unlimited | validation (S09) |
| `[DataType(...)]` | mostly none | choice of input control + formatting (S09) |
| `[Display(Name=...)]` | none | the label shown in views and forms (S09) |

Two things to point out precisely:

- `string?` vs `string` matters. With nullable reference types on (the default in modern
  .NET), a non-nullable `string` is already treated as required. `[Required]` makes the
  intent explicit and drives validation messages.
- `[MaxLength]` is not decoration. Without it, every text column is unbounded, which is
  slow to index and impossible to validate.

Mention that the Fluent API in `OnModelCreating` is the alternative for anything
annotations cannot express (composite keys, relationships, indexes) — name it, do not
teach it.

### 8.5 Creating the first migration

```bash
dotnet ef migrations add InitialCreate
```

**Then stop and read the generated file.** This is the most valuable five minutes of the
session. `Migrations/20260304_InitialCreate.cs`:

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.CreateTable(
        name: "Articles",
        columns: table => new
        {
            Id          = table.Column<int>(nullable: false)
                               .Annotation("Sqlite:Autoincrement", true),
            Title       = table.Column<string>(maxLength: 200, nullable: false),
            Author      = table.Column<string>(maxLength: 100, nullable: false),
            Summary     = table.Column<string>(maxLength: 500, nullable: true),
            Body        = table.Column<string>(nullable: false),
            PublishedOn = table.Column<DateTime>(nullable: false)
        },
        constraints: table => table.PrimaryKey("PK_Articles", x => x.Id));
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DropTable(name: "Articles");
}
```

Walk it line by line against the annotations: `[MaxLength(200)]` → `maxLength: 200`;
`[Required]` → `nullable: false`; `string?` → `nullable: true`; `Id` → primary key and
autoincrement. *"Every annotation you wrote has a visible consequence. This file is the
proof."*

Also note the three parts of the migrations folder: the timestamped migration, its
`.Designer.cs`, and `ApplicationDbContextModelSnapshot.cs` — the snapshot is how EF Core
knows the *previous* state to diff against. Never edit the snapshot by hand.

To see the raw SQL without touching the database:

```bash
dotnet ef migrations script
```

Show it. It closes the loop with Session 07's SQL primer: *"this is what an ORM was hiding
from you, and you can always ask to see it."*

### 8.6 Applying it

```bash
dotnet ef database update
```

Then open `simpleblog.db` in the SQLite viewer and show:

- the `Articles` table with the expected columns and types
- the `__EFMigrationsHistory` table with one row

That second table is the whole trick — the database's record of what it has applied. Run
`dotnet ef database update` again: nothing happens, because it is already at the latest
version.

Run the app. The Session 07 error is gone; the page loads with an empty list. Applause.

### 8.7 Changing the schema

Do a change live, end to end, so the cycle is muscle memory:

```csharp
[MaxLength(50)]
public string? Tags { get; set; }
```

```bash
dotnet ef migrations add AddTagsToArticle
dotnet ef database update
```

Show the new migration file — it contains `AddColumn`, not `CreateTable`. Existing rows
survive. **That** is the point of migrations.

Then show going back:

```bash
dotnet ef database update AddTagsToArticle      # move to a specific migration
dotnet ef migrations remove                     # delete the last, un-applied migration
```

And the destructive lever, with an explicit warning:

```bash
dotnet ef database drop                         # deletes the whole database. Development only.
```

> **Visual Studio equivalents** (the syllabus mentions the Package Manager Console):
> `Add-Migration AddTagsToArticle`, `Update-Database`, `Remove-Migration`,
> `Script-Migration`. Same operations, different front end. Cross-platform students use the
> CLI.

**The daily cycle**, put on a slide students photograph:

```
1. change the model class
2. dotnet ef migrations add <DescriptiveName>
3. READ the generated migration
4. dotnet ef database update
5. run the app
```

### 8.8 Seeding

An empty database makes for a poor demo and poor development. **Seeding** puts known
starting data in.

The migration-based approach — the data becomes part of the schema history:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Article>().HasData(
        new Article { Id = 1, Title = "How the web works", Author = "Ada",
                      Summary = "Requests and responses.", Body = "...",
                      PublishedOn = new DateTime(2026, 1, 10) },
        new Article { Id = 2, Title = "Getting started with MVC", Author = "Grace",
                      Summary = "Controllers, views, models.", Body = "...",
                      PublishedOn = new DateTime(2026, 2, 2) }
    );
}
```

```bash
dotnet ef migrations add SeedArticles
dotnet ef database update
```

Note the one surprise: with `HasData` you **must** specify `Id` values explicitly, even
though the database normally generates them, because EF Core needs stable identities to
tell inserts from updates across migrations.

Mention the alternative — a seeding method called at startup for data that should not live
in migration history — in one sentence, without code.

### 8.9 Querying, and reading the SQL

Now the payoff from Session 02:

```csharp
// all, newest first
var all = await _context.Articles
    .OrderByDescending(a => a.PublishedOn)
    .ToListAsync();

// one, or null
var article = await _context.Articles
    .FirstOrDefaultAsync(a => a.Id == id);

// filtered and projected
var titles = await _context.Articles
    .Where(a => a.Author == author)
    .OrderBy(a => a.Title)
    .Select(a => a.Title)
    .ToListAsync();

// read-only listing — no change tracking, measurably faster
var list = await _context.Articles.AsNoTracking().ToListAsync();
```

Three things to teach here, each with a demo:

1. **`async`/`await`.** A database call waits on the network. `await` frees the thread to
   serve other requests meanwhile. The rule for this course: use the `...Async` methods and
   make the action `public async Task<IActionResult>`. Do not teach the theory; teach the
   pattern.

2. **Deferred execution.** `.Where(...)` builds a *query*; `.ToListAsync()` *runs* it. One
   round trip happens, at the end, with everything combined. Prove it with logging:

   ```
   SELECT "a"."Id", "a"."Title", "a"."Author", "a"."PublishedOn"
   FROM "Articles" AS "a"
   WHERE "a"."Author" = @__author_0
   ORDER BY "a"."Title"
   ```

   EF Core logs generated SQL to the console at `Information` level by default in
   Development. Point at it in the running terminal — students should watch this window all
   session.

3. **The classic trap.** `.ToList().Where(...)` loads the entire table into memory and
   filters in C#. `.Where(...).ToList()` filters in the database. Show both SQL statements
   side by side. With 500 000 rows this is the difference between a working site and a dead
   one.

The controller and view, completing the loop:

```csharp
public async Task<IActionResult> Index(string? author)
{
    var query = _context.Articles.AsNoTracking().AsQueryable();

    if (!string.IsNullOrWhiteSpace(author))
        query = query.Where(a => a.Author == author);

    var vm = new ArticleListViewModel
    {
        Articles     = await query.OrderByDescending(a => a.PublishedOn).ToListAsync(),
        Heading      = author is null ? "All articles" : $"Articles by {author}",
        FilterAuthor = author,
        AllAuthors   = await _context.Articles.Select(a => a.Author)
                                              .Distinct().OrderBy(x => x).ToListAsync()
    };

    return View(vm);
}
```

The view is Session 06's, unchanged. Say that out loud: *the view model insulated the page
from where the data came from.* One layer changed; the other did not. That is separation of
concerns paying a dividend.

For an admin-style listing, the Bootstrap table from Session 06:

```cshtml
<div class="table-responsive">
  <table class="table table-striped table-hover align-middle">
    <thead>
      <tr><th>Title</th><th>Author</th><th>Published</th><th></th></tr>
    </thead>
    <tbody>
    @foreach (var a in Model.Articles)
    {
        <tr>
            <td>@a.Title</td>
            <td>@a.Author</td>
            <td>@a.PublishedOn.ToString("d MMM yyyy")</td>
            <td class="text-end">
                <a asp-action="Details" asp-route-id="@a.Id"
                   class="btn btn-sm btn-outline-primary">Details</a>
            </td>
        </tr>
    }
    </tbody>
  </table>
</div>
```

### 8.10 Optional: a second entity

If the group is moving fast, add `Category` and a one-to-many relationship — otherwise
leave it as an extension task:

```csharp
public class Category
{
    public int Id { get; set; }
    [Required, MaxLength(50)] public string Name { get; set; } = "";
    public List<Article> Articles { get; set; } = new();
}

public class Article
{
    // ...
    public int? CategoryId { get; set; }        // foreign key (Session 07's diagram)
    public Category? Category { get; set; }     // navigation property
}
```

with `.Include(a => a.Category)` to load the related row. Point out that without `Include`,
`Category` is `null` — EF Core does not fetch what you did not ask for.

---

## Live demo checkpoints

| # | State | How to reach it |
|---|---|---|
| 1 | Annotations added to `Article` | `[Required]`, `[MaxLength]`, `[DataType]`, `[Display]` |
| 2 | First migration generated and **read aloud** | `dotnet ef migrations add InitialCreate` |
| 3 | Raw SQL of the migration shown | `dotnet ef migrations script` |
| 4 | Database created; table inspected | `dotnet ef database update` + SQLite viewer |
| 5 | `__EFMigrationsHistory` shown; update run twice | inspect the table, rerun the command |
| 6 | App runs with an empty list — Session 07's error gone | `dotnet run` |
| 7 | A column added by a second migration, data preserved | add `Tags`, migrate, check rows |
| 8 | Rollback demonstrated | `dotnet ef database update <PreviousMigration>` |
| 9 | Seed data appearing on the page | `HasData` + migration + update |
| 10 | Generated SQL read in the console | watch the terminal while browsing |
| 11 | `.ToList().Where()` vs `.Where().ToList()` compared | two SQL statements side by side |

**SimpleBlog state after this session:** a real SQLite database created by migrations,
seeded with sample articles, driving the responsive list and details pages from Session 06.
Reading works end to end. Writing — forms and CRUD — is Session 09.

## Common misconceptions

| Misconception | Correction |
|---|---|
| "A migration is the database." | It is one recorded *change* to it. The database tracks which ones it has applied. |
| "Migrations run automatically." | You run them: `dotnet ef database update`. Production is Session 10. |
| "Edit the migration if it's wrong." | Only if it has never been shared or applied. Otherwise add a new one. |
| "Delete the database and start over." | Fine on day one, catastrophic with real data. That is why migrations exist. |
| "`[MaxLength]` is just for validation." | It changes the column type. It also validates (Session 09). |
| "Annotations only matter to the database." | They also drive validation and form rendering in Session 09. |
| "`.Where()` runs immediately." | Nothing runs until `ToList`, `First`, `Count`, or `foreach`. |
| "`.ToList().Where()` and `.Where().ToList()` are the same." | The first loads the whole table into memory. |
| "`async` makes it faster." | It makes the server able to serve more requests. One request is not faster. |
| "Related data loads by itself." | Not without `Include`. It will be `null`. |

## Check for understanding

1. Why can't we just delete and recreate the database whenever a class changes?
2. What is `__EFMigrationsHistory` for? What would break without it?
3. Which annotations changed the *shape* of your columns, and which changed nothing in the
   database at all?
4. You added a property and ran `dotnet ef database update` without running
   `migrations add`. What happens, and why?
5. Explain deferred execution using `Where` and `ToList`. Which one talks to the database?
6. `.Include()` — what problem does it solve, and what happens if you forget it?
7. You need to rename a column. What must you check in the generated migration before
   applying it?

## Lab task (in class) · 30 min

Working in your own `SimpleBlog`:

1. Add data annotations to `Article`: `Title` required, max 200; `Author` required, max
   100; `Summary` optional, max 500; `Body` required, multiline; `PublishedOn` as a date
   with the display name "Published on".
2. `dotnet ef migrations add InitialCreate`. **Do not apply it yet** — open the file and
   write down which line each annotation produced.
3. `dotnet ef migrations script` and skim the SQL. `dotnet ef database update`.
4. Open the database file and confirm the columns, their types and nullability. Find
   `__EFMigrationsHistory`.
5. Seed at least five articles by at least two authors using `HasData` + a new migration.
6. Make `Index` and `Details` `async`, query with `...Async` methods, and confirm the
   Session 06 list page renders the seeded data with no change to the view.
7. Add a `Tags` property, migrate, apply, and confirm the seeded rows survived.
8. In the terminal, find and copy the SQL EF Core generated for your list page.
9. Write both `.ToList().Where(...)` and `.Where(...).ToList()`, capture both SQL
   statements, and write one sentence on the difference.

**Acceptance criteria:** the database is created *by migrations* (not by hand); the list
page shows seeded data; the `Tags` migration preserved existing rows; the student can
produce the generated SQL for their query and explain the deferred-execution difference.

## Homework

1. Add an `UpdatedOn` nullable date, migrate, and confirm existing rows have `NULL`. Why
   would making it `[Required]` have failed?
2. Roll your database back to `InitialCreate` and forward again. Record the commands and
   what each did.
3. Add the `Category` entity and the one-to-many relationship. Migrate, seed three
   categories, and display each article's category in the list — using `.Include()`.
   Then remove the `Include` and describe exactly what breaks.
4. Read the generated migration for the `Category` change and identify the foreign key
   constraint by name.
5. Write 100 words on what would go wrong if a team of four shared one database and did not
   use migrations.

## Glossary (EN → BG)

| English | Български |
|---|---|
| code-first / database-first | първо код / първо база данни |
| migration | миграция |
| schema change | промяна на схемата |
| rollback / revert | връщане назад |
| data annotation | атрибут за данни |
| constraint (NOT NULL, UNIQUE) | ограничение |
| seeding | начално зареждане на данни |
| deferred execution | отложено изпълнение |
| change tracking | проследяване на промените |
| eager loading (`Include`) | ранно зареждане |
| navigation property | навигационно свойство |
| asynchronous / await | асинхронен / изчакване |
| generated SQL | генериран SQL |

## References

- EF Core migrations: https://learn.microsoft.com/ef/core/managing-schemas/migrations/
- `dotnet ef` CLI reference: https://learn.microsoft.com/ef/core/cli/dotnet
- Data seeding: https://learn.microsoft.com/ef/core/modeling/data-seeding
- Querying data: https://learn.microsoft.com/ef/core/querying/
- Tracking vs. no-tracking queries: https://learn.microsoft.com/ef/core/querying/tracking
- Data annotations reference: https://learn.microsoft.com/dotnet/api/system.componentmodel.dataannotations
