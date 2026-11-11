namespace ServeSkillAcademy.ViewModels
{
    public class QuizTakeOption
    {
        public int OptionId { get; set; }
        public string OptionText { get; set; } = string.Empty;
    }

    public class QuizTakeQuestion
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public List<QuizTakeOption> Options { get; set; } = new();
        // Bound from the submitted form: QuestionId -> selected OptionId
        public int SelectedOptionId { get; set; }
    }

    public class QuizTakeViewModel
    {
        public int QuizId { get; set; }
        public string QuizTitle { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public List<QuizTakeQuestion> Questions { get; set; } = new();
    }

    public class QuizResultViewModel
    {
        public string QuizTitle { get; set; } = string.Empty;
        public int Score { get; set; }
        public bool Passed { get; set; }
        public int PassPercentage { get; set; }
        public int CourseId { get; set; }
        public bool CourseCompleted { get; set; }
        public bool BadgeAwarded { get; set; }
    }
}
