using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServeSkillAcademy.Models
{
    public class Video
    {
        [Key]
        public int VideoId { get; set; }

        [Required, StringLength(160)]
        public string Title { get; set; } = string.Empty;

        [StringLength(800)]
        public string? Description { get; set; }

        [Required, StringLength(500)]
        [Display(Name = "Video URL")]
        public string VideoUrl { get; set; } = string.Empty;

        [StringLength(500)]
        public string? ThumbnailUrl { get; set; }

        [Range(1, 600)]
        public int DurationMinutes { get; set; } = 10;

        public int OrderIndex { get; set; } = 1;
        public bool IsPublished { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int CourseId { get; set; }

        [ForeignKey(nameof(CourseId))]
        public Course? Course { get; set; }
    }
}
