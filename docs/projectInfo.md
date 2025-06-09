# 🎮 MyGameList

**MyGameList** is a personal video game tracking web app inspired by MyAnimeList. Built with ASP.NET Core MVC, it allows users to log in with Google, search for games using the RAWG API, and organize their gaming journey into custom lists such as **Backlog**, **Currently Playing**, and **Completed**.

---

## 🚀 Features

- 🔐 Google OAuth login (no local accounts)
- 🔎 Game search via the [RAWG API](https://rawg.io/apidocs)
- 🎮 Create and manage personalized game lists
- ⭐ Optional: Add ratings, reviews, and play progress
- 📱 Fully responsive layout with custom CSS
- ☁️ Hosted on AWS (EC2 + RDS)

---

## 🛠️ Tech Stack

| Layer       | Technology |
|-------------|------------|
| Language    | C#         |
| Framework   | ASP.NET Core MVC (Razor Views) |
| Styling     | Custom CSS (mobile-first, responsive) |
| Auth        | ASP.NET Identity + Google OAuth |
| API         | RAWG Video Games Database API |
| Database    | SQL Server (via AWS RDS) |
| ORM         | Entity Framework Core |
| Hosting     | AWS EC2 (app) + AWS RDS (DB) |
| Config      | `appsettings.json` + `dotnet user-secrets` for environment variables |

---

## 🧱 Database Structure (Simplified)

```text
Users (from ASP.NET Identity)
├── Id
├── DisplayName
└── Email

Games (API-linked, minimal info stored)
├── Id (from RAWG)
├── Title
├── CoverImageUrl
└── ReleaseDate

UserGameLists
├── Id
├── UserId (FK)
├── GameId (FK)
├── Status (Backlog / Playing / Completed)
└── CreatedAt
