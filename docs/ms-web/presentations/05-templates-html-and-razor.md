# Session 05 — Templates: HTML, CSS and Razor

> Syllabus mapping: Lecture 3 (views, HTML/CSS, Razor, layouts) · ~180 min
> The syllabus assumes HTML and CSS are known. They are not. This session teaches them
> as first-class material.

## Learning objectives

By the end of this session students will be able to:

1. Write a valid HTML document using the common structural elements.
2. Style it with CSS selectors and explain the box model.
3. Explain server-side rendering: how a template plus data becomes HTML.
4. Write Razor views using `@`, `@if`, `@foreach` and `@model`.
5. Use a layout to avoid repeating the page chrome, and a partial view to avoid repeating
   a fragment.
6. Explain how the framework finds a view for an action.

## Prerequisites / recap

- Session 01: the response body is usually an HTML document.
- Session 03: `return View()`; the "view was not found" error and the paths it listed.
- Session 04: `asp-controller` / `asp-action` links were previewed.
- Open with the Session 03 error message on screen: *"The framework told us exactly where
  to put a file. Today we find out what goes in it."*

## Agenda

| # | Block | Min | Mode |
|---|-------|-----|------|
| 1 | Recap + objectives | 10 | talk |
| 2 | HTML: structure and the common elements | 30 | talk + live coding |
| 3 | CSS: selectors and the box model | 25 | talk + live coding |
| 4 | *Break* | 10 | — |
| 5 | Templates and server-side rendering — the idea | 15 | talk |
| 6 | Razor: syntax, `@model`, control flow | 30 | demo |
| 7 | *Break* | 10 | — |
| 8 | Layouts, partial views, sections | 20 | demo |
| 9 | Lab: give SimpleBlog real pages | 25 | lab |
| 10 | Wrap-up, homework, Q&A | 5 | talk |

---

## Part 1 — The idea (vendor-neutral) · ~70 min

### 5.1 HTML — the structure of a document

HTML is **not** a programming language. It has no decisions and no loops. It is a way of
**labelling parts of a document** so that a browser knows what they are.

**Analogy — the newspaper.** A newspaper page has a masthead, a headline, a byline, body
paragraphs, a photo with a caption, a sidebar. HTML gives each of those a name. The
*label* is the HTML; the *look* is CSS.

An element:

```html
<p class="lead">Hello</p>
└┬┘ └───┬────┘ └─┬─┘└┬┘
tag  attribute content closing tag
```

The minimum document:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>SimpleBlog</title>
</head>
<body>
    <h1>Latest articles</h1>
