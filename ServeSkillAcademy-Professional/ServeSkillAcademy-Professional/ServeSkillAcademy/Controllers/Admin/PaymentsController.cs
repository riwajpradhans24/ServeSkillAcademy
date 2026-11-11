using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServeSkillAcademy.Data;
using ServeSkillAcademy.Models;

namespace ServeSkillAcademy.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]")]
    public class PaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PaymentsController(ApplicationDbContext context) => _context = context;

        [HttpGet("")]
        public async Task<IActionResult> Index() => View(await _context.PaymentTransactions.Include(p => p.Course).Include(p => p.User).OrderByDescending(p => p.CreatedAt).ToListAsync());

        [HttpPost("Approve/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var payment = await _context.PaymentTransactions.Include(p => p.Course).FirstOrDefaultAsync(p => p.PaymentTransactionId == id);
            if (payment == null) return NotFound();
            payment.Status = "Approved";
            payment.PaidAt = DateTime.UtcNow;
            if (!await _context.Enrollments.AnyAsync(e => e.UserId == payment.UserId && e.CourseId == payment.CourseId))
                _context.Enrollments.Add(new Enrollment { UserId = payment.UserId, CourseId = payment.CourseId, EnrolledAt = DateTime.UtcNow });
            await _context.SaveChangesAsync();
            TempData["Success"] = "Payment approved and course access granted.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("Reject/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var payment = await _context.PaymentTransactions.FindAsync(id);
            if (payment == null) return NotFound();
            payment.Status = "Rejected";
            await _context.SaveChangesAsync();
            TempData["Success"] = "Payment rejected.";
            return RedirectToAction(nameof(Index));
        }
    }
}
