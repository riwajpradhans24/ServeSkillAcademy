# ServeSkill Academy

A database-driven ASP.NET Core MVC web-based learning system for the restaurant & hospitality industry, built for the CT050-3-2 WAPP assignment.

## Stack
- ASP.NET Core MVC (.NET 8)
- Entity Framework Core (Code-First) + SQL Server
- ASP.NET Core Identity (roles: Admin, Learner)
- Bootstrap 5, jQuery Unobtrusive Validation

## Setup

1. **Install prerequisites**
   - [.NET 8 SDK](https://dotnet.microsoft.com/download)
   - SQL Server (LocalDB is fine — ships with Visual Studio) or any SQL Server instance

2. **Restore packages**
   ```
   dotnet restore
   ```

3. **Set your connection string**
   Edit `appsettings.json` → `ConnectionStrings:DefaultConnection` if you're not using LocalDB, e.g.:
   ```
   Server=YOUR_SERVER;Database=ServeSkillAcademyDb;Trusted_Connection=True;TrustServerCertificate=True
   ```

4. **Create the initial migration and database**
   The project currently has no migrations checked in (they must be generated for your machine/SQL Server version):
   ```
   dotnet tool install --global dotnet-ef   # if you don't already have it
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
   Alternatively, `Program.cs` calls `Database.EnsureCreatedAsync()` in `DbInitializer`, so the app will create the schema automatically on first run even without running migrations — use `dotnet ef migrations add` first if you want migration history for the report/marking scheme.

5. **Run the app**
   ```
   dotnet run
   ```
   Then open the URL shown in the console (typically `https://localhost:5001` or similar).

## Seeded accounts
On first run, `DbInitializer` seeds:
- **Admin:** admin@serveskill.com / Admin@123
- **Sample Learner:** learner@serveskill.com / Learner@123
- 4 categories, 3 published courses, 5 lessons, and 1 sample quiz with 2 questions.

## Project Structure
- `/Controllers` — Home (guest), Account (auth/profile), Learner, and `Controllers/Admin/*` (Dashboard, Users, Categories, Courses, Lessons, Quizzes)
- `/Models` — EF Core domain entities
- `/ViewModels` — Form and display view models
- `/Data` — `ApplicationDbContext` and `DbInitializer` (seed logic)
- `/Views` — Razor views, organized by controller (admin views share `_AdminLayout.cshtml` via a folder-level `_ViewStart.cshtml`)
- `/wwwroot` — CSS, JS, sample images, and uploaded media (`wwwroot/uploads/media`)

## Notes on admin routing
Admin management controllers live in `Controllers/Admin/` and are routed with an explicit `[Route("Admin/[controller]")]` attribute (e.g. `/Admin/Courses`), rather than using ASP.NET's formal Areas feature, to keep the view-folder structure simple (`Views/Courses`, `Views/Users`, etc. instead of `Areas/Admin/Views/...`).

## Known simplifications (documented for the report)
- Lesson "progress" is a simplified even-split percentage per lesson marked complete, not per-second video tracking.
- Quiz editing replaces all questions/options for that quiz on save, rather than diffing individual items — simplest reliable approach at this scale.
- Course thumbnails and lesson images are uploaded to `wwwroot/uploads/media`; lesson video/audio are entered as URLs (e.g. an uploaded file link or public sample) rather than uploaded binaries, to keep the lesson form simple.