</body>
</html>
```

- `<head>` — information *about* the page; not displayed.
- `<body>` — what the user sees.
- `<title>` — the browser tab and the search-result heading.
- `<meta charset>` — without it, Cyrillic text turns into mojibake. Demonstrate it.
- `<meta viewport>` — required for the page to behave on a phone (Session 06).

The elements worth teaching, grouped by job:

| Job | Elements |
|---|---|
| Headings | `<h1>` … `<h6>` — one `<h1>` per page, do not skip levels |
| Text | `<p>`, `<strong>`, `<em>`, `<br>`, `<hr>` |
| Lists | `<ul>`/`<ol>` + `<li>` |
| Links | `<a href="/articles/42">Read</a>` |
| Images | `<img src="/images/x.png" alt="description">` — `alt` is not optional |
| Tables | `<table>`, `<thead>`, `<tbody>`, `<tr>`, `<th>`, `<td>` |
| Forms | `<form>`, `<input>`, `<label>`, `<textarea>`, `<button>` — Session 09 |
| Grouping | `<div>` (block), `<span>` (inline) |
| Semantics | `<header>`, `<nav>`, `<main>`, `<article>`, `<section>`, `<footer>` |

Two rules that prevent most beginner pain:

1. **Nesting must be balanced.** `<p><strong>x</strong></p>`, never
   `<p><strong>x</p></strong>`.
2. **Semantic elements over `<div>` where one exists.** `<nav>` says "navigation" to
   screen readers and search engines; `<div class="nav">` says nothing.

Live: open a blank `.html` file, build a small article page in ten lines, open it in the
browser. Then open DevTools → **Elements** and show the same document as a tree.

### 5.2 CSS — the look

CSS answers: *given elements labelled like that, how should they look?* A rule:

```css
h1 { color: darkslateblue; font-size: 2rem; }
└┬┘  └────────────────┬────────────────────┘
selector          declarations (property: value;)
```

**Selectors** — the four that cover 95 % of use:

```css
h1            { }   /* every <h1>            — by element   */
.lead         { }   /* class="lead"          — by class     */
#main-title   { }   /* id="main-title"       — by id        */
article p     { }   /* <p> inside <article>  — descendant   */
```

Teach the preference: **classes** for styling, ids sparingly, elements for broad defaults.

**The box model** — draw it, then show it in DevTools' Computed panel:

```
┌─────────── margin ───────────┐   space outside, between boxes
│ ┌───────── border ─────────┐ │
│ │ ┌─────── padding ──────┐ │ │   space inside, around the content
│ │ │      content         │ │ │
│ │ └──────────────────────┘ │ │
│ └──────────────────────────┘ │
└──────────────────────────────┘
```

Say the one thing everyone gets wrong: **padding is inside the border, margin is outside.**
Then show the fix every real project uses:

```css
*, *::before, *::after { box-sizing: border-box; }
```

*"Without this, a box declared `width: 200px` with padding is not 200px wide. With it, it
is. Every framework, including Bootstrap, sets this."*

Three ways to attach CSS, and which to use:

```html
<p style="color:red">inline — avoid</p>
<style> p { color: red; } </style>            <!-- in <head> — fine for one page -->
<link rel="stylesheet" href="/css/site.css" /> <!-- external — do this -->
```

Connect to Session 01: that `<link>` causes **a second HTTP request**. Show it in the
Network tab. Then connect to Session 02: the file lives in `wwwroot/css/site.css`, and
`app.UseStaticFiles()` in `Program.cs` is what allows the browser to fetch it.

### 5.3 Templates and server-side rendering

Now the problem this session actually exists to solve. Our controller has three articles
in a list. The browser needs HTML. Who converts one into the other?

The bad answer (Session 03's opening slide) — glue strings together in the controller:

```
html = "<h1>" + article.Title + "</h1><p>" + article.Body + "</p>"
```

Unreadable, unmaintainable, and a security hole (Session 09).

The good answer: a **template** — an HTML file with *holes* in it, plus a **template
engine** that fills the holes with data.

```
Template  +  Data  →  [ template engine ]  →  HTML  →  response body
(static)     (varies)                         (final)
```

Key idea, and it deserves a full minute: **this all happens on the server, before the
response is sent.** The browser never sees the template, the placeholders, or the loop. It
receives finished HTML. That is **server-side rendering**. Prove it in the demo with
DevTools' *View source*, which shows what arrived — not the Elements panel, which shows
the live tree.

Every ecosystem has one: Razor (.NET), Thymeleaf/JSP (Java), Jinja/Django templates
(Python), Blade/Twig (PHP), EJS/Handlebars (Node). The syntax differs; the idea is
identical.

---

## Part 2 — In practice: Razor · ~50 min

### 5.4 The view-lookup convention

From Session 03's error message, made explicit:

```
ArticlesController.Index()  →  Views/Articles/Index.cshtml
                                     └──┬───┘ └──┬─┘
                              controller name  action name
                            (without "Controller")
```

If not found there, the framework looks in `Views/Shared/`. That fallback is what makes
shared partials work.

`.cshtml` = **C# + HTML** in one file. Create the folder and the file, and the Session 03
error disappears.

### 5.5 Razor syntax

**One symbol to learn: `@`.** It means "switch from HTML to C# here." Razor works out
where the C# ends by itself.

```cshtml
@* a Razor comment — never reaches the browser *@

<p>Today is @DateTime.Now.ToShortDateString()</p>          @* an expression *@
<p>2 + 2 = @(2 + 2)</p>                                    @* parentheses when ambiguous *@

@{
    var greeting = "Hello";                                 @* a code block *@
    var count = 3;
}
<p>@greeting — @count articles</p>

<p>Email: name@("@")example.com</p>                        @* escaping a literal @ *@
```

**Conditionals**

```cshtml
@if (Model.Count == 0)
{
    <p class="text-muted">No articles yet.</p>
}
else
{
    <p>@Model.Count article(s)</p>
}
```

**Loops** — the workhorse:

```cshtml
<ul>
@foreach (var article in Model)
{
    <li>
        <a asp-action="Details" asp-route-id="@article.Id">@article.Title</a>
        <small>@article.PublishedOn.ToString("d MMM yyyy")</small>
    </li>
}
</ul>
```

Note how HTML and C# alternate with no ceremony — inside `{ }` you are in C#, inside a tag
you are in HTML, `@` switches back. Say plainly that this feels strange for one day and
natural afterwards.

**Two rules to state now**, because they cause real bugs:

1. Razor **HTML-encodes output by default**. If `Title` is `<script>alert(1)</script>`, the
   page shows that text; it does not run it. Demonstrate this — set a title containing a
   tag and show the escaped output. `@Html.Raw(...)` disables the protection: *"if you
   ever type that, be certain you know where the string came from."* (Full treatment in
   Session 09.)
2. **Views must not fetch data.** No database calls, no business rules. If a view needs
   something, the controller supplies it. This is Session 03's traffic rule, enforced.

### 5.6 `@model` — typed views

Without a declared model a view has no idea what it was given. With one, it does — and so
does the editor:

```cshtml
@model List<SimpleBlog.Models.Article>

