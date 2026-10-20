# Microsoft Web Technologies — Presentation Plan

Ten lecture sessions (~3 h each, ≈30 lecture hours) derived from the official syllabus
in [`../program.md`](../program.md) and [`../01-microsoft-web-technologies-bachelors.md`](../01-microsoft-web-technologies-bachelors.md).

## Teaching principles

1. **Concept first, vendor-neutral.** Every idea — request handler, template, ORM,
   routing, validation — is introduced as a *general* web-development concept, with no
   Microsoft vocabulary attached. Students should be able to carry the idea to any stack.
2. **Then concrete, in .NET 10 MVC.** Once the idea has landed, it is named and
   demonstrated in ASP.NET Core MVC on .NET 10.
3. **Zero-background safe.** The audience may never have programmed. No term is used
   before the session that defines it; forward references are explicit.
4. **One app, grown incrementally.** Every session moves the same demo app forward.

## Session map

| # | Session | Syllabus | Focus |
|---|---------|----------|-------|
| 01 | [How the Web Works](01-how-the-web-works.md) | Lecture 1 | Client–server, HTTP, URLs, methods, status codes |
| 02 | [From Language to Framework](02-from-language-to-framework.md) | Lecture 1 | C# essentials, SDK/runtime/framework, first MVC project |
| 03 | [Handling a Request](03-handling-a-request.md) | Lecture 2 | MVC pattern, controllers, actions, parameters |
| 04 | [Routing and URL Design](04-routing-and-url-design.md) | Lecture 2 | URL design, conventional + attribute routing |
| 05 | [Templates: HTML, CSS and Razor](05-templates-html-and-razor.md) | Lecture 3 | HTML/CSS basics, server-side rendering, Razor, layouts |
| 06 | [View Models and Bootstrap](06-view-models-and-bootstrap.md) | Lecture 3 | Passing data to views, responsive design, Bootstrap 5 |
| 07 | [Data, Databases and ORMs](07-data-databases-and-orms.md) | Lecture 4 | Relational model, SQL basics, ORM, EF Core + DI |
| 08 | [Code-First and Migrations](08-code-first-and-migrations.md) | Lecture 4 | Data annotations, migrations, seeding, LINQ queries |
| 09 | [Forms, CRUD and Validation](09-forms-crud-and-validation.md) | Lecture 5 | HTML forms, POST/Redirect/GET, tag helpers, validation, CSRF |
| 10 | [Debugging, Logging and Deployment](10-debugging-logging-deployment.md) | Lecture 5 | Debugger, error pages, logging, publish, prod migrations |

Progress and open items: [`../BACKLOG.md`](../BACKLOG.md).

## Demo application: SimpleBlog

A small blog / news site. The domain is deliberately trivial — everyone already knows
what an article is — so classroom time goes to the technology, not to the business rules.

- **Core entity:** `Article` — `Title`, `Author`, `Summary`, `Body`, `PublishedOn`.
- **Optional second entity** (only if time allows, Session 08): `Category`.
- **Created with:** `dotnet new mvc -n SimpleBlog -f net10.0`

Each session file ends with the exact state of `SimpleBlog` at that point, so a student
who misses a week can catch up from the previous session's checkpoint.

> The syllabus uses a `LocalLibrary` / `Book` example. It maps one-to-one:
> `Book` → `Article`, `BookController` → `ArticlesController`.

## Tooling baseline

| Concern | Course default | Alternative |
|---|---|---|
| SDK | .NET 10 SDK | — |
| Editor | VS Code + C# Dev Kit (any OS) | Visual Studio Community (Windows), Rider |
| Database | SQLite (works on every OS) | SQL Server Express / LocalDB + SSMS (Windows) |
| CSS | Bootstrap 5 | — |
| Hosting (Session 10) | Azure App Service | IIS |

SQLite is the default so that macOS and Linux students are never blocked. Session 07
shows that switching to SQL Server changes only the connection string and one line of
provider registration.

## Session file structure

Every session file follows the same template, so any session can be scanned and timed
identically:

- **Learning objectives** — "students will be able to…"
- **Prerequisites / recap**
- **Agenda** — block / minutes / mode (talk, demo, lab)
- **Part 1 — The idea (vendor-neutral)**
- **Part 2 — In practice: .NET 10 MVC**
- **Live demo checkpoints** — resumable app states + exact commands
- **Common misconceptions**
- **Check for understanding**
- **Lab task (in class)** — with acceptance criteria
- **Homework**
- **Glossary (EN → BG)**
- **References**

## Timing convention

180 minutes per session, including two 10-minute breaks:

```
00:00  Recap + objectives            10 min
00:10  Part 1 — the idea             45 min
00:55  Break                         10 min
01:05  Part 2 — in .NET 10           50 min
01:55  Break                         10 min
02:05  Lab                           45 min
02:50  Wrap-up, homework, Q&A        10 min
```

Individual sessions adjust these blocks; each file states its own split.

## Assessment

Per the syllabus: continuous assessment (in-class practical tasks) plus a defended final
project. The final-project brief and grading rubric are introduced in Session 10.
