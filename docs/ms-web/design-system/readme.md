# Microsoft Web Technologies Design System

A Fluent-inspired design system for building course materials (slides, screens, prototypes) for a course called **Microsoft Web Technologies**. No company codebase, Figma file, or brand logo was provided — the only source material was a screenshot of an unrelated startup pitch deck ("Under"), which was confirmed by the user to be irrelevant. Everything here is built from scratch, drawing on Microsoft's public Fluent 2 design language (colors, spacing, radii, shadow tiers, motion curves) as a starting aesthetic for course branding, not from any proprietary source.

**No logo was provided.** Nowhere in this system is Microsoft's actual logo drawn or approximated — the "Brand" card and templates use the plain wordmark "Microsoft Web Technologies" in type. If a real course logo or brand mark exists, drop it into `assets/` and it'll replace the wordmark.

## Sources
- `uploads/44d0f2ff9893e6d75b0643da8f4235f9.webp` — an unrelated fintech pitch deck screenshot (not used; confirmed out of scope by user).
- No Figma file, GitHub repo, or codebase was attached.

## Content fundamentals
- **Voice**: instructional and direct, second person ("you'll build...", "your API"). Avoid marketing hype.
- **Casing**: sentence case for body copy and slide titles; UPPERCASE tracked labels for small eyebrow/category tags (e.g. "MODULE 01", "REST FUNDAMENTALS").
- **Emoji**: none — Fluent/Microsoft course materials read as professional and text-first.
- **Vibe**: technical, credible, calm — a lecture slide, not a startup pitch. Short declarative sentences, concrete technical nouns.

## Visual foundations
- **Colors**: Communication Blue (`--brand-base` #0078D4) as the single primary; a shade/tint ramp for depth; warm-neutral greys (Segoe grey ramp) for text/surfaces, not pure black/white; semantic success/warning/danger/info colors used sparingly for status only.
- **Type**: Segoe UI Variable stack (falls back to system UI fonts — Segoe UI is an OS font, not embedded). Display/title weights are 600 semibold; body copy is 400 regular. Scale runs Display 68 → Caption 11.
- **Spacing**: strict 4px base grid, tokens `--space-1` (4px) through `--space-20` (80px).
- **Backgrounds**: flat brand-blue color blocks for section/title moments, not gradients or imagery; content slides sit on a warm off-white page surface (`--surface-page`).
- **Shadows**: five ambient+key elevation tiers (`--shadow-2` → `--shadow-28`), used only on `elevated` cards/dialogs — most surfaces are flat with a 1px border instead.
- **Corner radius**: small controls 4px, default controls/cards 6–8px, larger surfaces 12px, pills/switches fully rounded.
- **Motion**: standard ease `cubic-bezier(0.8,0,0.2,1)`, fast (150ms) for hover/press, normal (200ms) for reveals — subtle, no bounce/spring.
- **Hover/press**: buttons darken one step (base → hover → pressed); no lightening, no scale/shrink effects.
- **Borders**: 1px hairline borders in neutral grey define flat surfaces; no colored left-border accent cards.
- **Transparency/blur**: not used — this system favors flat, high-contrast Fluent surfaces over acrylic/blur materials.
- **Imagery**: none provided; no photography used in specimens or the deck template.

## Iconography
No icon source was provided. Glyphs are linked live from the **Fluent System Icons** CDN (`https://cdn.jsdelivr.net/npm/@fluentui/svg-icons/icons/…svg`) — Microsoft's own open-source icon set, matching the Fluent visual language used for tokens/spacing. Used sparingly: checkmarks (Checkbox), chevrons/settings (IconButton, Button icon slot), link (Tooltip demo). No emoji, no unicode glyphs, no hand-drawn SVGs.

## Fonts
No font files were supplied. `--font-sans` / `--font-sans-text` point at `'Segoe UI Variable Display'` / `'Segoe UI Variable Text'` with a full system-font fallback chain (`Segoe UI`, `-apple-system`, `Roboto`, `Arial`...) — this is intentional (Segoe UI ships with Windows and isn't freely redistributable as a web font), so text renders correctly today via OS fonts. **If you have the actual Segoe UI Variable `.woff2` files, upload them and I'll wire up `@font-face` for pixel-accurate rendering everywhere.**

## Index
- `styles.css` + `tokens/` — colors, typography, spacing, radius, shadows, motion custom properties.
- `guidelines/` — foundation specimen cards (Colors ×2, Type, Spacing, Radius & Shadow, Brand wordmark).
- `components/forms/` — Button, IconButton, Input, Checkbox, Switch.
- `components/surfaces/` — Card.
- `components/feedback/` — Badge, Tooltip.
- `components/navigation/` — Tabs.
- `templates/course-deck/` — `CourseDeck.dc.html`, a 4-slide starter deck (title, content grid, comparison, quote) for course lectures.
- `SKILL.md` — portable skill file for reuse in Claude Code.

## Components
Badge, Button, Card, Checkbox, IconButton, Input, Switch, Tabs, Tooltip.

## Intentional additions
No source defined a component inventory, so this is the standard starter set sized down for a course-materials use case (forms, a card surface, status/tooltip feedback, tabs). Select, Radio, Dialog, Toast and Tag were left out for now — say the word and I'll add them.
