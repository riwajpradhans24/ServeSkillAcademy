using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServeSkillAcademy.Models
{
    public class LessonCompletion
    {
        [Key]
        public int LessonCompletionId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        [Required]
        public int LessonId { get; set; }

        [ForeignKey(nameof(LessonId))]
        public Lesson? Lesson { get; set; }

        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    }
}
