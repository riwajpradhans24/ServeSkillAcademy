using System.ComponentModel.DataAnnotations;

namespace ServeSkillAcademy.ViewModels
{
    public class VideoFormViewModel
    {
        public int VideoId { get; set; }
        [Required, StringLength(160)] public string Title { get; set; } = string.Empty;
        [StringLength(800)] public string? Description { get; set; }
        [Required, Url, StringLength(500)] public string VideoUrl { get; set; } = string.Empty;
        [Url, StringLength(500)] public string? ThumbnailUrl { get; set; }
        [Range(1, 600)] public int DurationMinutes { get; set; } = 10;
        [Range(1, 999)] public int OrderIndex { get; set; } = 1;
        public bool IsPublished { get; set; } = true;
        public int CourseId { get; set; }
        public string? CourseTitle { get; set; }
    }
}
