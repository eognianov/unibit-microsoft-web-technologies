# Session 09 — Forms, CRUD and Validation

> Syllabus mapping: Lecture 5 (CRUD, forms, tag helpers, validation, CSRF, scaffolding)
> · ~180 min

## Learning objectives

By the end of this session students will be able to:

1. Name the four CRUD operations and map each to an HTTP method and a URL.
2. Explain the GET/POST action pair and the POST/Redirect/GET pattern.
3. Build a Razor form with tag helpers bound to a model.
4. Validate input on the server with data annotations and `ModelState`, and display the
   errors.
5. Explain CSRF and protect a form with an anti-forgery token.
6. Use scaffolding deliberately, and read what it generated.

## Prerequisites / recap

- Session 01: GET vs POST, safe methods, 302 redirects.
- Session 03: actions, model binding. Session 04: same URL, different method.
- Session 05: HTML forms named but not used. Session 08: annotations, `DbContext`, async.
- Open with: *"Everything we have built so far only reads. Today the site gains the ability
  to change — which is also the day it gains the ability to be attacked."*

## Agenda

| # | Block | Min | Mode |
|---|-------|-----|------|
| 1 | Recap + objectives | 10 | talk |
| 2 | CRUD; forms; the two-request dance | 30 | talk |
| 3 | Never trust the client; POST/Redirect/GET | 20 | talk |
| 4 | *Break* | 10 | — |
| 5 | Razor forms and tag helpers | 25 | demo |
| 6 | Validation: annotations, `ModelState`, summaries | 25 | demo |
| 7 | *Break* | 10 | — |
| 8 | CSRF and the anti-forgery token; Edit and Delete | 20 | demo |
| 9 | Lab: full CRUD for Article | 25 | lab |
| 10 | Scaffolding; wrap-up; homework | 5 | talk + demo |

---

## Part 1 — The idea (vendor-neutral) · ~50 min

### 9.1 CRUD

Nearly every data-driven application, in every language, is four operations over some
kind of record:

| Letter | Operation | HTTP | Typical URL |
|---|---|---|---|
| **C** | Create | `GET` the form, then `POST` the data | `/articles/create` |
| **R** | Read | `GET` | `/articles`, `/articles/42` |
| **U** | Update | `GET` the filled form, then `POST` the changes | `/articles/42/edit` |
| **D** | Delete | `GET` the confirmation, then `POST` the deletion | `/articles/42/delete` |

Read is already done (Sessions 03–08). Today: the other three.

### 9.2 Forms and the two-request dance

An HTML **form** is the browser's built-in way to send data to a server. Three parts:

```html
<form action="/articles/create" method="post">
  <label for="title">Title</label>
  <input type="text" id="title" name="title" />
  <button type="submit">Save</button>
</form>
```

- `action` — **where** the data goes.
- `method` — **how**: `get` (values in the query string) or `post` (values in the request
  body). Browsers support only these two, which is why delete is a `POST`.
- `name` — the key each value is sent under. **No `name`, no value sent.** This is the
  single most common beginner bug; demonstrate it.

**The crucial insight — creating one record takes two requests:**

```
1.  GET  /articles/create      → server returns an empty form         (200 + HTML)
    ... the user types ...
2.  POST /articles/create      → server receives the data, saves it   (302 redirect)
3.  GET  /articles             → the browser follows the redirect     (200 + HTML)
```

Draw this. Then state the naming convention that falls out of it: **one URL, two actions**
— one for `GET`, one for `POST`, distinguished by the HTTP method, exactly as previewed in
Session 04.

### 9.3 Never trust the client

The single most important idea of the session. Say it slowly.

The form you served has `maxlength="200"` and `required`. That HTML is **advice to a
cooperating browser**, nothing more. Anyone can:

- open DevTools and delete the `required` attribute,
- send the request with `curl` and never load your form at all,
- replay a modified request.

Therefore: **client-side validation is a convenience; server-side validation is the
security boundary.** Both are worth having — the first gives instant feedback, the second
is the one that actually protects your data. If you can only have one, keep the server.

Show it live: submit a valid form, then remove `required` in DevTools and submit an empty
one. Server-side validation catches it. This demo is worth more than any slide.

Three attacks the session touches, each in one sentence:

- **SQL injection** — user input concatenated into SQL changes the query's meaning. We are
  already safe: EF Core parameterises everything (show a generated `@__p_0` from
  Session 08). Never build SQL with string concatenation.
