using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ServeSkillAcademy.ViewModels
{
    public class LessonFormViewModel
    {
        public int LessonId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Video URL")]
        public string? VideoUrl { get; set; }

        [Display(Name = "Audio URL")]
        public string? AudioUrl { get; set; }

        [Display(Name = "Image")]
        public IFormFile? ImageFile { get; set; }

        public string? ExistingImageUrl { get; set; }

        [Display(Name = "Order")]
        public int OrderIndex { get; set; }

        [Range(1, 365)]
        [Display(Name = "Day Number")]
        public int DayNumber { get; set; } = 1;

        [Required]
        [StringLength(120)]
        public string Chapter { get; set; } = "Chapter 1";

        [Range(5, 180)]
        [Display(Name = "Estimated Minutes")]
        public int EstimatedMinutes { get; set; } = 20;

        public string? CourseTitle { get; set; }
    }
}