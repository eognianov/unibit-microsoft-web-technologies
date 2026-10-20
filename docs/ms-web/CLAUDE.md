# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this directory is

Course-preparation material for **Microsoft Web Technologies**, a 10-session university
course taught in ASP.NET Core MVC on .NET 10. This directory holds *content*, not code:
session outlines, a design system, and presentation decks. The .NET projects that the
course builds toward live in sibling directories at the repo root (`FootballTeamsManager`,
`MoviesWebApp`, `RecipeVaultWithDb`, …) and are unrelated to the files here.

There is no build, test or lint step. The only tooling need is a static file server to
preview decks (see below).

## Layout

| Path | Role |
|---|---|
| `program.md`, `01-microsoft-web-technologies-bachelors.md` | The official syllabus, in Bulgarian. **Source material — never modify.** |
| `presentations/*.md` | The ten session outlines. These are the source of truth for content. |
| `presentations/README.md` | Session index, teaching principles, tooling baseline, timing convention. |
| `presentations/slides/*.html` | Presentation decks, generated from the session outlines. |
| `design-system/` | Fluent-inspired tokens and component specs used by the decks. |
| `BACKLOG.md` | Settled decisions and open questions. Read the "Scope & decisions" block before proposing changes — those are deliberately not up for re-litigation. |

Nothing in `docs/ms-web/` is committed to git yet; the whole directory is untracked.

## Delivery format (supersedes the docs)

The course runs as an **intensive: 3 back-to-back days, ~10 hours a day** — roughly three
sessions per day, not one session per week. Consequences that the checked-in docs do not
yet reflect:

- **There is no homework.** `presentations/README.md` still lists "Homework" in its session
  template and `BACKLOG.md` still refers to Session 01 homework; both are stale.
- **Labs are demonstrated from the front**, not worked by students in pairs. Lab slides are
  walkthroughs the lecturer performs on screen.
- **Slide voice is first-person plural** — "we install the toolchain", "we trigger a 404",
  "by the end we will have…". Avoid "today", "next week", "bring to class", "hand in".
- Session 02 follows Session 01 the *same day*, so the SDK install cannot be an overnight
  task. Deck 01 currently says "before we begin" without committing to how that is solved.

Only deck 01 has been reworded so far. Decks 02–10 do not exist yet and should be written
in this voice from the start.

## Writing session content

Every session file follows a fixed template (listed in `presentations/README.md`) so that
sessions can be scanned and timed identically. Two rules carry most of the weight:

- **Concept first, vendor-neutral; then concrete in .NET.** No Microsoft vocabulary is
  allowed anywhere in a `## Part 1` block — those ideas must transfer to any stack.
- **Assume zero programming background.** No term is used before the session that defines
  it, and forward references are explicit.

The running demo app is **SimpleBlog** (an `Article` entity), which replaces the syllabus'
`LocalLibrary`/`Book` one-to-one. SQLite is the default database so macOS, Linux and
Windows behave identically.

## Building a deck

Decks are **self-contained HTML files**, one per session, at
`presentations/slides/NN-session-name.html`. This settles the open pipeline choice in
`BACKLOG.md` E3 — no Marp, no reveal.js, no PowerPoint.

Structure: a fixed 1280×720 "stage" of `<section class="slide">` elements, scaled to the
viewport by a JS `transform: scale()`. Arrow keys / click navigate, `F` fullscreens, `P`
prints, and `#13` in the URL deep-links to a slide. A `@media print` block with
`@page {size: 1280px 720px}` gives one landscape page per slide for PDF export.

The deck links `../../design-system/styles.css` directly, so every color, size, space,
radius, shadow and easing resolves to a live token — retheming the design system
rethemes the decks. Component classes (`.card`, `.badge`) mirror the JSX specs in
`design-system/components/`. Follow the content rules in `design-system/readme.md`:
sentence-case titles, UPPERCASE tracked eyebrows, no emoji, flat brand-blue color blocks
for title and section slides, no gradients or imagery.

One deliberate deviation: the design system's type scale is sized for UI density
(`--fs-body` is 14px), which is unreadable projected. Decks declare `--fs-slide-*`
overrides at the top of the file — body copy at 20px — and every other value stays on
the shared tokens.

### Previewing and verifying

`file://` works, but a server avoids surprises:

```bash
python3 -m http.server 8731        # from docs/ms-web/
# http://localhost:8731/presentations/slides/01-how-the-web-works.html
```

**Chrome caches these files aggressively.** After an edit, add or bump a `?v=N` query
string or you will screenshot the previous version and conclude your fix did nothing.

Slides silently clip rather than scroll, so check every slide for overflow after any
content change, in the page console:

```js
[...document.querySelectorAll('.slide')].forEach((s,i)=>{
  const p=s.className; s.classList.add('is-active');
  const oy=s.scrollHeight-s.clientHeight, ox=s.scrollWidth-s.clientWidth;
  if(oy>1||ox>1) console.log(`slide ${i+1}: y+${oy} x+${ox}`);
  s.className=p;
});
```

### Layout traps, all hit in practice

- `.cols` sets `min-height:0`, so an inline `flex:0` on it resolves to `flex:0 1 0%` and
  **collapses the grid to zero height**. Use the `.cols--auto` class instead.
- CSS custom properties do not work in SVG *presentation attributes*
  (`fill="var(--brand-base)"` renders as nothing). Put them in `style="fill:var(…)"`.
- An SVG with `width="100%"` derives its height from the viewBox aspect ratio and ignores
  its own `height` attribute. Size diagrams by height (`style="width:auto;height:236px"`)
  so they cannot blow the slide.
- A `display:inline-flex` badge stretches full-width inside a flex-column card; it needs
  `align-self:flex-start`.
- Give any `<p>` used at display sizes an explicit `margin:0` — the browser default is
  1em, which at 40px injects ~80px of dead space.

### Diagrams

Draw them as inline SVG on design tokens rather than importing images: they restyle with
the system, stay sharp on a projector, and survive print-to-PDF. Deck 01 has two —
the client–server request/response diagram (slide 10) and the one-page-many-requests
fan-out (slide 13). Keep numbers used in diagrams consistent with the numbers in the
session's "Check for understanding" questions; slide 13's 87 requests is the callback for
question 5.

## Known doc discrepancies

`design-system/readme.md` indexes a `SKILL.md` and a `templates/course-deck/` starter deck.
Neither exists on disk.
