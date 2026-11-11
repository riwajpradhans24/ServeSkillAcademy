# ServeSkillAcademy 🎓

A full-stack hospitality learning management web platform built to manage course catalogs, student enrollments, and progress tracking. 

This project demonstrates scalable backend architecture, relational database management, and dynamic data filtering to serve both administrators and students.

## 🚀 Tech Stack
* **Backend:** C#, ASP.NET Core MVC
* **Database:** MS SQL Server, Entity Framework Core (Code-First)
* **Frontend:** HTML5, CSS3, Bootstrap, Razor Pages
* **Data Queries:** LINQ for dynamic filtering and relational data mapping

## ✨ Key Features
* **Admin Portal Database Updates:** Full CRUD operations for managing courses, lessons, and users.
* **Student Progress Tracking:** Relational mapping across `Enrollments`, `Lessons`, and `LessonCompletions` tables.
* **Dynamic Content Filtering:** Optimized data retrieval using LINQ to dynamically display course catalogs based on user roles.

## 🛠️ How to Run Locally

1. **Clone the repository:**
   `git clone https://github.com/riwajpradhans24/ServeSkillAcademy.git`
2. **Setup Database:**
   * Ensure MS SQL Server or LocalDB is running.
   * Verify the `DefaultConnection` string in `appsettings.json` matches your local server.
3. **Apply EF Core Migrations & Run:**
   * Open Package Manager Console in Visual Studio and run `Update-Database`.
   * Press `F5` or run `dotnet run` in the terminal.

---
*Developed by [Riwaj Pradhan](https://github.com/riwajpradhans24) to demonstrate robust C# and ASP.NET Core backend development.*
