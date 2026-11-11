using Microsoft.AspNetCore.Identity;

namespace ServeSkillAcademy.Models
{
    // Extends the built-in Identity user with the extra profile fields the
    // assignment brief asks for. "Role" is a friendly display copy of the
    // Identity role the user was assigned (kept in sync by DbInitializer /
    // AccountController) — actual authorization uses AspNetUserRoles.
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string? Workplace { get; set; }
        public string Role { get; set; } = "Learner";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
        public ICollection<Badge> Badges { get; set; } = new List<Badge>();
        public ICollection<PaymentTransaction> Payments { get; set; } = new List<PaymentTransaction>();
    }
}
