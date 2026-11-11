using System.ComponentModel.DataAnnotations;

namespace ServeSkillAcademy.ViewModels
{
    public class ProfileViewModel
    {
        [Required, StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        [StringLength(150)]
        public string? Workplace { get; set; }

        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
