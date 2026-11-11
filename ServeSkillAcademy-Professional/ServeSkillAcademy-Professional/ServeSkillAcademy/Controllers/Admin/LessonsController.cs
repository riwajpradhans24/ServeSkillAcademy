using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServeSkillAcademy.Data;
using ServeSkillAcademy.Models;
using ServeSkillAcademy.ViewModels;

namespace ServeSkillAcademy.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]")]
    public class LessonsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public LessonsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // Lessons are managed in the context of their course
        [HttpGet("ForCourse/{courseId}")]
        public async Task<IActionResult> ForCourse(int courseId)
        {
            var course = await _context.Courses.Include(c => c.Lessons.OrderBy(l => l.OrderIndex))
                .FirstOrDefaultAsync(c => c.CourseId == courseId);
            if (course == null) return NotFound();
            return View(course);
        }

        [HttpGet("Create/{courseId}")]
        public async Task<IActionResult> Create(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null) return NotFound();

            var nextOrder = await _context.Lessons.Where(l => l.CourseId == courseId).CountAsync() + 1;
            var vm = new LessonFormViewModel { CourseId = courseId, CourseTitle = course.Title, OrderIndex = nextOrder, DayNumber = Math.Max(1, (int)Math.Ceiling(nextOrder / 3.0)), Chapter = $"Chapter {((nextOrder - 1) % 3) + 1}" };
            return View(vm);
        }

        [HttpPost("Create/{courseId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int courseId, LessonFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.CourseId = courseId;
                return View(vm);
            }

            var lesson = new Lesson
            {
                CourseId = courseId,
                Title = vm.Title,
                Content = vm.Content,
                VideoUrl = vm.VideoUrl,
                AudioUrl = vm.AudioUrl,
                OrderIndex = vm.OrderIndex,
                DayNumber = vm.DayNumber,
                Chapter = vm.Chapter,
                EstimatedMinutes = vm.EstimatedMinutes
            };

            if (vm.ImageFile != null)
                lesson.ImageUrl = await SaveFile(vm.ImageFile);

            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Lesson added.";
            return RedirectToAction(nameof(ForCourse), new { courseId });
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var lesson = await _context.Lessons.Include(l => l.Course).FirstOrDefaultAsync(l => l.LessonId == id);
            if (lesson == null) return NotFound();

            var vm = new LessonFormViewModel
            {
                LessonId = lesson.LessonId,
                CourseId = lesson.CourseId,
                CourseTitle = lesson.Course?.Title,
                Title = lesson.Title,
                Content = lesson.Content,
                VideoUrl = lesson.VideoUrl,
                AudioUrl = lesson.AudioUrl,
                ExistingImageUrl = lesson.ImageUrl,
                OrderIndex = lesson.OrderIndex,
                DayNumber = lesson.DayNumber,
                Chapter = lesson.Chapter,
                EstimatedMinutes = lesson.EstimatedMinutes
            };
            return View(vm);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LessonFormViewModel vm)
        {
            if (id != vm.LessonId) return NotFound();
            if (!ModelState.IsValid) return View(vm);

            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null) return NotFound();

            lesson.Title = vm.Title;
            lesson.Content = vm.Content;
            lesson.VideoUrl = vm.VideoUrl;
            lesson.AudioUrl = vm.AudioUrl;
            lesson.OrderIndex = vm.OrderIndex;
            lesson.DayNumber = vm.DayNumber;
            lesson.Chapter = vm.Chapter;
            lesson.EstimatedMinutes = vm.EstimatedMinutes;

            if (vm.ImageFile != null)
                lesson.ImageUrl = await SaveFile(vm.ImageFile);

            await _context.SaveChangesAsync();
            TempData["Success"] = "Lesson updated.";
            return RedirectToAction(nameof(ForCourse), new { courseId = lesson.CourseId });
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null) return NotFound();

            var courseId = lesson.CourseId;
            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Lesson deleted.";
            return RedirectToAction(nameof(ForCourse), new { courseId });
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "media");
            Directory.CreateDirectory(uploadsDir);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(uploadsDir, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/media/{fileName}";
        }
    }
}
