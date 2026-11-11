using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServeSkillAcademy.Models
{
    public class Lesson
    {
        [Key]
        public int LessonId { get; set; }

        [Required, StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public string? VideoUrl { get; set; }
        public string? AudioUrl { get; set; }
        public string? ImageUrl { get; set; }

        [Required, StringLength(120)]
        public string Chapter { get; set; } = "Chapter 1";

        [Range(5, 180)]
        public int EstimatedMinutes { get; set; } = 20;

        public int OrderIndex { get; set; }

        [Range(1, 365)]
        public int DayNumber { get; set; } = 1;

        [Required]
        public int CourseId { get; set; }

        [ForeignKey(nameof(CourseId))]
        public Course? Course { get; set; }
    }
}
