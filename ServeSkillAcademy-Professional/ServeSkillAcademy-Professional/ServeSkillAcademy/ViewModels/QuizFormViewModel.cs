using System.ComponentModel.DataAnnotations;

namespace ServeSkillAcademy.ViewModels
{
    public class QuizOptionInput
    {
        public int OptionId { get; set; }
        public string OptionText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }

    public class QuizQuestionInput
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public List<QuizOptionInput> Options { get; set; } = new();
    }

    public class QuizFormViewModel
    {
        public int QuizId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required, StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Range(1, 100)]
        [Display(Name = "Pass Percentage")]
        public int PassPercentage { get; set; } = 60;

        public string? CourseTitle { get; set; }

        public List<QuizQuestionInput> Questions { get; set; } = new();
    }
}
