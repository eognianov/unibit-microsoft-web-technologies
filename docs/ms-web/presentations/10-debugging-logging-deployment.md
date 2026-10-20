# Session 10 — Debugging, Logging and Deployment

> Syllabus mapping: Lecture 5 (debugging, error pages, logging, deployment, wrap-up)
> · ~180 min
> The last session. It also closes the course and hands out the final-project brief.

## Learning objectives

By the end of this session students will be able to:

1. Debug an application with breakpoints, stepping, watches and the call stack.
2. Distinguish the developer exception page from a production error page, and configure
   both.
3. Write useful log entries and choose an appropriate level.
4. Explain what deployment is: the difference between running locally and hosting.
5. Publish an ASP.NET Core application and apply migrations to a production database.
6. State the final-project requirements and how they will be assessed.

## Prerequisites / recap

- Session 01: 4xx vs 5xx — *whose bug is it?*
- Session 02: exceptions. Session 07: connection strings and secrets. Session 08:
  migrations. Session 09: a complete CRUD application.
- Open with: *"Your app works on your machine. Two things remain: finding out why it
  doesn't when it doesn't, and getting it onto a machine that isn't yours."*

## Agenda

| # | Block | Min | Mode |
|---|-------|-----|------|
| 1 | Recap + objectives | 10 | talk |
| 2 | How to find a bug — the method | 20 | talk |
| 3 | Demo: the debugger | 25 | demo |
| 4 | *Break* | 10 | — |
| 5 | Error pages: development vs. production | 20 | demo |
| 6 | Logging: levels, what to log, what never to log | 20 | talk + demo |
| 7 | *Break* | 10 | — |
| 8 | Deployment: the idea, then publishing | 30 | talk + demo |
| 9 | Course wrap-up and the final project | 25 | talk |
| 10 | Q&A | 10 | talk |

---

## Part 1 — The idea (vendor-neutral) · ~60 min

### 10.1 Debugging is a method, not a talent

Beginners debug by changing things until the error stops. That is slow and it teaches
nothing. The method:

1. **Reproduce it reliably.** A bug you cannot trigger on demand cannot be fixed, only
   guessed at. Write down the exact steps.
2. **Read the error message.** All of it — including the file, the line and the innermost
   exception. Students skip this constantly; make it a rule.
3. **Form one hypothesis.** "The id is arriving as 0." Not "something is wrong with the
   database."
4. **Test that one hypothesis** — with a breakpoint or a log line.
5. **Narrow the search.** Confirm where the value is still right and where it is already
   wrong. Halve the interval each time.
6. **Fix the cause, not the symptom.** A `try/catch` around a crash is not a fix.
7. **Verify** — reproduce the original steps and confirm the behaviour changed.

State the rule that saves the most time: **you are not looking for the bug, you are looking
for the first place the state is wrong.** Everything after that point is a consequence.

**Three tools, three purposes:**

| Tool | Best for |
|---|---|
| The **debugger** | inspecting state at a moment, in detail |
| **Logging** | what happened over time, especially where you cannot attach a debugger |
| **Browser DevTools** | what actually crossed the wire (Session 01) |

### 10.2 What a debugger does

A **breakpoint** tells the runtime to pause on a line, before executing it. While paused,
the program is frozen and inspectable.

| Action | Meaning |
|---|---|
| Continue | run until the next breakpoint |
| Step over | run this line, do not go inside calls |
| Step into | go inside the call on this line |
| Step out | finish the current method, return to the caller |

While paused you can see:
- **Locals** — every variable in scope, right now.
- **Watch** — expressions you chose to keep an eye on.
- **Call stack** — who called whom to get here. Read it bottom-up to answer "how did we
  get to this line?"

Say plainly: *a debugger is not for emergencies. Stepping through code you just wrote,
while it works, is the fastest way to learn what it really does.*

### 10.3 Error pages: two audiences

When something throws, two very different people might be looking:

| | You, developing | A user, in production |
|---|---|---|
| Needs | the exception, the stack trace, the line, the query | "Something went wrong. Try again." |
| Must not see | — | file paths, code, connection strings, table names |

