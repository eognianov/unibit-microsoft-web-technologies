# Session 06 — View Models and Bootstrap

> Syllabus mapping: Lecture 3 (passing data to views, Bootstrap styling) · ~180 min

## Learning objectives

By the end of this session students will be able to:

1. Choose deliberately between `ViewData`, `ViewBag` and a view model, and justify the
   choice.
2. Design a view model that carries exactly what one page needs.
3. Explain what a CSS framework is and what responsive design means.
4. Build a responsive page with the Bootstrap 5 grid.
5. Use Bootstrap's navbar, card, table and button components.
6. Produce a home page for SimpleBlog that works on a phone and on a projector.

## Prerequisites / recap

- Session 03: the controller gathers data and picks a view.
- Session 05: `@model`, `@foreach`, layouts, partials, `wwwroot`.
- Open with: *"Our list page shows articles. Now the page also needs a heading, a count,
  the current filter and a list of authors. `@model List<Article>` cannot carry all of
  that. Today: how data actually travels from controller to view — and then, how to make
  the result not look like 1996."*

## Agenda

| # | Block | Min | Mode |
|---|-------|-----|------|
| 1 | Recap + objectives | 10 | talk |
| 2 | Passing data to a view — three mechanisms | 30 | talk + demo |
| 3 | View models: designing for the page | 25 | demo |
| 4 | *Break* | 10 | — |
| 5 | CSS frameworks and responsive design — the idea | 25 | talk |
| 6 | The Bootstrap grid | 25 | demo |
| 7 | *Break* | 10 | — |
| 8 | Bootstrap components: navbar, card, table, badge | 15 | demo |
| 9 | Lab: build the SimpleBlog home page | 25 | lab |
| 10 | Wrap-up, homework, Q&A | 5 | talk |

---

## Part 1 — The idea (vendor-neutral) · ~55 min

### 6.1 The problem: what does a page actually need?

Take the article list page and enumerate honestly what must appear on it:

- the articles themselves
- a page heading
- how many results there are
- the author currently filtered on (or "all")
- the list of authors to offer as filters
- whether the visitor may write a new article

That is **six** things, of five different types. A single `List<Article>` carries one of
them. So the general question is: *what is the shape of the box you hand from controller
to view?*

Three answers exist in every framework, in increasing order of discipline:

| Mechanism | Shape | Checked by the compiler? |
|---|---|---|
| An untyped bag of key/value pairs | `bag["title"] = "Articles"` | no |
| A dynamic object | `bag.Title = "Articles"` | no |
| A purpose-built class for this page | `new ArticleListViewModel { ... }` | **yes** |

The third is the professional default, and the reason is worth stating carefully:
**a typo in the first two is discovered by a user; a typo in the third is discovered by the
compiler.**

### 6.2 The view model idea

A **view model** is a class that exists for exactly one screen. It is not the database
shape and not the domain shape — it is *"everything this page needs, and nothing else."*

**Analogy — the tray.** The kitchen does not send the fridge to the table. It plates
exactly what was ordered, on one tray. The view model is that tray.

Why it is worth an extra file:

- **It is a contract.** The view declares what it needs; the controller must supply it.
- **It shows only what should be shown.** A user record has a password hash. A view model
  for the profile page simply does not have that property, so it cannot leak. (This
  becomes a real security point in Session 09.)
- **It can combine sources.** Articles from one place, author names from another, the
  current filter from the URL — one object.
- **It can pre-compute.** Formatting and derived values belong in the controller or the
  view model, not tangled into markup.

The cost is honest: one more small class per page. Accept it.

### 6.3 CSS frameworks and why they exist

From Session 05, students can write CSS. Now ask: *how much CSS does a real site need?*
Navigation that collapses on a phone, buttons in five states, forms, tables, modals,
spacing that is consistent across 40 pages, and all of it working in every browser. That
is thousands of lines, and none of it is your blog.

