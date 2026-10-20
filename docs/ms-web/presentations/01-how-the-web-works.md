# Session 01 — How the Web Works

> Syllabus mapping: Lecture 1 (HTTP) · ~180 min (incl. two 10-min breaks)
> **No code in this session.** The tool is the browser.

## Learning objectives

By the end of this session students will be able to:

1. Explain the client–server model and say who does what when a page loads.
2. Read a URL and name each of its parts.
3. Describe the structure of an HTTP request and an HTTP response.
4. Choose the correct HTTP method (GET, POST, PUT, DELETE) for a given action.
5. Interpret the common status codes 200, 301, 302, 400, 401, 403, 404, 500.
6. Inspect real traffic in the browser's DevTools Network tab.

## Prerequisites / recap

None. This is the first session; assume no programming background whatsoever.
The only requirement is a browser.

## Agenda

| # | Block | Min | Mode |
|---|-------|-----|------|
| 1 | Course intro: what we will build, how we will be assessed | 15 | talk |
| 2 | The client–server model | 25 | talk |
| 3 | Anatomy of a URL | 15 | talk |
| 4 | *Break* | 10 | — |
| 5 | The HTTP request | 25 | talk |
| 6 | The HTTP response and status codes | 25 | talk |
| 7 | *Break* | 10 | — |
| 8 | Demo: watching real requests in DevTools | 20 | demo |
| 9 | Lab: investigate a website | 30 | lab |
| 10 | Wrap-up, homework, Q&A | 5 | talk |

---

## Part 1 — The idea (vendor-neutral) · ~90 min

### 1.1 Course intro

- What we will build over 10 sessions: a small blog / news website with a database
  behind it, that you can add, edit and delete articles in.
- How the course works: each session is *idea first*, *technology second*. Ideas
  transfer to any language or stack; the technology we use to practise them is
  Microsoft's .NET.
- Assessment: in-class practical tasks + a final project you defend.
- Housekeeping: what to install before Session 02 (see Homework).

### 1.2 The client–server model

**Analogy — the restaurant.** You (the *client*) sit at a table and read a menu. You
give the waiter an order. The kitchen (the *server*) prepares it. The waiter brings back
either your dish, or a message: "sorry, we're out of that." You never enter the kitchen;
you never see how the dish is made. You only send requests and receive responses.

Key points to land:

- Two roles, not two machines: **client** = the one who asks; **server** = the one who
  answers. Your laptop can be both at once.
- The browser is a client. So is a mobile app, so is a smart TV, so is another program.
- The server is a program that is *always listening* on a machine that is always on.
- Communication is **request → response**, always in that order. The server never speaks
  first.
- Each request is **independent** — the server does not remember the previous one by
  default. (This is called *statelessness*; how sites "remember" you is a later topic.)

**Whiteboard diagram.** Two boxes, `Browser` and `Server`, with an arrow right labelled
`request` and an arrow left labelled `response`. Then add a third box behind the server,
`Database` — "we will get to this in Session 07."

Ask the room: *what happens between clicking a link and seeing the page?* Collect
answers, then walk through the real sequence:

1. You type/click a URL.
2. The browser resolves the domain name to an address (**DNS** — like a phone book).
3. The browser opens a connection to that address.
4. The browser sends a **request**.
5. The server does some work and sends a **response** — usually an HTML document.
6. The browser reads the HTML, discovers it needs more files (CSS, images, scripts) and
   sends **more requests** for each one.
7. The browser draws the page.

> Emphasise step 6. Beginners think one page = one request. One page is typically
> 20–100 requests. We will see this in the demo.

### 1.3 What is HTTP?

- Both sides must agree on the *format* of the messages. That agreement is a
  **protocol**.
- **HTTP** = HyperText Transfer Protocol — the rules for how a web request and a web
  response are written down.
- It is **text-based**: a request is human-readable. That is why we can learn it by
  reading it.
- **HTTPS** is the same protocol inside an encrypted tunnel. Same rules, private channel.
  Everything in this course applies identically.

### 1.4 Anatomy of a URL

Dissect one on the board, piece by piece:

```
https://news.example.com:443/articles/42?lang=en&sort=new#comments
└─┬─┘   └───────┬───────┘└┬┘└────┬─────┘└───────┬────────┘└───┬───┘
scheme       host        port   path        query string   fragment
```

| Part | Meaning | Note |
|---|---|---|
| scheme | which protocol | `http`, `https` |
| host | which server | resolved via DNS |
| port | which door on that server | usually implied: 80 / 443 |
| path | which resource on that server | this is what we will design in Session 04 |
| query string | extra named parameters | `key=value`, joined by `&` |
| fragment | position *within* the page | **never sent to the server** |