- **XSS (cross-site scripting)** — user input rendered as HTML runs as code. We are already
  safe: Razor encodes output by default (Session 05). `@Html.Raw` removes that protection.
- **CSRF** — §9.7.

### 9.4 POST/Redirect/GET

After a successful `POST`, do **not** return HTML directly. Return a **redirect** (302 —
Session 01), so the browser issues a fresh `GET`.

Why: if the response to a `POST` is a page, then pressing **Refresh** re-submits the
`POST`. The user creates a second article by accident. Everyone has seen the browser's
"Confirm form resubmission" dialog — that is this bug, in the wild.

```
POST /articles/create  →  302 Location: /articles/42
GET  /articles/42      →  200 HTML          ← this is what sits in the address bar
```

Now refresh does the harmless thing, the URL is bookmarkable, and the back button behaves.
This pattern has a name because it is universal: **POST/Redirect/GET**.

---

## Part 2 — In practice: .NET 10 MVC · ~70 min

### 9.5 The action pair

```csharp
// GET /articles/create — show an empty form
[HttpGet]
public IActionResult Create() => View();

// POST /articles/create — receive it
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(Article article)
{
    if (!ModelState.IsValid)
        return View(article);            // redisplay the form WITH the user's input

    _context.Articles.Add(article);
    await _context.SaveChangesAsync();

    return RedirectToAction(nameof(Index));   // POST/Redirect/GET
}
```

Points to make in order:

- Two methods, same name, different HTTP verbs — C# overloading plus `[HttpGet]` /
  `[HttpPost]`. This is what Session 04's method attributes were for.
- Model binding (Session 03) now fills a **whole object** from the form fields, matching
  by property name.
- `return View(article)` on failure, not `return View()` — otherwise the user's typing is
  erased and they will never forgive you.
- `nameof(Index)` instead of `"Index"`: a rename becomes a compile error rather than a
  broken link.
- `SaveChangesAsync` is what actually issues the `INSERT` — watch it in the SQL log.

**Over-posting**, stated with a real example: `Article` has an `Id`. If a user adds
`Id=999` to the form body, model binding sets it. On richer models the same trick can set
`IsAdmin` or `IsPublished`. Two defences:

```csharp
public async Task<IActionResult> Create([Bind("Title,Author,Summary,Body,PublishedOn")] Article article)
```

or — better, and the reason Session 06 exists — bind to a **view model** containing only
the fields the form may set, then copy to the entity. Show both; recommend the second.

### 9.6 Razor forms and tag helpers

Hand-written HTML forms are verbose and easy to desynchronise from the model. **Tag
helpers** are server-side attributes that generate the right HTML from the model:

```cshtml
@model SimpleBlog.Models.Article

<form asp-action="Create" method="post">
    <div asp-validation-summary="ModelOnly" class="alert alert-danger d-none"></div>

    <div class="mb-3">
        <label asp-for="Title" class="form-label"></label>
        <input asp-for="Title" class="form-control" />
        <span asp-validation-for="Title" class="text-danger"></span>
    </div>

    <div class="mb-3">
        <label asp-for="Author" class="form-label"></label>
        <input asp-for="Author" class="form-control" />
        <span asp-validation-for="Author" class="text-danger"></span>
    </div>

    <div class="mb-3">
        <label asp-for="Body" class="form-label"></label>
        <textarea asp-for="Body" class="form-control" rows="8"></textarea>
        <span asp-validation-for="Body" class="text-danger"></span>
    </div>

    <div class="mb-3">
        <label asp-for="PublishedOn" class="form-label"></label>
        <input asp-for="PublishedOn" class="form-control" />
        <span asp-validation-for="PublishedOn" class="text-danger"></span>
    </div>

    <button type="submit" class="btn btn-primary">Save</button>
    <a asp-action="Index" class="btn btn-link">Cancel</a>
</form>
```

**View source and read the generated HTML with the class.** From one attribute, `asp-for`
produced: the `name`, the `id`, the `value`, `type="date"` (from `[DataType(DataType.Date)]`),
`maxlength="200"` (from `[MaxLength]`), and `data-val-*` attributes carrying the validation
rules to the browser. *"You wrote your rules once, in Session 08, on the model. They are
now shaping the database, the form, the client-side check and the server-side check."*
This is the moment data annotations pay off.

