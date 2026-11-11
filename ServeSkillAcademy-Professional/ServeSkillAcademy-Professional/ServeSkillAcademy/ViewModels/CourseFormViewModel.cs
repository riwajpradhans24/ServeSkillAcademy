using System.ComponentModel.DataAnnotations;

namespace ServeSkillAcademy.ViewModels
{
    public class CourseFormViewModel
    {
        public int CourseId { get; set; }

        [Required, StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required]
        public string Level { get; set; } = "Basic";

        [Range(0, 1000000)]
        public decimal Price { get; set; }

        [Range(1, 365)]
        public int DurationDays { get; set; } = 7;

        public bool IsPublished { get; set; }

        [Display(Name = "Thumbnail Image")]
        public IFormFile? ThumbnailFile { get; set; }

        public string? ExistingThumbnailUrl { get; set; }

        public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>? Categories { get; set; }
    }
}