Two facts worth stressing:

- The **path** is the part *our* application will be responsible for interpreting.
- The **fragment** stays in the browser. This surprises people later.

### 1.5 The HTTP request

Show a raw request on a slide and read it line by line:

```http
GET /articles/42?lang=en HTTP/1.1
Host: news.example.com
User-Agent: Mozilla/5.0 ...
Accept: text/html
Cookie: session=abc123

```

Four parts:

1. **Method** — *what do you want done?* (`GET`)
2. **Path (+ query)** — *to which resource?* (`/articles/42?lang=en`)
3. **Headers** — *metadata about the request*: who is asking, what formats they accept,
   what language they prefer, what cookies they carry. Name-value pairs, one per line.
4. **Body** — *the data being sent*, if any. A `GET` has no body. A form submission does.

### 1.6 HTTP methods

Introduce them as **verbs of intent**, not as syntax:

| Method | Intent | Has body? | Safe? | Idempotent? |
|---|---|---|---|---|
| `GET` | give me this resource | no | yes — changes nothing | yes |
| `POST` | here is data, create/process it | yes | no | no |
| `PUT` | replace this resource with what I send | yes | no | yes |
| `DELETE` | remove this resource | usually no | no | yes |

- **Safe** = does not change anything on the server. A `GET` must never delete something.
- **Idempotent** = doing it twice has the same effect as doing it once.
- Why it matters, concretely: a browser will happily re-issue a `GET` (back button,
  refresh, prefetch). If your "delete" is a `GET`, a search-engine crawler can wipe your
  database just by following links. This is a real thing that has happened to real sites.
- In this course, browsers' HTML forms only send `GET` and `POST`. `PUT` and `DELETE`
  matter for APIs — we name them now, use them later.

### 1.7 The HTTP response

```http
HTTP/1.1 200 OK
Content-Type: text/html; charset=utf-8
Content-Length: 1543

<!DOCTYPE html>
<html>...</html>
```

Three parts: **status line**, **headers**, **body**.

The body is *whatever was asked for* — an HTML page, an image, a stylesheet, JSON data.
`Content-Type` tells the browser how to interpret those bytes.

### 1.8 Status codes

Teach the **families** first, then the members. The first digit is the whole story:

| Family | Meaning | Mnemonic |
|---|---|---|
| `1xx` | informational | "hold on" |
| `2xx` | success | "here you go" |
| `3xx` | redirection | "it's over there" |
| `4xx` | client error | "*you* made a mistake" |
| `5xx` | server error | "*we* made a mistake" |

The ones from the syllabus, with a one-line scenario each:

| Code | Name | When you see it |
|---|---|---|
| `200` | OK | the normal case |
| `301` | Moved Permanently | the site moved to a new address, forever — update your links |
| `302` | Found | temporary redirect — e.g. after submitting a form (Session 09) |
| `400` | Bad Request | the request itself is malformed |
| `401` | Unauthorized | you are not logged in |
| `403` | Forbidden | you *are* logged in, but you're not allowed |
| `404` | Not Found | no such resource |
| `500` | Internal Server Error | our code crashed |

> The 401 vs 403 distinction is worth 60 seconds: *"who are you?"* versus
> *"I know who you are, and no."*
> The 4xx vs 5xx distinction is worth more: it tells you **whose bug it is**. When your
> app returns 500 in Session 10, that is your code throwing an exception.

---

## Part 2 — In practice: seeing it for real · ~20 min

No .NET yet — we have nothing to run. Instead, make the abstract concrete with the tool
every student already has.

### Demo: the DevTools Network tab

1. Open the browser, press `F12` (or `Cmd+Option+I`), select **Network**.
2. Tick **Preserve log** and **Disable cache**.
3. Load a content-heavy site (a news site works well). Let the request list fill up.
4. Point at the row count: *"one page — how many requests?"*
5. Click the **first** row (the HTML document). Show:
   - **Headers** panel → request method, request URL, status code.
   - **Request Headers** → `Host`, `User-Agent`, `Accept`, `Cookie`.
   - **Response Headers** → `Content-Type`, `Content-Length`.
   - **Response** panel → the raw HTML the server sent. *"This is what our app will
     produce for the rest of the course."*
6. Filter by type: **Doc**, **CSS**, **JS**, **Img**. One page, many kinds of resource.
7. Type a deliberately wrong URL on the same site → show the **404** row in red.
8. Visit an `http://` URL that redirects to `https://` → show the **301/302** row and the
   `Location` header, then the follow-up request.
