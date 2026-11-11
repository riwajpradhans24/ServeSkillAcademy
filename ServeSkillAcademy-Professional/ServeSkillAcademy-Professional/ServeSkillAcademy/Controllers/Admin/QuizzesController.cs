using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServeSkillAcademy.Data;
using ServeSkillAcademy.Models;
using ServeSkillAcademy.ViewModels;

namespace ServeSkillAcademy.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]")]
    public class QuizzesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public QuizzesController(ApplicationDbContext context) => _context = context;

        [HttpGet("ForCourse/{courseId}")]
        public async Task<IActionResult> ForCourse(int courseId)
        {
            var course = await _context.Courses.Include(c => c.Quizzes).ThenInclude(q => q.Questions)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);
            if (course == null) return NotFound();
            return View(course);
        }

        [HttpGet("Create/{courseId}")]
        public async Task<IActionResult> Create(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null) return NotFound();

            var vm = new QuizFormViewModel
            {
                CourseId = courseId,
                CourseTitle = course.Title,
                Questions = new List<QuizQuestionInput>
                {
                    NewQuestion()
                }
            };
            return View(vm);
        }

        [HttpPost("Create/{courseId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int courseId, QuizFormViewModel vm)
        {
            if (!ModelState.IsValid || !vm.Questions.Any())
            {
                vm.CourseId = courseId;
                return View(vm);
            }

            var quiz = new Quiz
            {
                CourseId = courseId,
                Title = vm.Title,
                PassPercentage = vm.PassPercentage
            };
            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();

            foreach (var q in vm.Questions.Where(q => !string.IsNullOrWhiteSpace(q.QuestionText)))
            {
                var question = new QuizQuestion { QuizId = quiz.QuizId, QuestionText = q.QuestionText };
                _context.QuizQuestions.Add(question);
                await _context.SaveChangesAsync();

                foreach (var o in q.Options.Where(o => !string.IsNullOrWhiteSpace(o.OptionText)))
                {
                    _context.QuizOptions.Add(new QuizOption
                    {
                        QuestionId = question.QuestionId,
                        OptionText = o.OptionText,
                        IsCorrect = o.IsCorrect
                    });
                }
            }
            await _context.SaveChangesAsync();

            TempData["Success"] = "Quiz created.";
            return RedirectToAction(nameof(ForCourse), new { courseId });
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Course)
                .Include(q => q.Questions).ThenInclude(qq => qq.Options)
                .FirstOrDefaultAsync(q => q.QuizId == id);
            if (quiz == null) return NotFound();

            var vm = new QuizFormViewModel
            {
                QuizId = quiz.QuizId,
                CourseId = quiz.CourseId,
                CourseTitle = quiz.Course?.Title,
                Title = quiz.Title,
                PassPercentage = quiz.PassPercentage,
                Questions = quiz.Questions.Select(q => new QuizQuestionInput
                {
                    QuestionId = q.QuestionId,
                    QuestionText = q.QuestionText,
                    Options = q.Options.Select(o => new QuizOptionInput
                    {
                        OptionId = o.OptionId,
                        OptionText = o.OptionText,
                        IsCorrect = o.IsCorrect
                    }).ToList()
                }).ToList()
            };
            if (!vm.Questions.Any()) vm.Questions.Add(NewQuestion());

            return View(vm);
        }

        // Simplest reliable approach for a coursework-scale app: replace all
        // questions/options for this quiz with what was submitted.
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, QuizFormViewModel vm)
        {
            if (id != vm.QuizId) return NotFound();
            if (!ModelState.IsValid) return View(vm);

            var quiz = await _context.Quizzes
                .Include(q => q.Questions).ThenInclude(qq => qq.Options)
                .FirstOrDefaultAsync(q => q.QuizId == id);
            if (quiz == null) return NotFound();

            quiz.Title = vm.Title;
            quiz.PassPercentage = vm.PassPercentage;

            _context.QuizQuestions.RemoveRange(quiz.Questions); // cascades to options
            await _context.SaveChangesAsync();

            foreach (var q in vm.Questions.Where(q => !string.IsNullOrWhiteSpace(q.QuestionText)))
            {
                var question = new QuizQuestion { QuizId = quiz.QuizId, QuestionText = q.QuestionText };
                _context.QuizQuestions.Add(question);
                await _context.SaveChangesAsync();

                foreach (var o in q.Options.Where(o => !string.IsNullOrWhiteSpace(o.OptionText)))
                {
                    _context.QuizOptions.Add(new QuizOption
                    {
                        QuestionId = question.QuestionId,
                        OptionText = o.OptionText,
                        IsCorrect = o.IsCorrect
                    });
                }
            }
            await _context.SaveChangesAsync();

            TempData["Success"] = "Quiz updated.";
            return RedirectToAction(nameof(ForCourse), new { courseId = quiz.CourseId });
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null) return NotFound();

            var courseId = quiz.CourseId;
            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Quiz deleted.";
            return RedirectToAction(nameof(ForCourse), new { courseId });
        }

        private static QuizQuestionInput NewQuestion() => new()
        {
            Options = new List<QuizOptionInput>
            {
                new(), new(), new(), new()
            }
        };
    }
}