The same argument as Session 02's framework argument, applied to appearance. A **CSS
framework** is a pre-written stylesheet of ready-made classes and components — Bootstrap,
Tailwind, Bulma, Foundation. We use **Bootstrap 5** because the syllabus does and because
it is the most common in .NET shops, but the reasoning transfers.

**How they work:** you stop writing rules and start applying **utility classes** to your
markup.

```html
<!-- what you would have written -->
<div style="padding:1rem; margin-bottom:1rem; border:1px solid #dee2e6; border-radius:.375rem">

<!-- what you write instead -->
<div class="p-3 mb-3 border rounded">
```

Name the trade-off out loud so students can argue about it later in their careers: markup
gets noisier, but styling becomes consistent, responsive and fast, and a newcomer can read
it. For a course project, and for most business applications, that trade is clearly worth
it.

### 6.4 Responsive design

**The problem.** Your page will be viewed on a 360px phone and a 2560px monitor. A layout
with fixed pixel widths is broken on one of them.

**The principle.** Design one page that *adapts*, rather than building separate sites.
Three mechanisms make it work:

1. **Fluid widths** — percentages and fractions instead of fixed pixels.
2. **The viewport meta tag** — from Session 05:
   `<meta name="viewport" content="width=device-width, initial-scale=1">`. Without it a
   phone pretends to be 980px wide and shrinks everything. Demonstrate by deleting it.
3. **Media queries** — apply different rules at different widths:

```css
.sidebar { display: none; }
@media (min-width: 768px) {
    .sidebar { display: block; width: 30%; }
}
```

**Mobile-first** is the convention: write the small-screen rules as the base, then *add*
rules for wider screens. It is easier to add than to undo, and the phone — the constrained
device — gets the simplest stylesheet.

**The grid idea.** Almost every framework divides the page into **12 columns** and lets
you say how many columns each block occupies *at each screen size*. Twelve because it
divides by 2, 3, 4 and 6 — halves, thirds and quarters all work.

```
Phone:            Desktop:
┌────────────┐    ┌──────┬──────┬──────┐
│  block A   │    │  A   │  B   │  C   │
├────────────┤    └──────┴──────┴──────┘
│  block B   │      4 cols each of 12
├────────────┤
│  block C   │
└────────────┘
 12 cols each
```

Draw this. It is the mental model students need before any Bootstrap class name appears.

---

## Part 2 — In practice: .NET 10 MVC + Bootstrap 5 · ~40 min

### 6.5 `ViewData` and `ViewBag`

```csharp
public IActionResult Index()
{
    ViewData["Title"] = "All articles";      // string key, object value
    ViewBag.ArticleCount = _articles.Count;  // dynamic property
    return View(_articles);
}
```

```cshtml
<h1>@ViewData["Title"]</h1>
<p>@ViewBag.ArticleCount article(s)</p>
```

Facts to state, then move on quickly:

- They are **the same dictionary** behind two syntaxes. `ViewData["X"]` and `ViewBag.X`
  are the same slot — show it live.
- `ViewData` values come out as `object`; casting is on you:
  `((List<string>)ViewData["Authors"])`.
- `ViewBag` is `dynamic`: `ViewBag.ArticleCont` compiles happily and renders **nothing**.
  Demonstrate this typo deliberately. It is the whole argument for view models in one
  keystroke.
- **Legitimate uses:** small, cross-cutting values that every page has, such as
  `ViewData["Title"]` used by `_Layout`. That is why the template uses it.
- Everything else: use a view model.

### 6.6 View models in practice

`Models/ViewModels/ArticleListViewModel.cs`:

```csharp
namespace SimpleBlog.Models.ViewModels;

public class ArticleListViewModel
{
    public IReadOnlyList<Article> Articles { get; set; } = new List<Article>();
    public string Heading { get; set; } = "Articles";
    public string? FilterAuthor { get; set; }
    public IReadOnlyList<string> AllAuthors { get; set; } = new List<string>();

    public int Count => Articles.Count;               // computed, not stored
    public bool IsFiltered => FilterAuthor is not null;
}
```