| Tag helper | Does |
|---|---|
| `asp-for` | name, id, value, input type, client-validation attributes |
| `asp-action` / `asp-controller` / `asp-route-*` | generates the URL (Session 04) |
| `asp-validation-for` | the error message for one field |
| `asp-validation-summary` | `All` \| `ModelOnly` \| `None` |
| `asp-items` | `<option>`s for a `<select>` |

Client-side validation needs the jQuery validation scripts — in the template's
`_ValidationScriptsPartial.cshtml`:

```cshtml
@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
```

Show the form both with and without it: with, errors appear as you type; without, they
appear after a round trip. Same errors, different moment. Reinforce §9.3 — the server check
runs in both cases.

### 9.7 Validation on the server

The rules are already on the model from Session 08. Add the ones specific to input:

```csharp
[Required(ErrorMessage = "Please enter a title.")]
[StringLength(200, MinimumLength = 5,
    ErrorMessage = "The title must be between 5 and 200 characters.")]
public string Title { get; set; } = "";

[Required, EmailAddress]
public string ContactEmail { get; set; } = "";

[Range(0, 10_000)]
public int ReadingMinutes { get; set; }

[RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Lowercase letters, digits and hyphens only.")]
public string Slug { get; set; } = "";
```

`ModelState` is the framework's record of *what arrived and what was wrong with it*. It is
populated automatically during model binding, before your action runs.

```csharp
if (!ModelState.IsValid)
    return View(article);
```

Add your own rules — the ones annotations cannot express, because they need the database:

```csharp
if (await _context.Articles.AnyAsync(a => a.Title == article.Title))
    ModelState.AddModelError(nameof(article.Title), "An article with this title already exists.");

if (article.PublishedOn > DateTime.Today.AddYears(1))
    ModelState.AddModelError(nameof(article.PublishedOn), "That date is too far in the future.");

if (!ModelState.IsValid)
    return View(article);
```

Errors added with an empty key (`ModelState.AddModelError("", "...")`) appear in the
`ModelOnly` summary — that is what the summary at the top of the form is for.

### 9.8 CSRF and the anti-forgery token

**The attack, told as a story.** You are logged into your blog. In another tab you open a
page that contains:

```html
<form action="https://yourblog.example/articles/5/delete" method="post" id="f">
</form>
<script>document.getElementById('f').submit();</script>
```

The browser submits it — **and attaches your cookies**, because it always sends cookies for
that site. Your server sees a valid, authenticated request to delete article 5. Nothing
about the request is malformed. That is **CSRF**: cross-site request forgery.

**The defence.** Give each of your own forms a secret value that the attacker cannot know,
and reject any POST that does not carry it.

In ASP.NET Core this is nearly automatic:

- A `<form>` with a tag helper **already emits** a hidden anti-forgery field. Show it in
  View source:
  `<input name="__RequestVerificationToken" type="hidden" value="CfDJ8..." />`
- `[ValidateAntiForgeryToken]` on the POST action makes the server require and check it.

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Delete(int id) { ... }
```

Demonstrate the failure: remove the hidden field in DevTools and submit. The server returns
**400 Bad Request** — Session 01's status code, now with a reason.

Two rules to state plainly:

1. **Every state-changing POST gets `[ValidateAntiForgeryToken]`.** No exceptions in this
   course.
2. **`GET` must never change state** (Session 01). A CSRF token cannot protect a `GET`,
   because a plain `<img src="...">` can trigger one.

### 9.9 Edit and Delete

**Edit** — same two-request shape, but the form starts filled:

```csharp
[HttpGet]
public async Task<IActionResult> Edit(int id)
{
    var article = await _context.Articles.FindAsync(id);
    if (article is null) return NotFound();          // Session 03's rule
    return View(article);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, Article article)
{
    if (id != article.Id) return BadRequest();       // the URL and the body must agree

    if (!ModelState.IsValid) return View(article);

    try
    {
        _context.Update(article);
        await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!await _context.Articles.AnyAsync(a => a.Id == id)) return NotFound();
        throw;
    }

    return RedirectToAction(nameof(Index));
}
```

The `try/catch` is Session 02's exception handling with a real purpose: someone else may
have deleted the row while this user was typing. Note the hidden `<input asp-for="Id" />`
the Edit form needs, and why.

**Delete** — always two steps, never one:

```csharp
[HttpGet]
public async Task<IActionResult> Delete(int id)
{
    var article = await _context.Articles.AsNoTracking()
                                         .FirstOrDefaultAsync(a => a.Id == id);
    if (article is null) return NotFound();
    return View(article);              // a confirmation page, showing what will be lost
}

