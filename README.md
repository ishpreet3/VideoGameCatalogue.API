# Video Game Catalogue

A two-page catalogue of video games: a **browse** page listing every game, and an **edit** page for adding or changing one.

- **Backend:** ASP.NET Core 10 Web API, EF Core 10 (Code First) on SQL Server Express, xUnit tests
- **Frontend:** Angular 22, ng-bootstrap 21 / Bootstrap 5, Angular Router, Vitest tests

## Prerequisites

| Tool | Version |
|---|---|
| .NET SDK | 10.0 |
| SQL Server | SQL Server Express (`SQLEXPRESS` instance) **or** LocalDB, which is installed with Visual Studio |
| Node.js | 22.22.3+ or 24.15+ (required by Angular 22) |

Optional: Visual Studio for the backend, and VS Code for the frontend.

## Running the app

The API and the frontend run separately, so use two terminals, or Visual Studio plus a terminal. Start the API first.

### 1. Choose a database

The API uses the `DefaultConnection` connection string in `VideoGameCatalogue.API/appsettings.json`. It points at a SQL Server Express instance, `localhost\SQLEXPRESS`, using Windows authentication. If you have SQL Server Express, skip this step.

**Using LocalDB instead.** Create `VideoGameCatalogue.API/appsettings.Development.json`. It's gitignored, it's picked up both by `dotnet run` and by Visual Studio, and it overrides only the connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=VideoGameCatalogueDB;Trusted_Connection=true;TrustServerCertificate=true"
  }
}
```

To check that LocalDB is installed, run `SqlLocalDB info`; it should list `MSSQLLocalDB`. For any other server, use its name in `Server=`. You can also set the connection string for one terminal session only:

```powershell
$env:ConnectionStrings__DefaultConnection = "Server=(localdb)\MSSQLLocalDB;Database=VideoGameCatalogueDB;Trusted_Connection=true;TrustServerCertificate=true"
```

### 2. Start the API

**From a terminal**, in the repository root (the folder containing `VideoGameCatalogue.API.slnx`):

```powershell
dotnet run --project VideoGameCatalogue.API
```

**From Visual Studio:** open `VideoGameCatalogue.API.slnx`. Set `VideoGameCatalogue.API` as the startup project, choose the **http** launch profile, and press **F5**.

Either way:
- The API runs on **http://localhost:5267**, and Swagger UI is at http://localhost:5267/swagger.
- On startup the EF Core migrations in `VideoGameCatalogue.API/Migrations` are applied automatically. This creates the `VideoGameCatalogueDB` database with two seeded games, so you don't need to run any `dotnet ef` commands.

### 3. Start the frontend

In a second terminal, or in VS Code's integrated terminal with the `VideoGameCatalogue.Frontend` folder open:

```powershell
cd VideoGameCatalogue.Frontend
npm install     # first time only
npm start
```

Open **http://localhost:4200**. The dev server proxies `/api` requests to the API (see `proxy.conf.json`), so the API must be running.

## Tests and checks

```powershell
# Backend: validation, repository and HTTP API tests (from the repository root,
# or Test > Run All Tests in Visual Studio)
dotnet test

# Frontend (from VideoGameCatalogue.Frontend)
npm test                     # unit tests (Vitest) in watch mode; re-runs on file changes
npm test -- --watch=false    # run the unit tests once and exit
npm run lint                 # ESLint with angular-eslint
npm run build                # production build
```

The backend tests don't need SQL Server. Repository and API tests run against an in-memory SQLite database.

## Changing the database schema

You only need this when you change the entity model; running the app doesn't need it. Install the EF Core CLI once, then add a migration from the repository root:

```powershell
dotnet tool install --global dotnet-ef
dotnet ef migrations add <MigrationName> --project VideoGameCatalogue.API
```

The new migration is applied automatically the next time the API starts.

## Project structure

```
VideoGameCatalogue.API/
├── Contracts/        Request/response records and the explicit mappings between them and the entity
├── Controllers/      VideoGamesController (CRUD) and LookupsController (allowed genres/platforms)
├── Data/             VideoGameContext: Fluent API configuration and seed data
├── Domain/           Lookups: the allowed genre and platform values
├── Migrations/       EF Core Code First migrations
├── Models/           VideoGame entity
├── Repositories/     IVideoGameRepository and its EF Core implementation
└── Program.cs        Service registration and HTTP pipeline

