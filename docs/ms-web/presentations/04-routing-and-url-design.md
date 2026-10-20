# Session 04 — Routing and URL Design

> Syllabus mapping: Lecture 2 (routing) · ~180 min

## Learning objectives

By the end of this session students will be able to:

1. Explain what routing is and why URLs are a design decision, not an accident.
2. Design readable, stable URLs for a set of features.
3. Read and modify a conventional route template, including defaults and optional
   segments.
4. Use attribute routing to give an action a specific URL.
5. Constrain route parameters and predict which route wins when several match.
6. Generate URLs in code instead of hard-coding them.

## Prerequisites / recap

- Session 01: URL anatomy (scheme, host, path, query, fragment).
- Session 03: controllers, actions, model binding by name.
- Open with: *"Last week we wrote the code that handles `/Articles/Details/2`. Nobody
  explained why that URL, and not `/a/2` or `/read?what=2`. Today: who decides, and how."*

## Agenda

| # | Block | Min | Mode |
|---|-------|-----|------|
| 1 | Recap + objectives | 10 | talk |
| 2 | What routing is; URLs as a public interface | 25 | talk |
| 3 | URL design principles | 20 | talk + group exercise |
| 4 | *Break* | 10 | — |
| 5 | Conventional routing in ASP.NET Core | 30 | talk + demo |
| 6 | Attribute routing and constraints | 25 | demo |
| 7 | *Break* | 10 | — |
| 8 | Generating URLs; route order and conflicts | 15 | demo |
| 9 | Lab: design and implement the blog's URLs | 30 | lab |
| 10 | Wrap-up, homework, Q&A | 5 | talk |

---

## Part 1 — The idea (vendor-neutral) · ~45 min

### 4.1 What routing is

A request arrives carrying only a **method** and a **path**: `GET /articles/42`.
Somewhere, something must decide *which piece of your code answers it*. That decision is
**routing**, and the piece of configuration that describes it is a **route**.

**Analogy — the switchboard.** A call comes in for extension 42. The switchboard does not
know or care what happens on that extension; it knows only how to connect the number to a
desk. Routing is the switchboard between URLs and code.

**The historical detour, worth two minutes.** In the 1990s a URL *was* a file path:
`/products/shoes.html` meant a file called `shoes.html` in a folder called `products`.
Adding a page meant adding a file. Today the path is a **name we choose**, interpreted by
our program. Nothing on disk has to match it. This is why routing exists as a topic at
all.

### 4.2 URLs are a public interface

Make the case that URLs deserve design effort:

- **They are the most public part of your app.** People read them, type them, paste them
  into chats, print them on posters, read them aloud on the phone.
- **They are permanent.** Once someone bookmarks or links to a URL, changing it breaks
  their link. ("Cool URIs don't change" — Tim Berners-Lee, 1998.)
- **They are your app's table of contents.** A good URL tells a stranger what the page is
  before it loads.
- **Search engines use them.**

Compare, on a slide:

```
Bad                                  Good
/page.aspx?id=42&t=3                 /articles/42
/GetArticleByIdAction                /articles/42
/a/x/9                               /articles/2026/03/routing-basics
/Articles/Details/42                 /articles/42
```

Then be honest: the last "bad" example is the framework's *default*, and it is perfectly
acceptable for a course project and for internal apps. The point is that it is a
**choice**, and you now know how to change it.

### 4.3 URL design principles

1. **Nouns for things, not verbs.** `/articles/42`, not `/getArticle?id=42`. The *verb* is
   already in the HTTP method (Session 01).
2. **Hierarchy with `/`, filters with `?`.** The path identifies *which resource*; the
   query string modifies *how you want it*.
   `/articles/42/comments` — a thing. `/articles?author=ada&sort=new` — a filtered view.
3. **Lowercase, hyphen-separated.** `/article-archive`, not `/ArticleArchive` or
   `/article_archive`. Paths are case-sensitive on many servers; do not make people guess.
4. **Stable and guessable.** If `/articles/42` exists, `/articles/43` should mean the
   analogous thing. Predictability is a feature.
5. **No implementation leakage.** `.aspx`, `.php`, `/api/v2/internal/` — nobody outside
   your team should be able to tell what you built it in.
6. **Plural collections.** `/articles` (many) and `/articles/42` (one of them). Pick one
   style and never mix.

**Group exercise (10 min).** Put a feature list on the screen and have pairs write the
URL and HTTP method for each:

| Feature | URL? | Method? |
|---|---|---|
| The home page | | |
| All articles | | |
| One article | | |
| All articles by one author | | |
| The article archive for March 2026 | | |
| The form to write a new article | | |
| Actually saving that new article | | |
| Deleting an article | | |
| The "about us" page | | |

