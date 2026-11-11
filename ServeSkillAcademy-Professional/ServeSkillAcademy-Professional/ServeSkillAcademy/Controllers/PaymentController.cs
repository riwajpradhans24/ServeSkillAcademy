using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServeSkillAcademy.Data;
using ServeSkillAcademy.Models;

namespace ServeSkillAcademy.Controllers
{
    [Authorize(Roles = "Learner")]
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Checkout(int courseId)
        {
            var course = await _context.Courses.Include(c => c.Category).FirstOrDefaultAsync(c => c.CourseId == courseId && c.IsPublished);
            if (course == null) return NotFound();
            if (course.Price <= 0) return RedirectToAction("Enroll", "Learner", new { courseId });

            var userId = _userManager.GetUserId(User)!;
            if (await _context.Enrollments.AnyAsync(e => e.UserId == userId && e.CourseId == courseId))
                return RedirectToAction("Lesson", "Learner", new { courseId });

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(int courseId, string paymentMethod, string? transactionReference)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId && c.IsPublished);
            if (course == null) return NotFound();
            if (course.Price <= 0) return RedirectToAction("Enroll", "Learner", new { courseId });

            var userId = _userManager.GetUserId(User)!;
            if (await _context.Enrollments.AnyAsync(e => e.UserId == userId && e.CourseId == courseId))
                return RedirectToAction("Lesson", "Learner", new { courseId });

            var allowed = new[] { "eSewa", "Bank Transfer", "Credit/Debit Card" };
            if (!allowed.Contains(paymentMethod))
            {
                TempData["Error"] = "Please select a valid payment method.";
                return RedirectToAction(nameof(Checkout), new { courseId });
            }

            var payment = new PaymentTransaction
            {
                UserId = userId,
                CourseId = courseId,
                Amount = course.Price,
                PaymentMethod = paymentMethod,
                TransactionReference = string.IsNullOrWhiteSpace(transactionReference) ? $"SS-{Guid.NewGuid():N}"[..14].ToUpperInvariant() : transactionReference.Trim(),
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };
            _context.PaymentTransactions.Add(payment);
            await _context.SaveChangesAsync();

            TempData["Success"] = paymentMethod == "Credit/Debit Card"
                ? "Card payment request created. Connect your production card gateway before accepting real card payments."
                : "Payment request submitted. Your enrollment will unlock after payment verification.";

            return RedirectToAction(nameof(Status), new { id = payment.PaymentTransactionId });
        }

        public async Task<IActionResult> Status(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var payment = await _context.PaymentTransactions
                .Include(p => p.Course)
                .FirstOrDefaultAsync(p => p.PaymentTransactionId == id && p.UserId == userId);
            if (payment == null) return NotFound();
            return View(payment);
        }
    }
}
