using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServeSkillAcademy.Models
{
    public class QuizOption
    {
        [Key]
        public int OptionId { get; set; }

        [Required, StringLength(300)]
        public string OptionText { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }

        [Required]
        public int QuestionId { get; set; }

        [ForeignKey(nameof(QuestionId))]
        public QuizQuestion? Question { get; set; }
    }
}