Collect answers on the board and argue about them. The interesting disagreements are the
last three — the form vs. the save (same URL, different methods, Session 09) and whether
delete is a `DELETE` or a `POST` (browsers' forms only do GET and POST; Session 09 again).

---

## Part 2 — In practice: .NET 10 MVC · ~70 min

### 4.4 The default route, decoded

In `Program.cs`:

```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

Take that string apart segment by segment:

| Piece | Meaning |
|---|---|
| `{controller}` | a **route parameter** — this segment names the controller |
| `=Home` | its **default** when the segment is absent |
| `{action}` | this segment names the action method |
| `=Index` | its default |
| `{id?}` | the `?` makes this segment **optional** |

Now walk a table of URLs against it — this is the single most useful slide of the session:

| URL | controller | action | id |
|---|---|---|---|
| `/` | Home | Index | — |
| `/Articles` | Articles | Index | — |
| `/Articles/Details` | Articles | Details | — |
| `/Articles/Details/42` | Articles | Details | 42 |
| `/Articles/Details/42/extra` | ✗ no match → 404 | | |

Then connect it to Session 03: the matched `id` value is handed to model binding, which
fills the `int id` parameter **by name**. `{id?}` and `Details(int id)` are the two halves
of one contract — rename one and you must rename the other. Demonstrate the breakage.

**Editing the default route.** Show that these are just strings you control:

```csharp
// Make Articles the home page of the site
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Articles}/{action=Index}/{id?}");
```

Save with `dotnet watch` running, reload `/`, and the blog is now the front page.

**Adding a named route** — order matters, most specific first:

```csharp
app.MapControllerRoute(
    name: "archive",
    pattern: "archive/{year:int}/{month:int}",
    defaults: new { controller = "Articles", action = "Archive" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

`/archive/2026/3` now reaches `Articles.Archive(int year, int month)`.

> **Route order rule:** routes are tried in the order they are registered, and the *first*
> match wins. Put specific routes before general ones. Demonstrate the bug: register
> `default` first, then `archive`, and watch `/archive/2026/3` fail as
> controller=`archive`, action=`2026`.

### 4.5 Attribute routing

Conventional routing describes URLs *centrally*; attribute routing describes them *next to
the code they belong to*. Both are valid; most real projects mix them — conventional for
the broad shape, attributes where a specific URL is wanted.

```csharp
[Route("articles")]                       // base path for every action in this controller
public class ArticlesController : Controller
{
    [HttpGet("")]                         // GET /articles
    public IActionResult Index() { ... }

    [HttpGet("{id:int}")]                 // GET /articles/42
    public IActionResult Details(int id) { ... }

    [HttpGet("by/{author}")]              // GET /articles/by/ada
    public IActionResult ByAuthor(string author) { ... }

    [HttpGet("archive/{year:int}/{month:int:range(1,12)}")]
    public IActionResult Archive(int year, int month) { ... }

    [HttpGet("/about")]                   // leading slash = absolute: GET /about
    public IActionResult About() { ... }
}
```

Points to make:

- `[Route]` on the class is a **prefix**; `[HttpGet("...")]` on the action completes it.
- A **leading `/`** escapes the prefix and gives an absolute path.
- `[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpDelete]` bind the *method* as well as the
  path — the same URL can have a GET action and a POST action. That is exactly the shape
  of every form in Session 09; plant the flag now.
- Once a controller has attribute routes, conventional routes no longer reach it. Say this
  explicitly — it is a classic confusion.

### 4.6 Route constraints

A constraint says *what kind of value* a segment may hold. It is a routing filter, not
validation of user input (that is Session 09).

```csharp
[HttpGet("{id:int}")]                             // only digits
[HttpGet("{slug:alpha}")]                         // only letters
[HttpGet("{year:int:min(2000)}")]                 // a range
[HttpGet("{month:int:range(1,12)}")]
[HttpGet("{code:length(3)}")]                     // exact length
[HttpGet("{id:guid}")]
```

Why they earn their keep:

- Without a constraint, `/articles/banana` matches `Details(int id)`, binding fails, and
  `id` silently becomes `0` — you get a confusing 404 for "article 0".
- With `{id:int}`, `/articles/banana` **does not match this route at all**, so it can fall
  through to a *different* route: `/articles/routing-basics` can mean "the article with
  this slug" while `/articles/42` means "the article with this id".

Demonstrate exactly that pair:

```csharp
[HttpGet("{id:int}")]      public IActionResult Details(int id) { ... }
[HttpGet("{slug}")]        public IActionResult BySlug(string slug) { ... }
```

### 4.7 Generating URLs — never hard-code

If URLs are typed as literal strings across your views and controllers, changing a route
means hunting through the whole project. Ask the framework instead:

```csharp
// in a controller
return RedirectToAction("Details", "Articles", new { id = 42 });

var url = Url.Action("Details", "Articles", new { id = 42 });   // "/articles/42"
```

```cshtml
@* in a view — the tag helper form, used from Session 05 onward *@
<a asp-controller="Articles" asp-action="Details" asp-route-id="42">Read</a>
```

The payoff, demonstrated live: change the route template from `articles` to `posts`, save,
and every generated link updates itself while every hard-coded `href="/articles/42"`
breaks. Do this on screen — it is the most convincing 60 seconds of the session.

---

## Live demo checkpoints

| # | State | How to reach it |
|---|---|---|
| 1 | Default route decoded against the URL table | read `Program.cs` |
| 2 | Blog set as the site's front page | change the default to `{controller=Articles}...` |
| 3 | `{id?}` renamed to break model binding, then fixed | rename to `{articleId?}`, observe, revert |
| 4 | `archive/{year}/{month}` route working | add a named route before `default` |
| 5 | Route-order bug shown and fixed | register `archive` after `default`, observe the 404 |
| 6 | Controller converted to attribute routing | `[Route("articles")]` + `[HttpGet]` attributes |
| 7 | `{id:int}` vs `{slug}` coexisting | `/articles/42` and `/articles/routing-basics` |
| 8 | Rename `articles` → `posts`; generated links survive, hard-coded ones break | edit the `[Route]` prefix |

**SimpleBlog state after this session:** `ArticlesController` from Session 03, now with
designed URLs (`/articles`, `/articles/42`, `/articles/by/{author}`,
`/articles/archive/{year}/{month}`, `/about`), constraints on the numeric segments, and no
hard-coded links. Still returning plain text — views arrive next session.

## Common misconceptions

| Misconception | Correction |
|---|---|
| "The URL maps to a file on the server." | It maps to whatever the routes say. Nothing on disk needs to match. |
| "Routes are matched by best fit." | They are matched **in registration order**; first match wins. |
| "A constraint validates user input." | It decides whether the route *matches*. User input validation is Session 09. |
| "`{id?}` makes the C# parameter optional." | The *segment* is optional. A missing `int id` becomes `0`; use `int?` for real optionality. |
| "You must choose conventional or attribute routing." | Projects routinely use both. But once a controller has attributes, conventional routes skip it. |
| "Query-string values need a route." | No. Anything after `?` is bound by name without appearing in the template. |
| "Changing a URL is harmless." | Every existing link and bookmark breaks. Use a 301 (Session 01) when you must move something. |

## Check for understanding

1. With the default template, which controller and action does `/` reach? And `/Articles/42`
   — is that the same as `/Articles/Details/42`?
2. You register `default` first and `archive/{year}/{month}` second. What happens to
   `/archive/2026/3`, and why?
3. What is the difference in behaviour between `{id}` and `{id:int}` when the request is
   `/articles/banana`?
4. Write the attribute routing needed for `GET /authors/ada/articles`.
5. Your team renames `/articles` to `/posts`. Which links in your project break, and which
   fix themselves? Why?
6. `/articles?sort=new&page=2` — how many of these values appear in the route template?
   How do they reach the action?

## Lab task (in class) · 30 min

Working in your own `SimpleBlog`:

1. **Design first, on paper.** Write the URL and HTTP method for: all articles; one
   article; articles by an author; the archive for a given year and month; the about page;
   the contact page. Apply the six principles from §4.3.
2. Implement your design with **attribute routing** on `ArticlesController`.
3. Add constraints: `id` must be an integer; `month` must be in the range 1–12.
4. Add a second route that catches `/articles/{slug}` (letters and hyphens) and returns
   `Content($"slug: {slug}")`, and verify that `/articles/42` still reaches `Details`.
5. Make `/about` an absolute path, not `/articles/about`.
6. Replace every hard-coded URL in your project with `Url.Action` / `RedirectToAction`.
7. Change your `[Route]` prefix from `articles` to `posts`, verify the site still works
   entirely, then change it back.

**Acceptance criteria:** every designed URL responds; `/articles/banana` reaches the slug
action rather than failing; `/articles/archive/2026/13` returns 404 because of the range
constraint; renaming the prefix breaks nothing.

## Homework

1. Take three websites you use. For each, write down five URLs and grade them against the
   six principles. Which site's URLs are best, and why?
2. Add a route so that `/2026/03/routing-basics` reaches an action
   `Permalink(int year, int month, string slug)`. Constrain year and month. What had to
   go *before* the default route, and why?
3. Explain in writing why `GET /articles/42/delete` is a bad idea, using Session 01's
   vocabulary. What should it be instead?
4. Find one URL on a real site that leaks its implementation. What does it reveal?

## Glossary (EN → BG)

| English | Български |
|---|---|
| routing / route | маршрутизиране / маршрут |
| route template | шаблон на маршрут |
| route parameter | параметър на маршрута |
| segment (of a path) | сегмент |
| default value | стойност по подразбиране |
| optional segment | незадължителен сегмент |
| constraint | ограничение |
| conventional routing | конвенционално маршрутизиране |
| attribute routing | атрибутно маршрутизиране |
| attribute (C#) | атрибут |
| slug | текстов идентификатор (slug) |
| URL generation | генериране на URL адреси |
| permalink | постоянна връзка |

## References

- Routing in ASP.NET Core: https://learn.microsoft.com/aspnet/core/fundamentals/routing
- Routing to controller actions: https://learn.microsoft.com/aspnet/core/mvc/controllers/routing
- Route constraint reference: https://learn.microsoft.com/aspnet/core/fundamentals/routing#route-constraint-reference
- Tim Berners-Lee, *Cool URIs don't change*: https://www.w3.org/Provider/Style/URI