The rule: **detailed diagnostics in development, a generic apology in production.** A stack
trace on a public site tells an attacker your framework, your versions, your file layout
and often your database structure. Meanwhile the *real* details go to the log, where only
you can read them.

### 10.4 Logging

`Console.WriteLine` does not survive deployment: on a server there is no console to watch.
A **logging framework** gives you levels, filtering, timestamps, and somewhere durable to
write.

**Levels**, with the honest question each answers:

| Level | Question | Example |
|---|---|---|
| `Trace` | every detail | rarely on |
| `Debug` | what is happening internally | "query returned 12 rows" |
| `Information` | normal, notable events | "article 42 created by Ada" |
| `Warning` | odd, but handled | "article 999 not found" |
| `Error` | this operation failed | "saving article failed" |
| `Critical` | the application is in trouble | "database unreachable" |

Practical guidance:

- **Log the events, not the noise.** A log nobody reads is worse than no log — it hides the
  one line that mattered.
- **Include the identifiers** you would need to investigate: which article, which user,
  which request.
- **Never log secrets or personal data**: passwords, tokens, connection strings, card
  numbers. Logs are copied, shipped and read by many people. This is a legal question as
  well as a technical one.
- **Log at the boundaries**: things entering and leaving your app, and every caught
  exception.

### 10.5 What deployment actually is

Ask: *what has been running our site for ten weeks?* `dotnet run`, on a laptop, on a
university network, stopping when the lid closes.

To be a website it needs:

| Requirement | Locally | In production |
|---|---|---|
| A machine that is always on | your laptop | a server / cloud service |
| A public address | `localhost:5001` | a domain name + DNS (Session 01) |
| HTTPS | a dev certificate | a real certificate |
| A database reachable from that machine | a local file | a hosted database |
| Restart after a crash or reboot | you press ▶ | the host does it |
| Configuration and secrets | `appsettings.json` | environment variables / key vault |

**The two steps of any deployment**, in any technology:

1. **Build** — turn source code into an optimised, self-contained set of files to run. Not
   your source: the *output*.
2. **Host** — put those files on a machine configured to run them and answer requests.

**Hosting models**, one line each: your own server (full control, full responsibility); a
platform service such as Azure App Service (you supply the app, they supply the machine);
containers (you ship the app *and* its environment, so "works on my machine" becomes
"ships as my machine").

**Environments.** The same code must behave differently in different places — verbose
errors in development, silence in production; a local database here, a real one there. The
mechanism is configuration by environment (`ASPNETCORE_ENVIRONMENT`), not `if` statements
in your code.

**The migration question**, which catches everyone once: the production database is *not*
your database. It has real data. Deploying new code that expects a new column, without
applying the migration, breaks the site. Deploy order matters, and the safe habit is
backup → migrate → deploy.

---

## Part 2 — In practice: .NET 10 · ~60 min

### 10.6 Debugging the app

**Set-up.** In VS Code: F5 with the C# Dev Kit (a `launch.json` is generated). In Visual
Studio: F5. The app starts with the debugger attached.

