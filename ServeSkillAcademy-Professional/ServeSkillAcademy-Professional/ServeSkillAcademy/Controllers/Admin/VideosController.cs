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
    public class VideosController : Controller
    {
        private readonly ApplicationDbContext _context;
        public VideosController(ApplicationDbContext context) => _context = context;

        [HttpGet("ForCourse/{courseId}")]
        public async Task<IActionResult> ForCourse(int courseId)
        {
            var course = await _context.Courses.Include(c => c.Videos.OrderBy(v => v.OrderIndex)).FirstOrDefaultAsync(c => c.CourseId == courseId);
            return course == null ? NotFound() : View(course);
        }

        [HttpGet("Create/{courseId}")]
        public async Task<IActionResult> Create(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null) return NotFound();
            var next = await _context.Videos.CountAsync(v => v.CourseId == courseId) + 1;
            return View(new VideoFormViewModel { CourseId = courseId, CourseTitle = course.Title, OrderIndex = next });
        }

        [HttpPost("Create/{courseId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int courseId, VideoFormViewModel vm)
        {
            if (!ModelState.IsValid) { vm.CourseId = courseId; return View(vm); }
            _context.Videos.Add(new Video { CourseId = courseId, Title = vm.Title, Description = vm.Description, VideoUrl = vm.VideoUrl, ThumbnailUrl = vm.ThumbnailUrl, DurationMinutes = vm.DurationMinutes, OrderIndex = vm.OrderIndex, IsPublished = vm.IsPublished });
            await _context.SaveChangesAsync();
            TempData["Success"] = "Learning video added.";
            return RedirectToAction(nameof(ForCourse), new { courseId });
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var v = await _context.Videos.Include(x => x.Course).FirstOrDefaultAsync(x => x.VideoId == id);
            if (v == null) return NotFound();
            return View(new VideoFormViewModel { VideoId=v.VideoId, CourseId=v.CourseId, CourseTitle=v.Course?.Title, Title=v.Title, Description=v.Description, VideoUrl=v.VideoUrl, ThumbnailUrl=v.ThumbnailUrl, DurationMinutes=v.DurationMinutes, OrderIndex=v.OrderIndex, IsPublished=v.IsPublished });
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VideoFormViewModel vm)
        {
            if (id != vm.VideoId) return NotFound();
            if (!ModelState.IsValid) return View(vm);
            var v = await _context.Videos.FindAsync(id);
            if (v == null) return NotFound();
            v.Title=vm.Title; v.Description=vm.Description; v.VideoUrl=vm.VideoUrl; v.ThumbnailUrl=vm.ThumbnailUrl; v.DurationMinutes=vm.DurationMinutes; v.OrderIndex=vm.OrderIndex; v.IsPublished=vm.IsPublished;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Learning video updated.";
            return RedirectToAction(nameof(ForCourse), new { courseId = v.CourseId });
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var v = await _context.Videos.FindAsync(id);
            if (v == null) return NotFound();
            var courseId = v.CourseId;
            _context.Videos.Remove(v);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Learning video deleted.";
            return RedirectToAction(nameof(ForCourse), new { courseId });
        }
    }
}
