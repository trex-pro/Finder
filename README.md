# Finder

Backend API for a member discovery and messaging application: user accounts with ASP.NET Core Identity, rich member profiles and photos (via Cloudinary), direct messaging, and admin role management. The solution is designed to pair with a SPA (CORS is configured for `https://localhost:4200`).

## Features

- **Authentication**: Register and login with ASP.NET Core Identity; JWT access tokens (short-lived) plus HTTP-only refresh cookies.
- **Members**: Paginated member listing with filters (gender, age range, sort), profile detail, profile updates, and photo gallery.
- **Photos**: Upload, delete, and set main profile image using Cloudinary (`finder-assets` folder, 500×500 face-centered crop).
- **Messaging**: Send messages, list inbox/outbox with pagination, read conversation threads, soft-delete for both parties.
- **Admin**: List users with roles, edit user roles; moderator/admin policy for photo moderation placeholder endpoint.
- **Data**: SQLite database, EF Core migrations applied automatically on startup, optional JSON seed for demo users.

## Tech stack

| Area | Choice |
|------|--------|
| Runtime | .NET 9 |
| Web | ASP.NET Core minimal hosting (`Program.cs`) |
| ORM | Entity Framework Core 9 (SQLite) |
| Identity | ASP.NET Core Identity (roles: Member, Moderator, Admin) |
| Auth | JWT Bearer + refresh token stored on user |
| Media | CloudinaryDotNet |
| API style | REST, `[Route("api/[controller]")]` |

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- A [Cloudinary](https://cloudinary.com/) account (Cloud name, API key, API secret) for photo uploads
- Optional: [Angular CLI](https://angular.dev/tools/cli) or another client on `https://localhost:4200` to exercise CORS and cookies

## Repository layout

```
Finder.sln          # Solution entry point
API/
  API.csproj
  Program.cs        # App composition, pipeline, migration + seed on startup
  Controllers/      # Account, Members, Messages, Admin, Buggy (test errors)
  Data/             # DbContext, repositories, migrations, Seed.cs, UserSeedData.json
  Entities/         # AppUser, Member, Photo, Message
  DTOs/, Services/, Middleware/, ...
```

## Configuration

The project expects standard ASP.NET Core configuration (for example `appsettings.json`, `appsettings.Development.json`, environment variables, or [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)). The repository does not ship committed `appsettings` files; create them locally or use user secrets.

### Required settings

1. **Connection string** — SQLite, key `ConnectionStrings:DefaultConnection`.

   Example:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Data Source=finder.db"
   }
   ```

2. **JWT signing key** — key `TokenKey`.

   Must be a **string longer than 64 characters** (enforced in `TokenService`). Use a long random secret in development and a managed secret in production.

3. **Cloudinary** — section `CloudinarySettings`:

   ```json
   "CloudinarySettings": {
     "CloudName": "your-cloud-name",
     "ApiKey": "your-api-key",
     "ApiSecret": "your-api-secret"
   }
   ```

### Example `appsettings.Development.json`

Adjust paths and secrets for your machine; do not commit real secrets.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=finder.db"
  },
  "TokenKey": "REPLACE_WITH_A_RANDOM_STRING_AT_LEAST_SIXTY_FIVE_CHARACTERS_LONG_FOR_LOCAL_DEV_ONLY",
  "CloudinarySettings": {
    "CloudName": "",
    "ApiKey": "",
    "ApiSecret": ""
  }
}
```

### User Secrets (recommended for local dev)

From the `API` directory:

```bash
cd API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=finder.db"
dotnet user-secrets set "TokenKey" "<your-64+-char-secret>"
dotnet user-secrets set "CloudinarySettings:CloudName" "<cloud>"
dotnet user-secrets set "CloudinarySettings:ApiKey" "<key>"
dotnet user-secrets set "CloudinarySettings:ApiSecret" "<secret>"
```

## Seed data

On first run, if the user store is empty, `Seed.SeedUsers` reads `API/Data/UserSeedData.json` and creates members (password `Pa$$w0rd`, role **Member**). It also creates an **Admin** user:

- Username: `Admin`
- Email: `admin@test.com`
- Password: `Pa$$w0rd`
- Roles: **Admin** and **Moderator**

Each seed entry must deserialize to `SeedUserDto`: `Id`, `Email`, `UserName`, `DOB`, `Gender`, `City`, `Country`, `Created`, `LastActive`, optional `ImageUrl` and `Description`. If the file is missing or empty, seeding skips members but may still create the admin user when applicable.

## Run the API

```bash
cd /path/to/Finder
dotnet restore
dotnet run --project API
```

Default HTTPS URL from launch profile: **`https://localhost:5001`**.

The app:

1. Applies pending EF Core migrations (`Database.MigrateAsync()`).
2. Runs user seeding as described above.
3. Enables CORS for `https://localhost:4200` with credentials (cookies for refresh tokens).

## Authentication for clients

- **Access token**: Returned in the JSON body as `UserDto.Token` on register, login, and refresh. Send as `Authorization: Bearer <token>`. Token lifetime is about **7 minutes** (see `TokenService`).
- **Refresh token**: Stored in an HTTP-only cookie named `refreshToken` (Secure, `SameSite=Strict`). Call `POST /api/account/refresh-token` with credentials included to rotate the cookie and get a new JWT.

Register and login set the refresh cookie automatically.

## API overview

Base path: `/api/{controller}`.

| Area | Method | Route | Auth | Notes |
|------|--------|-------|------|--------|
| Account | POST | `/api/account/register` | No | Body: register DTO; sets refresh cookie |
| Account | POST | `/api/account/login` | No | Email + password; sets refresh cookie |
| Account | POST | `/api/account/refresh-token` | Cookie | Returns new JWT; rotates refresh cookie |
| Members | GET | `/api/members` | Yes | Query: paging, `gender`, `minAge`, `maxAge`, `orderBy` |
| Members | GET | `/api/members/{id}` | Yes | Member detail |
| Members | GET | `/api/members/{id}/photos` | Yes | Photo list |
| Members | PUT | `/api/members` | Yes | Update current user’s member profile |
| Members | POST | `/api/members/add-photo` | Yes | `multipart/form-data` file |
| Members | DELETE | `/api/members/delete-photo/{photoId}` | Yes | |
| Members | PUT | `/api/members/set-main/{photoId}` | Yes | |
| Messages | POST | `/api/messages` | Yes* | Create message |
| Messages | GET | `/api/messages` | Yes* | Query: `container` (e.g. Inbox), paging |
| Messages | GET | `/api/messages/thread/{recipientId}` | Yes* | |
| Messages | DELETE | `/api/messages/{id}` | Yes* | Soft delete / remove when both sides deleted |
| Admin | GET | `/api/admin/user-roles` | Admin | |
| Admin | POST | `/api/admin/edit-roles/{userId}` | Admin | Query: `roles` comma-separated |
| Admin | GET | `/api/admin/photo-moderation` | Admin or Moderator | Placeholder response |
| Buggy | GET | `/api/buggy/*` | Mixed | Test client error handling / `admin-secret` needs Admin role |

\*Message endpoints use the current user id from the JWT; call them with a valid Bearer token.

`MembersController` actions are protected with `[Authorize]`. Successful requests update **LastActive** via the `LogUserActivity` filter.

## Database and migrations

- Provider: **SQLite** (local file path from connection string).
- Migrations live under `API/Data/Migrations/`.

Add a new migration after model changes:

```bash
cd API
dotnet ef migrations add YourMigrationName
dotnet ef database update
```

(Requires the `dotnet-ef` tool: `dotnet tool install --global dotnet-ef`.)

## Error handling

`ExceptionMiddleware` wraps unhandled exceptions for consistent API error responses. `BuggyController` exposes intentional 400/401/404/500 responses for client testing.

## License and contributing

Add your preferred license and contribution guidelines if this repository is public or shared.