Run a live investigation — plant a bug beforehand and find it with the class. A good one:
`Details` returns 404 for an article that exists, because the action parameter is named
`articleId` while the route says `{id}` (Session 04's contract, broken on purpose).

1. Breakpoint on the first line of `Details`.
2. Request `/articles/2`. Execution pauses.
3. **Locals**: `id` is `0`, not `2`. *There is the first wrong value.*
4. **Call stack**: read who invoked the action.
5. **Watch**: add `_context.Articles.Count()` — evaluate an expression against the live
   database while paused.
6. Step over the `FirstOrDefaultAsync` and watch `article` become `null`.
7. Diagnose: model binding never matched. Fix the name. Verify.

Also demonstrate:

- **Conditional breakpoints** — right-click a breakpoint → condition `id == 42`. Priceless
  inside a loop over 500 rows.
- **Breaking on a thrown exception** so you stop *where* it was thrown, not where it
  surfaced.
- **Hot reload** — edit while running (`dotnet watch`), as used all course.

### 10.7 Exception pages and error handling

The template's `Program.cs` already contains this; now it means something:

```csharp
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();     // implicit in development
}
```

Demonstrate both, deliberately:

1. Throw in an action: `throw new InvalidOperationException("boom");`
2. Run in Development → the **developer exception page**: message, stack trace, the source
   line, the request headers, the route values.
3. Switch environments and run again:

```bash
ASPNETCORE_ENVIRONMENT=Production dotnet run     # macOS/Linux
$env:ASPNETCORE_ENVIRONMENT="Production"; dotnet run   # PowerShell
```

4. → the generic `/Home/Error` page. **Both return HTTP 500** — check DevTools. Session 01
   closes here: the status code is the same, the disclosure is not.

Also add friendly pages for status codes without exceptions (a 404 from `NotFound()`):

```csharp
app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");
```

And show the `Properties/launchSettings.json` file where the development environment
variable is set — students often wonder where `Development` comes from.

### 10.8 Logging in practice

Logging is already registered; inject and use it:

```csharp
public class ArticlesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ArticlesController> _logger;

    public ArticlesController(ApplicationDbContext context,
                              ILogger<ArticlesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Details(int id)
    {
        var article = await _context.Articles.FindAsync(id);

        if (article is null)
        {
            _logger.LogWarning("Article {ArticleId} was not found.", id);
            return NotFound();
        }

        _logger.LogInformation("Serving article {ArticleId} ({Title}).", id, article.Title);
        return View(article);
    }
}
```

Note two things:

- `ILogger<T>` arrives by **dependency injection** — the same mechanism as `DbContext`
  (Session 07). Nothing new to configure.
- **Structured logging:** `{ArticleId}` is a named placeholder, not string concatenation.
  A log system can then search on `ArticleId = 42` across millions of lines. Show the
  difference against `$"Article {id}"` — which produces the same text and none of the
  searchability.

Levels are filtered in `appsettings.json`, per environment:

```json
"Logging": {
  "LogLevel": {
    "Default": "Information",
    "Microsoft.AspNetCore": "Warning",
    "Microsoft.EntityFrameworkCore.Database.Command": "Information"
  }
}
```

That last line is what has been printing the generated SQL since Session 08 — turn it off
and watch the console go quiet. Then note that `appsettings.Development.json` overrides
`appsettings.json`, which is how one codebase behaves differently in two places.

### 10.9 Publishing

```bash
dotnet publish -c Release -o ./publish
```

Open the `publish` folder and look at it with the class: DLLs, `appsettings.json`,
`wwwroot`. **No `.cs` files.** *"This is the artifact. This is what gets deployed — not
your source folder."*

Note `-c Release`: optimised, no debug symbols. `Debug` is for your machine only.

**To Azure App Service** — from VS Code (Azure extension → Deploy to Web App) or the CLI:

```bash
az webapp up --name simpleblog-<yourname> --runtime "DOTNET:10" --sku F1
```

Or, in Visual Studio: right-click the project → **Publish** → Azure App Service → create a
**publish profile** → Publish. The syllabus names the publish profile explicitly; show the
`Properties/PublishProfiles/*.pubxml` file it creates and explain it is a saved recipe, not
magic.

**Then configure the deployed app** — and this is where the real lesson is:

- Set `ASPNETCORE_ENVIRONMENT=Production` (the default on App Service).
- Set the connection string as an **application setting** in the portal, *not* in the
  committed `appsettings.json` (Session 07's warning, now unavoidable).
- Confirm HTTPS is on.

**To IIS**, for the Windows-hosted case: install the ASP.NET Core Hosting Bundle, create a
site pointing at the published folder, set the app pool to "No Managed Code" (the app hosts
its own runtime — a genuine surprise for anyone who used .NET Framework). Mention it;
demonstrate Azure.

### 10.10 Migrations against a production database

Never `dotnet ef database update` against production from a laptop as a habit. Generate a
script, review it, and let a DBA or a deployment pipeline apply it:

```bash
dotnet ef migrations script --idempotent -o migrate.sql
```

`--idempotent` produces SQL that checks what has already been applied, so it is safe to run
more than once.

The safe order, stated as a checklist:

1. **Back up** the production database.
2. Review the script — especially anything that drops or renames a column (Session 08's
   warning about guessed renames).
3. Apply the script.
4. Deploy the new code.
5. Verify, and know how to roll back.

Note the tempting shortcut and why it is a trap: `context.Database.Migrate()` at startup is
convenient for a course project, but with several instances starting at once it races, and
it gives the running app permission to alter the schema. Fine for a student project; say
out loud that it is not the professional default.

---

## Part 3 — Course wrap-up and final project · ~25 min

### 10.11 What you have learned

Walk the arc, quickly, one line per session — students should recognise every step:

```
01  a request and a response are just text
02  a framework handles the repetitive part; C# is the language
03  a controller decides what happens                 ← your code
04  a route decides which controller
05  a template turns data into HTML on the server
06  a view model carries exactly what a page needs; a CSS framework makes it presentable
07  a database outlives the process; an ORM bridges objects and tables
08  migrations version the schema alongside the code
09  forms change data — and every input is untrusted
10  you can find what broke, and put it where others can reach it
```

Then name what a 30-hour course could not cover, so nobody thinks the list is finished:
authentication and authorisation (ASP.NET Core Identity), Web APIs and JSON, unit and
integration testing, front-end frameworks, caching and performance, background jobs,
Docker, CI/CD. Point at where to go next.

### 10.12 The final project

**Build a data-driven ASP.NET Core MVC application of your own choosing** — not the blog.

Minimum requirements:

- [ ] At least **two related entities** (one-to-many), created code-first with migrations.
- [ ] Full **CRUD** for the main entity, with all four operations working.
- [ ] **Server-side validation** with meaningful messages, plus client-side hints.
- [ ] **Anti-forgery tokens** on every POST; nothing changing state on a GET.
- [ ] At least **five views** using a shared layout and at least one partial.
- [ ] **Responsive** with Bootstrap; usable at 375px and at 1440px.
- [ ] **Designed URLs** — readable and consistent (Session 04's principles).
- [ ] **Seed data** so the app is not empty on first run.
- [ ] **View models**, not entities, wherever a page needs more or less than one entity.
- [ ] A `README.md`: what it does, how to run it, what you would do next.

Assessment weighting:

| Criterion | Weight |
|---|---|
| It works — the required features actually function | 30 % |
| Correct MVC structure and separation of concerns | 20 % |
| Data layer: modelling, migrations, sensible queries | 20 % |
| Validation and security (validation, CSRF, no state-changing GETs) | 15 % |
| Presentation: layout, responsiveness, URL design | 10 % |
| The defence: can you explain your own code? | 5 % |

**The defence** is 10 minutes: demonstrate it, then answer questions about any part of your
code. Explaining something you cannot justify is the fastest way to lose marks — so write
code you understand.

Deadline, submission format and defence dates: *(fill in for your term)*.

---

## Live demo checkpoints

| # | State | How to reach it |
|---|---|---|
| 1 | A planted bug found with the debugger | breakpoint → Locals → call stack → fix |
| 2 | A conditional breakpoint inside a loop | right-click the breakpoint → condition |
| 3 | The developer exception page | `throw` in an action, run in Development |
| 4 | The production error page, same 500 | `ASPNETCORE_ENVIRONMENT=Production dotnet run` |
| 5 | A friendly 404 page | `UseStatusCodePagesWithReExecute` |
| 6 | `ILogger` injected; warning and information entries in the console | inject and call |
| 7 | The EF Core SQL log switched off and on | edit `appsettings.json` log levels |
| 8 | The `publish` output folder inspected | `dotnet publish -c Release -o ./publish` |
| 9 | The app deployed and reachable on a public URL | `az webapp up` or VS Publish |
| 10 | The connection string set as an app setting, not in the file | Azure portal configuration |
| 11 | An idempotent migration script generated and read | `dotnet ef migrations script --idempotent` |

**SimpleBlog state after this session:** complete — a deployed, publicly reachable CRUD
application with environment-appropriate error handling, logging, and a documented
migration procedure.

## Common misconceptions

| Misconception | Correction |
|---|---|
| "Debugging means adding print statements." | The debugger shows every variable at once, without editing code. |
| "The debugger is for emergencies." | Stepping through working code is the fastest way to understand it. |
| "The stack trace is noise." | It is the map of how execution reached the failure. |
| "Users should see the error so they can report it." | They should see an apology; you should see the log. |
| "`Console.WriteLine` is fine for logging." | There is no console on a server, no levels, no filtering, no timestamps. |
| "Log everything, just in case." | A log nobody reads hides the line that mattered. |
| "Deploying means copying my project folder." | You deploy the *published output*, not source. |
| "It works locally, so it will work deployed." | Different environment, configuration, database, permissions and file system. |
| "The production database updates itself." | Migrations must be applied deliberately, after a backup. |
| "`appsettings.json` is a fine place for the production password." | It is committed to source control. Use app settings, environment variables or a key vault. |
| "Debug and Release are the same." | Release is optimised and strips debug information. |

## Check for understanding

1. You get a 500 in production. List, in order, the first four things you do.
2. Same exception in Development and in Production: what differs on the user's screen, and
   what stays the same in the HTTP response?
3. When would you log `Warning` rather than `Error`? Give an example from your own project.
4. Why is `_logger.LogInformation("Article {Id} saved", id)` better than
   `_logger.LogInformation($"Article {id} saved")`?
5. Name three things that must exist in production but not on your laptop.
6. You add a required column and deploy the code without running the migration. What does
   the user see? What is the correct order?
7. Why should the production connection string not be in `appsettings.json`?

## Lab task (in class) · integrated into the demos

1. Plant a bug in a partner's project (swap machines). Find it using only the debugger and
   write down which value was wrong first.
2. Add `ILogger<T>` to your `ArticlesController`. Log an `Information` on successful
   create, a `Warning` on a not-found, and an `Error` inside a `catch`.
3. Throw an exception in an action and view it in both Development and Production. Record
   both screens and the status code from DevTools.
4. Add a friendly 404 page and verify it appears for an unknown article id.
5. `dotnet publish -c Release -o ./publish` and list what is in the output. Find the one
   thing that surprises you.
6. Generate `migrate.sql` with `--idempotent` and read the guard around each migration.
7. *(Optional, if accounts are available)* Deploy to Azure App Service and share the URL.

**Acceptance criteria:** the bug was found with the debugger and the student can say which
value was wrong first; three log entries at appropriate levels appear in the console; both
error pages were observed with the same status code; the published output contains no `.cs`
files.

## Homework — the final project

Deliver the project described in §10.12. Between now and the defence:

1. Choose a domain and write down the two entities and their relationship.
2. Build it in the same order the course did: routes and controllers → views and layout →
   model and migrations → CRUD and validation → error handling and polish.
3. Commit as you go, with meaningful messages. The history is part of the story you tell at
   the defence.
4. Write the `README.md` first, not last — it forces you to decide what the app is.

## Glossary (EN → BG)

| English | Български |
|---|---|
| debugger / breakpoint | дебъгер / точка на прекъсване |
| step over / into / out | прескачане / влизане / излизане |
| watch / locals | наблюдение / локални променливи |
| call stack | стек на извикванията |
| stack trace | следа на стека |
| exception page | страница за грешка |
| environment (dev / prod) | среда (разработка / продукция) |
| logging / log level | логване / ниво на логване |
| structured logging | структурирано логване |
| deployment / to publish | разгръщане / публикуване |
| build artifact | резултат от компилацията |
| hosting | хостинг |
| publish profile | профил за публикуване |
| idempotent script | идемпотентен скрипт |
| rollback / backup | връщане назад / резервно копие |

## References

- Debugging in VS Code (C#): https://code.visualstudio.com/docs/csharp/debugging
- Error handling in ASP.NET Core: https://learn.microsoft.com/aspnet/core/fundamentals/error-handling
- Logging in .NET: https://learn.microsoft.com/aspnet/core/fundamentals/logging
- Use multiple environments: https://learn.microsoft.com/aspnet/core/fundamentals/environments
- Host and deploy ASP.NET Core: https://learn.microsoft.com/aspnet/core/host-and-deploy
- Deploy to Azure App Service: https://learn.microsoft.com/azure/app-service/quickstart-dotnetcore
- Host on IIS: https://learn.microsoft.com/aspnet/core/host-and-deploy/iis
- Applying migrations in production: https://learn.microsoft.com/ef/core/managing-schemas/migrations/applying
