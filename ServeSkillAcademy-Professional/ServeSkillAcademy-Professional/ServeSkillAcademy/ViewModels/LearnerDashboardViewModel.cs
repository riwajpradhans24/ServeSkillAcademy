namespace ServeSkillAcademy.ViewModels
{
    public class EnrolledCourseSummary
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public int ProgressPercentage { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class RecentQuizScoreSummary
    {
        public string QuizTitle { get; set; } = string.Empty;
        public string CourseTitle { get; set; } = string.Empty;
        public int Score { get; set; }
        public bool Passed { get; set; }
        public DateTime AttemptedAt { get; set; }
    }

    public class BadgeSummary
    {
        public string Title { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime AwardedAt { get; set; }
    }

    public class LearnerDashboardViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public List<EnrolledCourseSummary> EnrolledCourses { get; set; } = new();
        public List<RecentQuizScoreSummary> RecentQuizScores { get; set; } = new();
        public List<BadgeSummary> Badges { get; set; } = new();
    }
}