[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteConfirmed(int id)
{
    var article = await _context.Articles.FindAsync(id);
    if (article is not null)
    {
        _context.Articles.Remove(article);
        await _context.SaveChangesAsync();
    }
    return RedirectToAction(nameof(Index));
}
```

Two teaching points:

- `[ActionName("Delete")]` lets a differently-named method answer the `Delete` URL — needed
  because C# cannot have two methods with the same name and signature.
- **Never delete on a `GET`.** Session 01's warning, now concrete: a crawler following
  links would empty the database. The confirmation page is a `GET`; the deletion is a
  `POST` from a form with a token, styled `btn btn-danger`.

Mention **soft delete** in one line — real systems often set `IsDeleted = true` rather than
removing rows — as something to consider for their final project.

### 9.10 Scaffolding — after, not before

Only now, having written it by hand, show the generator:

```bash
dotnet tool install --global dotnet-aspnet-codegenerator
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design

dotnet aspnet-codegenerator controller \
    -name ArticlesController -m Article \
    -dc ApplicationDbContext --relativeFolderPath Controllers \
    --useDefaultLayout --referenceScriptLibraries
```

(In Visual Studio: right-click `Controllers` → Add → New Scaffolded Item → MVC Controller
with views, using Entity Framework.)

It produces the five actions and five views in seconds. Read the generated code together
and have the class **name every pattern they recognise**: the action pairs, `ModelState`,
`[ValidateAntiForgeryToken]`, `RedirectToAction`, `NotFound()`, `[Bind]`, the tag helpers.
Nothing in it should be a surprise.

Then say the honest thing: *"Scaffolding is a fast first draft, not a finished feature. It
binds to the entity rather than a view model, its views are plain, and it makes no product
decisions. It is useful because you can now read and fix it — which you could not have done
an hour ago."*

---

## Live demo checkpoints

| # | State | How to reach it |
|---|---|---|
| 1 | A hand-written HTML form posting to an action | plain `<form method="post">` |
| 2 | A field without `name` sending nothing | remove `name`, submit, inspect the request |
| 3 | The same form with tag helpers; generated HTML read | `asp-for` + View source |
| 4 | Server-side validation rejecting bad input | submit an empty title |
| 5 | Client-side `required` removed in DevTools; server still rejects | the §9.3 demo |
| 6 | Client-side messages appearing live | add `_ValidationScriptsPartial` |
| 7 | A custom `ModelState` error (duplicate title) | `AddModelError` |
| 8 | Successful create → 302 → list page | watch the Network tab |
| 9 | Refresh after POST **not** re-submitting | thanks to POST/Redirect/GET |
| 10 | The anti-forgery hidden field found in the HTML | View source |
| 11 | Token removed → **400 Bad Request** | delete the field in DevTools, submit |
| 12 | Edit round trip with a pre-filled form | `Edit` GET + POST |
| 13 | Delete confirmation page, then POST delete | never on GET |
| 14 | Scaffolding generating the same thing in 10 seconds | `aspnet-codegenerator` |

**SimpleBlog state after this session:** full CRUD — list, details, create, edit and delete
— on a real database, with server-side validation, client-side hints, anti-forgery tokens
on every POST, POST/Redirect/GET everywhere, and Bootstrap-styled forms.

## Common misconceptions

| Misconception | Correction |
|---|---|
| "Client-side validation is enough." | It is a convenience. Anyone can bypass it. The server is the boundary. |
| "One action per page, so one action per form." | A form needs a pair: GET to show it, POST to receive it. |
| "Return the page after a POST." | Redirect. Otherwise refresh re-submits. |
| "`return View()` after a validation failure." | `return View(model)` — otherwise the user's input is wiped. |
| "An input without `name` still submits." | It sends nothing. |
| "Model binding only fills simple parameters." | It fills whole objects by property name — including ones you did not intend (over-posting). |
| "`[ValidateAntiForgeryToken]` is optional for small sites." | CSRF does not care how small your site is. |
| "The token protects GET requests too." | It cannot. `GET` must not change state. |
| "Delete needs one action." | Two: a GET confirmation and a POST that deletes. |
| "Scaffolding writes production code." | It writes a first draft bound to the entity, with no product decisions. |
| "EF Core makes me safe from SQL injection, so I'm safe." | From that one attack. XSS, CSRF and over-posting are separate. |

## Check for understanding

1. Creating one article takes three HTTP requests. Name each one with its method, URL and
   status code.
2. Why does `return View(article)` matter more than `return View()` when validation fails?
3. A user opens DevTools and deletes `required` from your form. What stops the bad data?
4. What exactly does `asp-for="Title"` generate, and where did each generated attribute
   come from?
5. Explain CSRF to someone who has never heard of it, in three sentences. What stops it
   here?
6. Why must delete be a POST? What could go wrong with a GET?
7. Your form has a `Body` field but the model also has `Id`. How could a user set `Id`, and
   how do you prevent it?

## Lab task (in class) · 25 min

Working in your own `SimpleBlog`, implement CRUD for `Article` **by hand** (scaffolding is
for the homework):

1. `Create` — GET showing an empty form with tag helpers for every field; POST validating,
   saving with `SaveChangesAsync`, and redirecting to the list.
2. Validation: `Title` required, 5–200 characters, with a custom message; `Author`
   required; `Body` required. Add a custom `ModelState` error rejecting a duplicate title.
3. `Edit` — GET pre-filled (404 for an unknown id); POST updating and redirecting. Include
   the hidden `Id` field.
4. `Delete` — GET showing a confirmation page with the article's details; POST performing
   the deletion. Style the confirm button `btn btn-danger`.
5. Put `[ValidateAntiForgeryToken]` on **every** POST and verify the hidden field appears
   in the HTML of each form.
6. Add `_ValidationScriptsPartial` and confirm messages appear before submission.
7. Add Create / Edit / Delete buttons to the list page using `asp-action` and
   `asp-route-id`.
8. **Break it deliberately, three ways**, and record what happens: submit an empty title;
   remove `required` in DevTools and submit; delete the anti-forgery field and submit.

**Acceptance criteria:** all four operations work end to end against the database; invalid
input is rejected by the *server* with the user's input preserved and a visible message;
every POST carries and validates a token; every successful POST ends in a redirect; nothing
changes state on a GET.

## Homework

1. Run the scaffolder into a **separate** controller (e.g. `ScaffoldedArticlesController`).
   Compare it with your hand-written one and write 200 words: what did it do that you did
   not, what did you do better, and where does it use `[Bind]`?
2. Refactor `Create` and `Edit` to bind to an `ArticleFormViewModel` instead of the entity,
   and explain in writing which attack this closes.
3. Add a `Slug` field with a `[RegularExpression]` rule and a custom message. Test three
   invalid values.
4. Add a "search" form on the list page using `method="get"`. Why is `GET` correct here,
   when everything else today was `POST`?
5. Add a success message after each operation using `TempData` (research it) and display it
   in `_Layout`. Why `TempData` and not `ViewData`?

## Glossary (EN → BG)

| English | Български |
|---|---|
| CRUD | създаване, четене, редактиране, изтриване |
| form / field / submit | форма / поле / изпращане |
| tag helper | помощен етикет |
| model binding | обвързване на модела |
| over-posting | подаване на непозволени полета |
| validation (client / server) | валидация (от страна на клиента / сървъра) |
| validation summary | обобщение на грешките |
| `ModelState` | състояние на модела |
| POST/Redirect/GET | POST → пренасочване → GET |
| CSRF (cross-site request forgery) | подправяне на заявки между сайтове |
| anti-forgery token | защитен токен |
| XSS / SQL injection | XSS / SQL инжекция |
| scaffolding | автоматично генериране на код |
| soft delete | логическо изтриване |

## References

- Forms and tag helpers: https://learn.microsoft.com/aspnet/core/mvc/views/working-with-forms
- Model validation: https://learn.microsoft.com/aspnet/core/mvc/models/validation
- Prevent CSRF attacks: https://learn.microsoft.com/aspnet/core/security/anti-request-forgery
- Prevent over-posting: https://learn.microsoft.com/aspnet/core/data/ef-mvc/crud#overposting
- Scaffolding: https://learn.microsoft.com/aspnet/core/tutorials/first-mvc-app/adding-model
- OWASP Top Ten: https://owasp.org/www-project-top-ten/
