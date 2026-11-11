using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServeSkillAcademy.Data;

namespace ServeSkillAcademy.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DashboardController(ApplicationDbContext context) => _context = context;

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.TotalCourses = await _context.Courses.CountAsync();
            ViewBag.PublishedCourses = await _context.Courses.CountAsync(c => c.IsPublished);
            ViewBag.TotalEnrollments = await _context.Enrollments.CountAsync();
            ViewBag.TotalQuizAttempts = await _context.QuizAttempts.CountAsync();
            return View();
        }
    }
}
