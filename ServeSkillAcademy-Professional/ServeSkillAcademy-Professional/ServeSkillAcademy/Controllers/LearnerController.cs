using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServeSkillAcademy.Data;
using ServeSkillAcademy.Models;
using ServeSkillAcademy.ViewModels;

namespace ServeSkillAcademy.Controllers
{
    [Authorize(Roles = "Learner")]
    public class LearnerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public LearnerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) { _context=context; _userManager=userManager; }

        public async Task<IActionResult> Dashboard()
        {
            var userId = _userManager.GetUserId(User)!;
            var user = await _userManager.GetUserAsync(User);
            var enrollments = await _context.Enrollments.Include(e=>e.Course).Where(e=>e.UserId==userId).ToListAsync();
            var recentAttempts = await _context.QuizAttempts.Include(a=>a.Quiz).ThenInclude(q=>q!.Course).Where(a=>a.UserId==userId).OrderByDescending(a=>a.AttemptedAt).Take(8).ToListAsync();
            var badges = await _context.Badges.Where(b=>b.UserId==userId).OrderByDescending(b=>b.AwardedAt).ToListAsync();
            return View(new LearnerDashboardViewModel {
                FullName=user?.FullName ?? "Learner",
                EnrolledCourses=enrollments.Select(e=>new EnrolledCourseSummary { CourseId=e.CourseId, Title=e.Course?.Title??"", ThumbnailUrl=e.Course?.ThumbnailUrl, ProgressPercentage=e.ProgressPercentage, IsCompleted=e.IsCompleted }).ToList(),
                RecentQuizScores=recentAttempts.Select(a=>new RecentQuizScoreSummary { QuizTitle=a.Quiz?.Title??"", CourseTitle=a.Quiz?.Course?.Title??"", Score=a.Score, Passed=a.Passed, AttemptedAt=a.AttemptedAt }).ToList(),
                Badges=badges.Select(b=>new BadgeSummary { Title=b.Title, ImageUrl=b.ImageUrl, AwardedAt=b.AwardedAt }).ToList()
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int courseId)
        {
            var userId=_userManager.GetUserId(User)!;
            var course=await _context.Courses.FirstOrDefaultAsync(c=>c.CourseId==courseId&&c.IsPublished);
            if(course==null)return NotFound();
            if(course.Price>0){var paid=await _context.PaymentTransactions.AnyAsync(p=>p.UserId==userId&&p.CourseId==courseId&&p.Status=="Approved"); if(!paid)return RedirectToAction("Checkout","Payment",new{courseId});}
            if(!await _context.Enrollments.AnyAsync(e=>e.UserId==userId&&e.CourseId==courseId)){_context.Enrollments.Add(new Enrollment{UserId=userId,CourseId=courseId});await _context.SaveChangesAsync();TempData["Success"]="You are enrolled. Your learning plan is ready.";}
            return RedirectToAction(nameof(Lesson),new{courseId});
        }

        public async Task<IActionResult> Lesson(int courseId, int? lessonId)
        {
            var userId=_userManager.GetUserId(User)!;
            if(!await _context.Enrollments.AnyAsync(e=>e.UserId==userId&&e.CourseId==courseId))return Forbid();
            var course=await _context.Courses.Include(c=>c.Lessons.OrderBy(l=>l.OrderIndex)).Include(c=>c.Quizzes).Include(c=>c.Videos.Where(v=>v.IsPublished).OrderBy(v=>v.OrderIndex)).FirstOrDefaultAsync(c=>c.CourseId==courseId);
            if(course==null)return NotFound();
            var completedIds=await _context.LessonCompletions.Where(x=>x.UserId==userId&&course.Lessons.Select(l=>l.LessonId).Contains(x.LessonId)).Select(x=>x.LessonId).ToListAsync();
            var lesson=lessonId.HasValue?course.Lessons.FirstOrDefault(l=>l.LessonId==lessonId):course.Lessons.FirstOrDefault();
            ViewBag.Course=course; ViewBag.CurrentLessonId=lesson?.LessonId; ViewBag.CompletedIds=completedIds; ViewBag.CompletedCount=completedIds.Count; ViewBag.TotalLessons=course.Lessons.Count;
            ViewBag.CourseLessonsComplete=course.Lessons.Count>0&&completedIds.Count>=course.Lessons.Count;
            return View(lesson);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkComplete(int courseId,int lessonId)
        {
            var userId=_userManager.GetUserId(User)!;
            var enrollment=await _context.Enrollments.FirstOrDefaultAsync(e=>e.UserId==userId&&e.CourseId==courseId);
            var lesson=await _context.Lessons.FirstOrDefaultAsync(l=>l.LessonId==lessonId&&l.CourseId==courseId);
            if(enrollment==null||lesson==null)return NotFound();
            if(!await _context.LessonCompletions.AnyAsync(x=>x.UserId==userId&&x.LessonId==lessonId))_context.LessonCompletions.Add(new LessonCompletion{UserId=userId,LessonId=lessonId});
            await _context.SaveChangesAsync();
            var total=await _context.Lessons.CountAsync(l=>l.CourseId==courseId);
            var done=await _context.LessonCompletions.CountAsync(x=>x.UserId==userId&&x.Lesson!.CourseId==courseId);
            enrollment.ProgressPercentage=total==0?0:(int)Math.Round(100.0*done/total);
            if(done>=total)enrollment.ProgressPercentage=100;
            await _context.SaveChangesAsync();
            if(done>=total)TempData["Success"]="All lesson chapters are complete. Take the course assessment to earn your completion badge.";
            var next=await _context.Lessons.Where(l=>l.CourseId==courseId&&!_context.LessonCompletions.Any(x=>x.UserId==userId&&x.LessonId==l.LessonId)).OrderBy(l=>l.OrderIndex).FirstOrDefaultAsync();
            return RedirectToAction(nameof(Lesson),new{courseId,lessonId=next?.LessonId??lessonId});
        }

        public async Task<IActionResult> Video(int id)
        {
            var userId=_userManager.GetUserId(User)!;
            var video=await _context.Videos.Include(v=>v.Course).FirstOrDefaultAsync(v=>v.VideoId==id&&v.IsPublished);
            if(video==null)return NotFound();
            if(!await _context.Enrollments.AnyAsync(e=>e.UserId==userId&&e.CourseId==video.CourseId))return Forbid();
            return View(video);
        }

        public async Task<IActionResult> TakeQuiz(int quizId)
        {
            var userId=_userManager.GetUserId(User)!;
            var quiz=await _context.Quizzes.Include(q=>q.Course).Include(q=>q.Questions).ThenInclude(qq=>qq.Options).FirstOrDefaultAsync(q=>q.QuizId==quizId);
            if(quiz==null)return NotFound();
            if(!await _context.Enrollments.AnyAsync(e=>e.UserId==userId&&e.CourseId==quiz.CourseId))return Forbid();
            var vm=new QuizTakeViewModel{QuizId=quiz.QuizId,QuizTitle=quiz.Title,CourseId=quiz.CourseId,CourseTitle=quiz.Course?.Title??"",Questions=quiz.Questions.Select(q=>new QuizTakeQuestion{QuestionId=q.QuestionId,QuestionText=q.QuestionText,Options=q.Options.Select(o=>new QuizTakeOption{OptionId=o.OptionId,OptionText=o.OptionText}).ToList()}).ToList()};
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitQuiz(int quizId,Dictionary<int,int> answers)
        {
            var userId=_userManager.GetUserId(User)!;
            var quiz=await _context.Quizzes.Include(q=>q.Course).Include(q=>q.Questions).ThenInclude(qq=>qq.Options).FirstOrDefaultAsync(q=>q.QuizId==quizId);
            if(quiz==null)return NotFound();
            var enrollment=await _context.Enrollments.FirstOrDefaultAsync(e=>e.UserId==userId&&e.CourseId==quiz.CourseId);
            if(enrollment==null)return Forbid();
            int correct=0; foreach(var q in quiz.Questions){if(answers.TryGetValue(q.QuestionId,out var selected)&&q.Options.Any(o=>o.OptionId==selected&&o.IsCorrect))correct++;}
            var total=quiz.Questions.Count; var score=total==0?0:(int)Math.Round(100.0*correct/total); var passed=score>=quiz.PassPercentage;
            _context.QuizAttempts.Add(new QuizAttempt{UserId=userId,QuizId=quizId,Score=score,Passed=passed});
            bool courseCompleted=false,badgeAwarded=false;
            var lessonTotal=await _context.Lessons.CountAsync(l=>l.CourseId==quiz.CourseId); var lessonDone=await _context.LessonCompletions.CountAsync(x=>x.UserId==userId&&x.Lesson!.CourseId==quiz.CourseId);
            if(passed&&lessonDone>=lessonTotal){enrollment.ProgressPercentage=100;if(!enrollment.IsCompleted){enrollment.IsCompleted=true;courseCompleted=true;}var exists=await _context.Badges.AnyAsync(b=>b.UserId==userId&&b.Title==$"{quiz.Course!.Title} — Certified");if(!exists){_context.Badges.Add(new Badge{UserId=userId,Title=$"{quiz.Course!.Title} — Certified",Description="Awarded for completing the full learning pathway and passing the final assessment.",ImageUrl="/images/badge-certified.svg"});badgeAwarded=true;}};
            await _context.SaveChangesAsync();
            return View("QuizResult",new QuizResultViewModel{QuizTitle=quiz.Title,Score=score,Passed=passed,PassPercentage=quiz.PassPercentage,CourseId=quiz.CourseId,CourseCompleted=courseCompleted,BadgeAwarded=badgeAwarded});
        }
    }
}