Controller:

```csharp
public IActionResult Index(string? author)
{
    var filtered = string.IsNullOrWhiteSpace(author)
        ? _articles
        : _articles.Where(a => a.Author == author).ToList();

    var vm = new ArticleListViewModel
    {
        Articles     = filtered.OrderByDescending(a => a.PublishedOn).ToList(),
        Heading      = author is null ? "All articles" : $"Articles by {author}",
        FilterAuthor = author,
        AllAuthors   = _articles.Select(a => a.Author).Distinct().OrderBy(x => x).ToList()
    };

    return View(vm);
}
```

View:

```cshtml
@model SimpleBlog.Models.ViewModels.ArticleListViewModel

<h1>@Model.Heading</h1>
<p class="text-muted">@Model.Count article(s)</p>

@if (Model.IsFiltered)
{
    <a asp-action="Index" class="btn btn-sm btn-outline-secondary">Clear filter</a>
}
```

Point out what just happened: the LINQ from Session 02 and the model binding from
Session 03 combined into a page that filters itself, and the view contains **no logic
beyond display**. Also: `@a.Titel` in a view model is now a compile error, whereas
`ViewBag.Titel` was silence.

> **Naming convention:** `<Page><Purpose>ViewModel`, in `Models/ViewModels/`. Keep them
> out of `Models/` proper — the distinction between a *domain* model and a *view* model
> matters from Session 07 on.

### 6.7 Bootstrap in the project

The `dotnet new mvc` template already ships Bootstrap 5 in
`wwwroot/lib/bootstrap/`, referenced from `_Layout.cshtml`:

```html
<link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
...
<script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
```

Show both, and explain why the script is at the *bottom*: the page renders before the
JavaScript is fetched. Note that some components (collapsing navbar, dropdowns, modals)
need that bundle — CSS alone is not enough.

Mention the CDN alternative and its trade-off in one line: fewer files to host, one more
external dependency and no offline development. Local files are the safer default for a
course.

### 6.8 The grid

Three levels, always in this nesting order:

```cshtml
<div class="container">        <!-- centred, max-width, page padding -->
  <div class="row">            <!-- a horizontal group of columns -->
    <div class="col-12 col-md-8">Main content</div>
    <div class="col-12 col-md-4">Sidebar</div>
  </div>
</div>
```

Read `col-12 col-md-8` aloud as a sentence: *"full width by default; from medium screens
upward, eight of twelve."*

The breakpoints:

| Prefix | Applies from | Typical device |
|---|---|---|
| *(none)* | 0px | phone |
| `sm` | 576px | large phone |
| `md` | 768px | tablet |
| `lg` | 992px | laptop |
| `xl` | 1200px | desktop |

They are **min-widths** — mobile-first, as in §6.4. Demonstrate by resizing the browser
and by using DevTools' device toolbar. Watching the sidebar drop below the content at
768px is the moment responsive design becomes real for students.

Spacing utilities, enough to be useful: `m`/`p` + side (`t b s e x y`) + step (`0`–`5`):
`mb-3`, `p-4`, `mt-0`, `px-2`. Plus `text-center`, `text-muted`, `d-flex`,
`justify-content-between`, `align-items-center`.

### 6.9 Components

Only the four the syllabus names — enough to build the site, few enough to remember.

