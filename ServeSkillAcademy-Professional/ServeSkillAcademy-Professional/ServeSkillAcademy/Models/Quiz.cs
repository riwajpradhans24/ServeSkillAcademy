using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServeSkillAcademy.Models
{
    public class Quiz
    {
        [Key]
        public int QuizId { get; set; }

        [Required, StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Range(1, 100)]
        public int PassPercentage { get; set; } = 60;

        [Required]
        public int CourseId { get; set; }

        [ForeignKey(nameof(CourseId))]
        public Course? Course { get; set; }

        public ICollection<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();
        public ICollection<QuizAttempt> Attempts { get; set; } = new List<QuizAttempt>();
    }
}