<h1>Articles</h1>
@foreach (var a in Model)
{
    <p>@a.Title</p>
}
```

- `@model` (lowercase) **declares** the type — once, at the top.
- `Model` (uppercase) **is** the object the controller passed.
- The payoff: IntelliSense and compile-time errors instead of typos discovered by users.
  Demonstrate by typing `@a.Titel` and showing the red squiggle.

And in the controller — this is the line that connects the two sessions:

```csharp
public IActionResult Index() => View(_articles);
```

### 5.7 Layout: stop repeating the chrome

Every page needs `<html>`, `<head>`, the nav bar, the footer. Copying that into ten views
means fixing every bug ten times.

`Views/Shared/_Layout.cshtml` — the frame:

```cshtml
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>@ViewData["Title"] — SimpleBlog</title>
    <link rel="stylesheet" href="~/css/site.css" />
</head>
<body>
    <header>
        <nav>
            <a asp-controller="Articles" asp-action="Index">Articles</a>
            <a asp-controller="Home" asp-action="About">About</a>
        </nav>
    </header>

    <main>
        @RenderBody()          @* ← the individual view is injected here *@
    </main>

    <footer>&copy; @DateTime.Now.Year SimpleBlog</footer>

    <script src="~/js/site.js"></script>
    @await RenderSectionAsync("Scripts", required: false)
</body>
</html>
```

And in a view:

```cshtml
@model List<SimpleBlog.Models.Article>
@{
    ViewData["Title"] = "Articles";
    Layout = "_Layout";          // usually set globally in _ViewStart.cshtml
}

<h1>Articles</h1>
...
```

Say it as an inversion, echoing Session 02: *"The view does not include the layout. The
layout wraps the view."*

Supporting files, each with one job:

| File | Job |
|---|---|
| `Views/_ViewStart.cshtml` | runs before every view — sets `Layout` once for all |
| `Views/_ViewImports.cshtml` | shared `@using` and `@addTagHelper` — why `asp-*` works |
| `Views/Shared/_Layout.cshtml` | the page frame |

The leading underscore is a convention meaning "not a page — a building block."

**Sections** let a view push content into a specific slot of the layout:

```cshtml
@section Scripts {
    <script src="~/js/articles.js"></script>
}
```

`required: false` in the layout means a view may omit it.

### 5.8 Partial views: stop repeating a fragment

A partial is a reusable *fragment* of markup — the same idea as a method in Session 02,
applied to HTML.

`Views/Shared/_ArticleCard.cshtml`:

```cshtml
@model SimpleBlog.Models.Article

<div class="article-card">
    <h2><a asp-action="Details" asp-route-id="@Model.Id">@Model.Title</a></h2>
    <p class="meta">@Model.Author · @Model.PublishedOn.ToString("d MMM yyyy")</p>
    <p>@Model.Summary</p>