**Navbar** (in `_Layout.cshtml`, replacing Session 05's plain nav):

```html
<nav class="navbar navbar-expand-md navbar-dark bg-dark mb-4">
  <div class="container">
    <a class="navbar-brand" asp-controller="Articles" asp-action="Index">SimpleBlog</a>
    <button class="navbar-toggler" type="button"
            data-bs-toggle="collapse" data-bs-target="#nav">
      <span class="navbar-toggler-icon"></span>
    </button>
    <div class="collapse navbar-collapse" id="nav">
      <ul class="navbar-nav">
        <li class="nav-item">
          <a class="nav-link" asp-controller="Articles" asp-action="Index">Articles</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" asp-controller="Home" asp-action="About">About</a>
        </li>
      </ul>
    </div>
  </div>
</nav>
```

Narrow the window until it collapses into the hamburger. That is `navbar-expand-md` plus
the JavaScript bundle doing its job.

**Card** — rewrite Session 05's `_ArticleCard.cshtml`:

```cshtml
@model SimpleBlog.Models.Article

<div class="card mb-3 h-100">
  <div class="card-body">
    <h5 class="card-title">
      <a asp-action="Details" asp-route-id="@Model.Id"
         class="text-decoration-none">@Model.Title</a>
    </h5>
    <h6 class="card-subtitle mb-2 text-muted">
      @Model.Author · @Model.PublishedOn.ToString("d MMM yyyy")
    </h6>
    <p class="card-text">@Model.Summary</p>
    <a asp-action="Details" asp-route-id="@Model.Id" class="btn btn-primary btn-sm">
      Read more
    </a>
  </div>
</div>
```

In a responsive grid of cards:

```cshtml
<div class="row">
@foreach (var article in Model.Articles)
{
    <div class="col-12 col-md-6 col-lg-4">
        <partial name="_ArticleCard" model="article" />
    </div>
}
</div>
```

One column on a phone, two on a tablet, three on a laptop — with no media queries written
by hand. That is the payoff slide.

**Table** — for a future admin listing (used in Session 08):

```html
<table class="table table-striped table-hover align-middle">
  <thead><tr><th>Title</th><th>Author</th><th>Published</th></tr></thead>
  <tbody>...</tbody>
</table>
```

Wrap it in `<div class="table-responsive">` so it scrolls instead of breaking the layout
on a phone.

**Buttons and badges:** `btn btn-primary`, `btn btn-outline-secondary`, `btn btn-danger`
(reserve red for destructive actions — it matters in Session 09), `badge bg-secondary`.

---

## Live demo checkpoints

| # | State | How to reach it |
|---|---|---|
| 1 | `ViewData` and `ViewBag` shown to be the same store | set with one, read with the other |
| 2 | A `ViewBag` typo rendering silently as nothing | `@ViewBag.ArticleCont` |
| 3 | `ArticleListViewModel` created and used | new class + `View(vm)` + `@model` |
| 4 | The same typo now a compile error | `@Model.Cont` |
| 5 | Filtering by author working through the URL | `/articles?author=Ada` |
| 6 | Bootstrap references located in `_Layout` | show `wwwroot/lib/bootstrap` |
| 7 | Viewport meta deleted → phone layout breaks → restored | edit `_Layout`, use device toolbar |
| 8 | Grid reflowing at 768px | resize the window, DevTools device toolbar |
| 9 | Navbar collapsing into a hamburger | narrow the window |
| 10 | Cards in a 1/2/3-column responsive grid | `col-12 col-md-6 col-lg-4` |

**SimpleBlog state after this session:** a styled, responsive site — Bootstrap navbar and
footer in `_Layout`, an article list rendered as a responsive card grid driven by
`ArticleListViewModel`, author filtering via the query string, and a styled details page.
Data is still the in-memory list; the database arrives next session.

## Common misconceptions

| Misconception | Correction |
|---|---|
| "`ViewData` and `ViewBag` are different stores." | Two syntaxes over the same dictionary. |
| "`ViewBag` is simpler, so it is better." | It is unchecked. A typo fails silently at runtime. |
| "The view model must match the database table." | It matches the **page**. It may combine, omit or compute. |
| "A view model is extra work for nothing." | It is the difference between a compile error and a user-visible bug. |
| "Bootstrap makes the site responsive automatically." | Only if you use the grid classes *and* keep the viewport meta tag. |
| "`col-md-8` means 8 columns on medium screens only." | It means "from medium **upward**". Breakpoints are min-widths. |
| "You can put columns directly in a container." | Columns must live inside a `row`. |
| "Using Bootstrap means never writing CSS." | You still write your own for anything specific to your site. |
| "Copy any Bootstrap snippet and it works." | Interactive components need `bootstrap.bundle.js`. |

## Check for understanding

1. `ViewBag.Titel = "x"` in the controller and `@ViewBag.Title` in the view. What does the
   user see, and when do you find out?
2. Your page shows articles *and* a list of categories with counts. Which mechanism, and
   what would the class look like?
3. Read `class="col-12 col-md-6 col-lg-3"` aloud as an English sentence.
4. What happens to a Bootstrap page on a phone if the viewport meta tag is missing?
5. Why is `bootstrap.bundle.min.js` at the bottom of the `<body>` and not in the `<head>`?
6. Name one thing a view model can do that passing the domain object directly cannot.

## Lab task (in class) · 25 min

Working in your own `SimpleBlog`:

1. Create `Models/ViewModels/ArticleListViewModel.cs` with `Articles`, `Heading`,
   `FilterAuthor`, `AllAuthors` and a computed `Count`.
2. Rewrite `ArticlesController.Index(string? author)` to build and pass it; sort newest
   first.
3. Rewrite `Views/Articles/Index.cshtml` to use `@model ArticleListViewModel`.
4. Replace the plain nav in `_Layout.cshtml` with a Bootstrap navbar that collapses below
   `md`. Verify the hamburger works.
5. Render the articles as Bootstrap **cards** inside a grid: 1 column on phones, 2 on
   `md`, 3 on `lg`. Resize to prove all three.
6. Add a row of author filter links (`btn btn-outline-primary btn-sm`) built from
   `AllAuthors`, plus a "Clear filter" link shown only when filtered.
7. Keep exactly one use of `ViewData["Title"]` — for the page title in the layout — and
   nothing else in `ViewData`/`ViewBag`.
8. Check the page in DevTools' device toolbar at 375px and at 1440px.

**Acceptance criteria:** the list page is driven entirely by a typed view model; filtering
by author works from the URL and from the links; the card grid reflows at both
breakpoints; the navbar collapses; no `ViewBag` remains except the page title.

## Homework

1. Build a `ArticleDetailsViewModel` for the details page carrying the article plus
   `PreviousArticle` and `NextArticle` (either may be `null`) and render Previous/Next
   buttons that disappear when there is nothing to link to.
2. Add a "featured article" hero at the top of the list page: the newest article, full
   width (`col-12`), visually distinct. Do not fetch it twice — put it on the view model.
3. Deliberately break your grid: put a `col-*` directly inside a `container` with no `row`.
   Screenshot the result and explain what went wrong.
4. Write 100 words: when is Bootstrap the right choice, and when would you write your own
   CSS instead?

## Glossary (EN → BG)

| English | Български |
|---|---|
| view model | модел за изгледа |
| dynamic / strongly typed | динамичен / строго типизиран |
| dictionary (key–value) | речник (ключ–стойност) |
| CSS framework | CSS рамка |
| utility class | помощен клас |
| responsive design | отзивчив дизайн |
| viewport | видима област |
| media query | медийна заявка |
| breakpoint | точка на пречупване |
| mobile-first | първо за мобилни устройства |
| grid / row / column | мрежа / ред / колона |
| container | контейнер |
| component | компонент |
| navbar / card / badge | навигационна лента / карта / етикет |

## References

- Views and view models: https://learn.microsoft.com/aspnet/core/mvc/views/overview
- ViewData and ViewBag: https://learn.microsoft.com/aspnet/core/mvc/views/overview#pass-data-to-views
- Bootstrap 5 grid: https://getbootstrap.com/docs/5.3/layout/grid/
- Bootstrap 5 components: https://getbootstrap.com/docs/5.3/components/
- Bootstrap 5 spacing utilities: https://getbootstrap.com/docs/5.3/utilities/spacing/
- MDN — Responsive design: https://developer.mozilla.org/en-US/docs/Learn/CSS/CSS_layout/Responsive_Design