VideoGameCatalogue.API.Tests/
├── Contracts/        Request validation rules (table-driven)
├── Repositories/     Repository tests against SQLite with a fake clock
├── Api/              End-to-end HTTP tests via WebApplicationFactory
└── TestData.cs       Builders for valid test objects

VideoGameCatalogue.Frontend/src/app/
├── components/browse/   Browse page: game cards, delete with confirmation
├── components/edit/     Add/edit page: typed reactive form
├── models/              API types
├── services/            VideoGameService (HTTP)
├── shared/              Toast notifications, HTTP error messages
├── testing/             Test data builders
├── app.config.ts        Router, HttpClient
└── app.routes.ts        Lazy-loaded routes
```

## Pages

| Route | Page |
|---|---|
| `/games` | Browse all games, sorted by title. Edit or delete each one, or add a new one. |
| `/games/new` | Add a game. |
| `/games/:id/edit` | Edit an existing game. |

## API

| Method | URL | Result |
|---|---|---|
| `GET` | `/api/videogames` | 200 with all games, sorted by title |
| `GET` | `/api/videogames/{id}` | 200 with the game, or 404 |
| `POST` | `/api/videogames` | 201 with the created game and a `Location` header, or 400 |
| `PUT` | `/api/videogames/{id}` | 200 with the updated game, 400, or 404 |
| `DELETE` | `/api/videogames/{id}` | 204, or 404 |
| `GET` | `/api/lookups` | 200 with the allowed `genres` and `platforms` |

Example request body for `POST` and `PUT`. `VideoGameCatalogue.API/VideoGameCatalogue.API.http` has ready-to-run requests.

```json
{
  "title": "Elden Ring",
  "developer": "FromSoftware",
  "publisher": "Bandai Namco",
  "releaseDate": "2022-02-25",
  "genre": "RPG",
  "price": 59.99,
  "platform": "PC",
  "metacriticScore": 96,
  "description": "An action role-playing game."
}
```

Errors use the standard [Problem Details](https://www.rfc-editor.org/rfc/rfc9457) format. A validation failure returns 400 with an `errors` object keyed by field.

### Validation rules

| Field | Rule |
|---|---|
| Title | Required, max 200 characters |
| Developer, Publisher | Required, max 150 characters |
| Genre, Platform | Required, must be one of the values from `/api/lookups` |
| Release date | Required (`yyyy-MM-dd`); future dates are allowed for upcoming games |
| Price | Required, 0 or more |
| Metacritic score | Required, 0–100 |
| Description | Optional, max 1000 characters |

`Id`, `CreatedAt` and `UpdatedAt` are set by the server and ignored if a client sends them.

## Design decisions

- **Separate API contracts.** Controllers accept `VideoGameRequest` and return `VideoGameResponse`, never the EF entity, so clients can't over-post server-owned fields. Mapping is a few lines of explicit code rather than a mapping library.
- **Validation happens once, at the edge.** DataAnnotations on the request plus `[ApiController]` produce 400 responses automatically. Unhandled exceptions become a 500 Problem Details response through the global exception handler, so controllers have no try/catch.
- **Repository pattern.** `IVideoGameRepository` keeps EF Core out of the controller and gives the data layer its own seam for testing. It takes a `TimeProvider` so timestamps are testable.
- **Server-owned lookups.** Genres and platforms are defined once in `Domain/Lookups.cs`, validated by the API and served to the frontend, so the two can't drift apart.
- **Date-only release dates.** `ReleaseDate` is a `DateOnly` (SQL `date`), serialised as `yyyy-MM-dd`. This binds directly to an HTML date input.
- **Modern Angular.** The app uses:
  - standalone components with signals, zoneless change detection and OnPush by default
  - `inject()`
  - lazy-loaded routes with route parameters bound to component inputs
  - typed reactive forms whose validators mirror the API's rules
- **ng-bootstrap** provides the alerts, toasts, progress bars and the delete confirmation modal. There is no Bootstrap JavaScript and no CDN; styling is kept minimal for the designer.
