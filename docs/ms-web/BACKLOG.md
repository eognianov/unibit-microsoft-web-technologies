# Backlog — Microsoft Web Technologies course preparation

Status: `[ ]` todo · `[~]` in progress · `[x]` done

## Scope & decisions (settled — do not re-litigate)

- **10 sessions**, ~3 h each (≈30 lecture hours), 2 per syllabus lecture.
- **English** content; each session carries an EN → BG glossary for students.
- **Concept first, vendor-neutral; then concrete in .NET 10 MVC.** No Microsoft vocabulary
  in any `## Part 1` block.
- **Audience may have zero programming background.** Session 02 carries that load.
- **Demo app: SimpleBlog** (`Article` entity), not the syllabus' `LocalLibrary`/`Book`.
- **SQLite by default** so macOS/Linux/Windows work identically; SQL Server LocalDB
  documented as the Windows alternative in Session 07.
- **VS Code + C# Dev Kit** as the baseline editor; Visual Studio shown where the syllabus
  requires it (debugger, scaffolding, publish profiles).
- Source syllabus files (`program.md`, `01-microsoft-web-technologies-bachelors.md`) are
  **not** modified.

## E1 — Session outlines

- [x] `presentations/README.md` — index, principles, tooling, timing convention
- [x] 01 How the Web Works
- [x] 02 From Language to Framework
- [x] 03 Handling a Request
- [x] 04 Routing and URL Design
- [x] 05 Templates: HTML, CSS and Razor
- [x] 06 View Models and Bootstrap
- [x] 07 Data, Databases and ORMs
- [x] 08 Code-First and Migrations
- [x] 09 Forms, CRUD and Validation
- [x] 10 Debugging, Logging and Deployment

## E2 — Demo app (SimpleBlog)

- [ ] Build the app for real, session by session, verifying every code snippet compiles on .NET 10
- [ ] Tag / branch one state per session (`session-03`, `session-04`, …) so students can catch up
- [ ] Verify the `dotnet ef` and `aspnet-codegenerator` commands on macOS and on Windows
- [ ] Prepare the planted bug for Session 10's debugger demo
- [ ] Publish once to Azure App Service end to end, and record the actual steps taken

## E3 — Slides

- [x] Choose the pipeline (Marp / reveal.js / PowerPoint) and set up a template — settled
  on self-contained HTML decks (`presentations/slides/NN-*.html`), no build step; see
  `CLAUDE.md` "Building a deck" for the stage/scale/print mechanics.
- [x] Convert each session's Markdown into slides — all ten decks exist
  (`01-how-the-web-works.html` … `10-debugging-logging-deployment.html`), 36–51 slides
  each, all passing the well-formedness, banned-wording and overflow checks.
- [x] Draw the recurring diagrams: request/response (01), MVC traffic rule (03), request
  lifecycle (03), box model (05), 12-column grid (06), POST/Redirect/GET (09), migration
  timeline (08). Supporting diagrams also added: URL/route matching (04), template
  substitution and layout composition (05), breakpoints and view-model shaping (06),
  object↔table mapping and the redrawn client–server–database diagram (07), code-first
  round trip (08), the two validation walls and the CSRF round trip (09), the debugging
  loop / log-level filter / publish pipeline (10).

## E4 — Labs & assessment

- [ ] Starter repo per lab where the lab does not simply continue the previous session
- [ ] Solution branches for every lab, kept private
- [ ] Final-project brief as a standalone handout (currently only in Session 10 §10.12)
- [ ] Grading rubric as a usable marking sheet
- [ ] Continuous-assessment scheme: which labs are graded, and how much each is worth

## E5 — Handouts

- [ ] Install guide (SDK, editor, `dotnet-ef`, SQLite viewer) — must reach students **before** Session 01's homework
- [ ] Consolidated EN → BG glossary across all ten sessions
- [ ] Cheat sheets: HTTP status codes; Razor syntax; `dotnet ef` commands; Bootstrap grid & utilities
- [ ] "How to ask for help" one-pager (reproduce, read the error, `dotnet --info`)

## Outline defects found while building E3 decks

Not fixed — the outlines (`presentations/*.md`) are source of truth and weren't touched.
The decks work around each of these; the outline itself still needs the correction.

- [ ] `01-how-the-web-works.md` — `presentations/README.md`'s stale "Homework" template
  entry and the syllabus-style wording throughout need the intensive-format pass that
  deck 01 already got in prose (see `CLAUDE.md`).
- [ ] `02-from-language-to-framework.md` — the SDK install can't be homework the night
  before if Session 02 runs the same day as Session 01; needs a stated in-day plan
  (e.g. install during the between-session break, verified at the door).
- [ ] `03-handling-a-request.md` — the lab asks for a `Summary` field on `Article`, but
  the session's own demo `Article` class never declares one.
- [ ] `04-routing-and-url-design.md` — §4.4 registers the archive route at the site root
  (`/archive/2026/3`) but the lab's acceptance criteria use a prefixed path
  (`/articles/archive/2026/13`); and §4.6 pairs `{id:int}` with an unconstrained
  `{slug}` while the lab asks for "letters and hyphens", which needs `:regex(...)` —
  a constraint the session never introduces.
- [ ] `06-view-models-and-bootstrap.md` — the agenda's block minutes sum to 170, not the
  180 the timing convention specifies.
- [ ] `07-data-databases-and-orms.md` — §7.8's LocalDB connection string
  (`Server=(localdb)\mssqllocaldb;…`) needs the doubled backslash for `appsettings.json`
  made explicit, since students will copy it as written; and §7.2's `Authors` example
  and the forward reference to Session 08's `Category` are two different entities
  presented as if interchangeable.
- [ ] `08-code-first-and-migrations.md` — §8.7's rollback command
  (`dotnet ef database update AddTagsToArticle` right after applying that migration) is
  a no-op, not a rollback — it should target the prior migration. §8.4 also documents
  `[MaxLength(n)]` as mapping to `nvarchar(n)`, which is the SQL Server behavior; on the
  course's default SQLite it maps to `TEXT`. Also: `dotnet-ef` is never installed
  anywhere in Sessions 01–08 even though the CLI is used from §8.5 on — belongs in the
  E5 install guide.

## Open questions (need the lecturer's decision)

- [ ] Exam format — is the final project the whole grade, or is there a written exam too?
- [ ] Is the `Category` entity (one-to-many, Session 08 §8.10) in scope, or extension only?
- [ ] Are Azure accounts available to students for Session 10, or is deployment demo-only?
- [ ] Do the 30 exercise hours run as separate lab sessions, and does someone else teach them?
- [ ] Is authentication (ASP.NET Core Identity) expected anywhere, or explicitly out of scope?
- [ ] Do slides need to be in Bulgarian even though the outlines are in English?

## Next up

1. E2 — build SimpleBlog through Session 04 and verify every snippet compiles.
2. E5 — install guide, needed before the first class.
3. Answer the open questions above; they affect E4.
