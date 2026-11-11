using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServeSkillAcademy.Data;
using ServeSkillAcademy.Models;
using ServeSkillAcademy.ViewModels;

namespace ServeSkillAcademy.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]")]
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CoursesController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 10;
            var query = _context.Courses.Include(c => c.Category).OrderByDescending(c => c.CreatedAt);
            var total = await query.CountAsync();
            var courses = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            return View(courses);
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            var vm = new CourseFormViewModel { Categories = await GetCategoryList() };
            return View(vm);
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Categories = await GetCategoryList();
                return View(vm);
            }

            var course = new Course
            {
                Title = vm.Title,
                Description = vm.Description,
                CategoryId = vm.CategoryId,
                IsPublished = vm.IsPublished,
                Level = vm.Level,
                Price = vm.Price,
                DurationDays = vm.DurationDays,
                CreatedAt = DateTime.UtcNow
            };

            if (vm.ThumbnailFile != null)
                course.ThumbnailUrl = await SaveThumbnail(vm.ThumbnailFile);

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Course created.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            var vm = new CourseFormViewModel
            {
                CourseId = course.CourseId,
                Title = course.Title,
                Description = course.Description,
                CategoryId = course.CategoryId,
                IsPublished = course.IsPublished,
                Level = course.Level,
                Price = course.Price,
                DurationDays = course.DurationDays,
                ExistingThumbnailUrl = course.ThumbnailUrl,
                Categories = await GetCategoryList()
            };
            return View(vm);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CourseFormViewModel vm)
        {
            if (id != vm.CourseId) return NotFound();

            if (!ModelState.IsValid)
            {
                vm.Categories = await GetCategoryList();
                return View(vm);
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            course.Title = vm.Title;
            course.Description = vm.Description;
            course.CategoryId = vm.CategoryId;
            course.IsPublished = vm.IsPublished;
            course.Level = vm.Level;
            course.Price = vm.Price;
            course.DurationDays = vm.DurationDays;

            if (vm.ThumbnailFile != null)
                course.ThumbnailUrl = await SaveThumbnail(vm.ThumbnailFile);

            await _context.SaveChangesAsync();
            TempData["Success"] = "Course updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("TogglePublish/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePublish(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            course.IsPublished = !course.IsPublished;
            await _context.SaveChangesAsync();
            TempData["Success"] = course.IsPublished ? "Course published." : "Course unpublished.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Course deleted.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<List<SelectListItem>> GetCategoryList()
        {
            return await _context.Categories
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.CategoryId.ToString(), Text = c.Name })
                .ToListAsync();
        }

        private async Task<string> SaveThumbnail(IFormFile file)
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
