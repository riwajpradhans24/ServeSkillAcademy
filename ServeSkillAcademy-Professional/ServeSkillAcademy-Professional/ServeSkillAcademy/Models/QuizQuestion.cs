using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServeSkillAcademy.Models
{
    public class QuizQuestion
    {
        [Key]
        public int QuestionId { get; set; }

        [Required, StringLength(500)]
        public string QuestionText { get; set; } = string.Empty;

        [Required]
        public int QuizId { get; set; }

        [ForeignKey(nameof(QuizId))]
        public Quiz? Quiz { get; set; }

        public ICollection<QuizOption> Options { get; set; } = new List<QuizOption>();
    }
}