</div>
```

Used from any view:

```cshtml
@foreach (var article in Model)
{
    <partial name="_ArticleCard" model="article" />
}
```

Layout vs. partial, in one line each: **a layout wraps a page; a partial is dropped into
one.**

---

## Live demo checkpoints

| # | State | How to reach it |
|---|---|---|
| 1 | A hand-written HTML page in the browser | plain `.html` file, opened directly |
| 2 | The same page styled; box model shown in DevTools | add `site.css`, inspect Computed |
| 3 | Session 03's "view not found" error resolved | create `Views/Articles/Index.cshtml` |
| 4 | Articles rendered from the controller's list | `View(_articles)` + `@model` + `@foreach` |
| 5 | View source proving the loop ran on the server | right-click → View page source |
| 6 | HTML-encoding demonstrated | give an article a `<script>` title |
| 7 | Layout extracted; nav and footer on every page | `_Layout.cshtml` + `_ViewStart.cshtml` |
| 8 | `_ArticleCard` partial reused in the list | `<partial name="_ArticleCard" ... />` |
| 9 | A `Scripts` section pushed into the layout | `@section Scripts { ... }` |

**SimpleBlog state after this session:** real HTML pages — `Views/Articles/Index.cshtml`
and `Details.cshtml`, a shared `_Layout` with navigation and footer, an `_ArticleCard`
partial, and `wwwroot/css/site.css`. Data still comes from the static in-memory list.

## Common misconceptions

| Misconception | Correction |
|---|---|
| "HTML is a programming language." | It is a markup language: structure only, no logic. |
| "The browser runs the `@foreach`." | The server runs it. The browser receives finished HTML. |
| "`@model` and `Model` are the same word." | `@model` declares the type; `Model` is the object. |
| "Views can query the database if it's convenient." | They must not. Session 03's traffic rule applies. |
| "Padding and margin are interchangeable." | Padding is inside the border; margin is outside. |
| "The view includes the layout." | The layout wraps the view via `@RenderBody()`. |
| "A partial view is a small page." | It is a fragment; it has no layout and no URL of its own. |
| "Razor output needs manual escaping." | It escapes by default. `@Html.Raw` *removes* that protection. |
| "Elements panel = what the server sent." | Elements is the live DOM. *View source* is what arrived. |

## Check for understanding

1. `ArticlesController.Details()` calls `return View();`. Which file is rendered, and
   where else does the framework look if it is missing?
2. A `<div>` has `width: 300px; padding: 20px; border: 2px solid`. How wide is it on
   screen — with and without `box-sizing: border-box`?
3. What is the difference between `@model` and `Model`? What breaks if you omit the first?
4. An article title is `<b>Breaking</b>`. What does the user see, and why?
5. When would you use a layout, and when a partial? Give one example of each from a blog.
6. How many HTTP requests does a page with one stylesheet, two images and one script make?

## Lab task (in class) · 25 min

Working in your own `SimpleBlog`:

1. Create `Views/Articles/Index.cshtml` with `@model List<Article>`; change the controller
   to `return View(_articles);`.
2. List every article with its title, author and formatted date; wrap each in an
   `<article>` element. Show "No articles yet." when the list is empty (test it).
3. Create `Views/Articles/Details.cshtml` showing one article in full; link to it from the
   list with `asp-action` / `asp-route-id` — no hard-coded `href`.
4. Edit `_Layout.cshtml`: give the site a header with navigation (Articles, About) and a
   footer with the current year. Set `ViewData["Title"]` per page and confirm the browser
   tab changes.
5. Extract the per-article markup into `Views/Shared/_ArticleCard.cshtml` and use
   `<partial>` in the list.
6. Add three rules to `wwwroot/css/site.css` — one element selector, one class selector,
   one descendant selector — and confirm in DevTools that they apply.
7. **View source** on your list page and find the `<li>` your `@foreach` produced.

**Acceptance criteria:** both views render with a shared layout; the list uses the partial
and generated links; the empty case is handled; page source shows plain HTML with no Razor
syntax anywhere.

## Homework

1. Give an article the title `<script>alert('hi')</script>` and describe what appears on
   the page. Then wrap it in `@Html.Raw()` and describe what happens. Which behaviour is
   the safe default, and why? **Revert the `Html.Raw` change.**
2. Add an `_ArticleMeta` partial (author + date + reading time) and use it in both views.
3. Add a `@section Scripts` block to `Details.cshtml` that logs the article id to the
   console. Verify it in DevTools.
4. Style your article cards with padding, a border and a margin, then use DevTools'
   Computed panel to write down the element's total width. Explain the number.

## Glossary (EN → BG)

| English | Български |
|---|---|
| markup language | език за маркиране |
| element / tag / attribute | елемент / етикет / атрибут |
| nesting | влагане |
| semantic element | семантичен елемент |
| selector | селектор |
| box model | кутиен модел |
| padding / border / margin | вътрешен отстъп / рамка / външен отстъп |
| stylesheet | стилов файл |
| template / template engine | шаблон / шаблонен двигател |
| server-side rendering | рендиране от страна на сървъра |
| view | изглед |
| layout | глобален изглед / оформление |
| partial view | частичен изглед |
| section | секция |
| HTML encoding / escaping | HTML кодиране / екраниране |
| static files | статични файлове |

## References

- MDN — HTML basics: https://developer.mozilla.org/en-US/docs/Learn/Getting_started_with_the_web/HTML_basics
- MDN — CSS first steps: https://developer.mozilla.org/en-US/docs/Learn/CSS/First_steps
- MDN — The box model: https://developer.mozilla.org/en-US/docs/Learn/CSS/Building_blocks/The_box_model
- Razor syntax reference: https://learn.microsoft.com/aspnet/core/mvc/views/razor
- Layouts: https://learn.microsoft.com/aspnet/core/mvc/views/layout
- Partial views: https://learn.microsoft.com/aspnet/core/mvc/views/partial
