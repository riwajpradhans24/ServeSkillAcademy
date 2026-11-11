using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServeSkillAcademy.Data;
using ServeSkillAcademy.Models;

namespace ServeSkillAcademy.Controllers
{
    // Public, unauthenticated pages: landing page, course catalog/search, and about.
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var featuredCourses = await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Lessons)
                .Include(c => c.Quizzes)
                .Include(c => c.Videos)
                .Where(c => c.IsPublished)
                .OrderByDescending(c => c.CreatedAt)
                .Take(6)
                .ToListAsync();

            return View(featuredCourses);
        }

        // Course catalog with keyword search and category filtering
        public async Task<IActionResult> Courses(string? search, int? categoryId)
        {
            var query = _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Lessons)
                .Include(c => c.Quizzes)
                .Include(c => c.Videos)
                .Where(c => c.IsPublished)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c => c.Title.Contains(search) || c.Description.Contains(search));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(c => c.CategoryId == categoryId.Value);
            }

            ViewBag.Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            ViewBag.Search = search;
            ViewBag.CategoryId = categoryId;

            var courses = await query.OrderBy(c => c.Title).ToListAsync();
            return View(courses);
        }

        public async Task<IActionResult> CourseDetails(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Lessons)
                .Include(c => c.Quizzes)
                .Include(c => c.Videos)
                .FirstOrDefaultAsync(c => c.CourseId == id && c.IsPublished);

            if (course == null) return NotFound();

            if (User.Identity?.IsAuthenticated == true && User.IsInRole("Learner"))
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                ViewBag.IsEnrolled = userId != null && await _context.Enrollments.AnyAsync(e => e.UserId == userId && e.CourseId == id);
            }
            else
            {
                ViewBag.IsEnrolled = false;
            }

            return View(course);
        }

        public IActionResult About() => View();

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View();
    }
}