9. Log in somewhere (or open a members-only page in a private window) → show a **401** or
   **403**.

Optional (fast, high-impact): open a terminal and run

```bash
curl -i https://example.com
```

The whole response — status line, headers, blank line, body — in one screen, with no
browser in the way. *"The browser is not magic. It sends text and reads text."*

---

## Live demo checkpoints

| # | State | How to reach it |
|---|---|---|
| 1 | DevTools open, Network tab recording | `F12` → Network → Preserve log |
| 2 | A 200 response inspected header by header | click the document row |
| 3 | A 404 shown | request a nonexistent path |
| 4 | A 301/302 shown with its `Location` header | request the `http://` form of an HTTPS site |
| 5 | Raw response in the terminal | `curl -i https://example.com` |

**SimpleBlog state after this session:** does not exist yet — created in Session 02.

## Common misconceptions

| Misconception | Correction |
|---|---|
| "One page = one request." | One page is dozens of requests: HTML, then CSS, JS, images, fonts. |
| "The server sends the whole website." | The server answers *one request at a time*, with one resource each. |
| "The URL is the file path on the server." | It *used* to be, in 1995. Today the path is interpreted by the app — Session 04. |
| "HTTPS is a different protocol." | Same HTTP, wrapped in encryption. |
| "404 means the site is broken." | 404 means *that resource* does not exist. The server answered correctly. |
| "The server remembers me between requests." | It does not, by default. Every request is independent. |
| "GET and POST are interchangeable." | GET must never change data. Refresh, back button and crawlers repeat GETs freely. |
| "The `#fragment` is sent to the server." | It never leaves the browser. |

## Check for understanding

1. In `https://shop.example.com/products/17?color=red#reviews` — which part does the
   server never see? Which part identifies the resource?
2. A page says "You must be logged in to view this." Which status code should it return —
   401 or 403? What if it says "Admins only" and you *are* logged in?
3. You click a link and the page appears; you click it again and it appears again,
   unchanged. Which HTTP method was almost certainly used, and why does that matter?
4. Your browser shows an error page. The status code is 500. Whose bug is it?
5. Loading one news article produced 87 rows in the Network tab. Explain that number to
   someone who has never opened DevTools.

## Lab task (in class) · 30 min

**Investigate a website.** In pairs, pick any public website and produce a short written
report (a shared document or paper — no code):

1. How many requests does the home page make? What is the largest single resource?
2. Find and record one request of each type you can: a document, a stylesheet, an image.
3. Copy out the full request line and three request headers of the main document.
4. Copy out the status line and the `Content-Type` header of the response.
5. Trigger a **404** on that site and record the URL you used.
6. Find any request whose status is **not** 200 and explain in one sentence what happened.

**Acceptance criteria:** the report names the method, path, status code and content type
for at least three distinct requests, and explains one non-200 response correctly.

## Homework

1. **Install the toolchain before Session 02** — this is mandatory, we start coding:
   - .NET 10 SDK — https://dotnet.microsoft.com/download
   - VS Code + the **C# Dev Kit** extension (or Visual Studio Community on Windows)
   - Verify by running `dotnet --version` in a terminal and bringing the output to class.
2. Pick a website you use often. Write down **five** URLs from it and, for each, label
   scheme / host / path / query / fragment.
3. For each of these actions, write which HTTP method you would use and why: viewing an
   article, publishing a comment, editing your profile, removing a photo.

## Glossary (EN → BG)

| English | Български |
|---|---|
| client | клиент |
| server | сървър |
| request | заявка |
| response | отговор |
| protocol | протокол |
| stateless | без запазване на състояние |
| header | хедър / заглавна част |
| body (of a message) | тяло (на съобщение) |
| method (HTTP) | метод |
| status code | статус код |
| redirect | пренасочване |
| resource | ресурс |
| query string | заявков низ / query параметри |
| fragment | фрагмент |
| DNS | система за имена на домейни |
| safe / idempotent | безопасен / идемпотентен |

## References

- MDN — An overview of HTTP: https://developer.mozilla.org/en-US/docs/Web/HTTP/Overview
- MDN — HTTP request methods: https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods
- MDN — HTTP response status codes: https://developer.mozilla.org/en-US/docs/Web/HTTP/Status
- MDN — What is a URL: https://developer.mozilla.org/en-US/docs/Learn/Common_questions/Web_mechanics/What_is_a_URL
- Chrome DevTools — Network features reference: https://developer.chrome.com/docs/devtools/network
