using Microsoft.AspNetCore.Identity;
using ServeSkillAcademy.Models;

namespace ServeSkillAcademy.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            await context.Database.EnsureCreatedAsync();
            foreach (var role in new[] { "Admin", "Learner" }) if (!await roleManager.RoleExistsAsync(role)) await roleManager.CreateAsync(new IdentityRole(role));
            await EnsureUser(userManager,"admin@serveskill.com","Admin@123","System Administrator","Admin");
            await EnsureUser(userManager,"learner@serveskill.com","Learner@123","Sample Learner","Learner");
            if (context.Courses.Any()) return;

            var categories = new[] {
                new Category{Name="Front Office",Description="Reception, reservations and guest relations.",Slug="front-office"},
                new Category{Name="Food & Beverage",Description="Restaurant service, etiquette and operations.",Slug="food-beverage"},
                new Category{Name="Housekeeping",Description="Room servicing, hygiene and hotel standards.",Slug="housekeeping"},
                new Category{Name="Culinary",Description="Kitchen fundamentals, food safety and preparation.",Slug="culinary"},
                new Category{Name="Professional Skills",Description="Communication, leadership and workplace readiness.",Slug="professional-skills"}
            }; context.Categories.AddRange(categories); await context.SaveChangesAsync();
            var front=categories[0];var fnb=categories[1];var house=categories[2];var culinary=categories[3];var professional=categories[4];
            var courses = new List<Course>{
                new Course{Title="Hospitality Foundations",Description="A practical starting pathway covering guest service, professional conduct, hotel operations and workplace communication.",CategoryId=professional.CategoryId,Level="Basic",Price=0,DurationDays=7,IsPublished=true,IsFeatured=true,ThumbnailUrl="/images/Hospitality.jpg"},
                new Course{Title="Front Desk Essentials",Description="Learn reception workflows, reservations, check-in and check-out, telephone etiquette and guest communication.",CategoryId=front.CategoryId,Level="Basic",Price=0,DurationDays=7,IsPublished=true,IsFeatured=true,ThumbnailUrl="/images/Frontdesk.jpg"},
                new Course{Title="Restaurant Service Fundamentals",Description="Build practical restaurant-service skills from table setup and order taking to service recovery and billing.",CategoryId=fnb.CategoryId,Level="Basic",Price=0,DurationDays=8,IsPublished=true,IsFeatured=true,ThumbnailUrl="/images/restaurant.jpg"},
                new Course{Title="Professional Housekeeping",Description="Learn room servicing, hygiene standards, guest privacy, inspection routines and quality-control practices.",CategoryId=house.CategoryId,Level="Basic",Price=0,DurationDays=7,IsPublished=true,ThumbnailUrl="/images/Housekeeping.jpg"},
                new Course{Title="Advanced Guest Relations & Complaint Handling",Description="A professional pathway for difficult conversations, service recovery, VIP handling, escalation and guest-experience design.",CategoryId=front.CategoryId,Level="Advanced",Price=1499,DurationDays=12,IsPublished=true,IsFeatured=true,ThumbnailUrl="/images/Guest Relations.jpg"},
                new Course{Title="Advanced Food & Beverage Operations",Description="Develop supervisory skills in service planning, team coordination, upselling, hygiene controls, cost awareness and operational improvement.",CategoryId=fnb.CategoryId,Level="Advanced",Price=1999,DurationDays=14,IsPublished=true,IsFeatured=true,ThumbnailUrl="/images/Food bevarages.jpg"},
                new Course{Title="Advanced Culinary & Food Safety",Description="An advanced pathway covering kitchen organization, food safety systems, preparation planning, quality control and waste reduction.",CategoryId=culinary.CategoryId,Level="Advanced",Price=2499,DurationDays=14,IsPublished=true,ThumbnailUrl="/images/Culinary.jpg"}
            }; context.Courses.AddRange(courses); await context.SaveChangesAsync();

            var themes = new Dictionary<string,string[]>{
                ["Hospitality Foundations"]=new[]{"The hospitality industry and guest expectations","Professional appearance and workplace etiquette","Guest-centered communication","Teamwork across hotel departments","Handling requests and creating service moments","Problem solving and service recovery basics","Workplace readiness and final review"},
                ["Front Desk Essentials"]=new[]{"The front desk role and guest journey","Reservation information and accuracy","Professional arrival and check-in","Telephone and digital communication","Guest records and privacy","Departure, billing and check-out","Front desk simulation and review"},
                ["Restaurant Service Fundamentals"]=new[]{"Restaurant roles and service sequence","Table setup and readiness","Greeting, seating and menu knowledge","Accurate order taking","Food and beverage service standards","Handling requests and complaints","Billing, payment and closing service","Full service simulation"},
                ["Professional Housekeeping"]=new[]{"Housekeeping operations and standards","Room preparation and trolley setup","Cleaning sequence and efficiency","Bathroom hygiene and sanitation","Amenities, linen and replenishment","Guest privacy, safety and lost property","Inspection and quality control"},
                ["Advanced Guest Relations & Complaint Handling"]=new[]{"Service recovery principles","Active listening and emotional cues","Complaint diagnosis and response","De-escalation techniques","Difficult conversations","VIP and high-expectation guests","Cultural awareness and inclusive service","Service recovery scripting","Escalation and documentation","Case-study decision making","Follow-up and retention","Final assessment readiness"},
                ["Advanced Food & Beverage Operations"]=new[]{"F&B operations and service design","Planning a smooth service","Team briefing and delegation","Upselling with guest value","Inventory awareness","Food safety controls","Guest experience design","Managing peak periods","Supervising service quality","Cost and waste awareness","Operational case study","Improvement planning","Leadership reflection","Final assessment readiness"},
                ["Advanced Culinary & Food Safety"]=new[]{"Kitchen organization and workflow","Personal hygiene standards","Cross-contamination prevention","Temperature control","Storage and labeling","Mise en place planning","Knife and equipment safety","Production planning","Quality control","Waste reduction","Kitchen teamwork","Food safety systems","Practical planning","Final assessment readiness"}
            };
            foreach(var course in courses){
                var list=themes[course.Title]; var order=1;
                for(int day=1;day<=course.DurationDays;day++){
                    var theme=list[Math.Min(day-1,list.Length-1)];
                    var chapters=new[]{($"Chapter 1 — Concepts & standards",$"{theme}: foundations"),($"Chapter 2 — Workplace workflow",$"{theme}: professional workflow"),($"Chapter 3 — Scenario & practice",$"{theme}: applied practice")};
                    foreach(var ch in chapters){context.Lessons.Add(new Lesson{CourseId=course.CourseId,Title=ch.Item2,Chapter=ch.Item1,DayNumber=day,OrderIndex=order++,EstimatedMinutes=25,Content=BuildLessonContent(course.Title,theme,ch.Item1,day)});}
                }
                var quiz=new Quiz{CourseId=course.CourseId,Title=$"{course.Title} Final Assessment",PassPercentage=70}; context.Quizzes.Add(quiz); await context.SaveChangesAsync();
                var questions=new[]{
                    new QuizQuestion{QuizId=quiz.QuizId,QuestionText=$"In {course.Title}, what is the best first step when a guest or colleague needs support?"},
                    new QuizQuestion{QuizId=quiz.QuizId,QuestionText="Which approach best demonstrates professional workplace practice?"},
                    new QuizQuestion{QuizId=quiz.QuizId,QuestionText="Why should a learner practise a procedure using a realistic scenario?"},
                    new QuizQuestion{QuizId=quiz.QuizId,QuestionText="What should be documented when a situation needs follow-up?"},
                    new QuizQuestion{QuizId=quiz.QuizId,QuestionText="What is a useful final check before completing a professional task?"}
                }; context.QuizQuestions.AddRange(questions); await context.SaveChangesAsync();
                foreach(var q in questions) context.QuizOptions.AddRange(new QuizOption{QuestionId=q.QuestionId,OptionText="Listen, confirm the need and follow the correct procedure",IsCorrect=true},new QuizOption{QuestionId=q.QuestionId,OptionText="Ignore the request until it disappears",IsCorrect=false},new QuizOption{QuestionId=q.QuestionId,OptionText="Guess without checking the procedure",IsCorrect=false},new QuizOption{QuestionId=q.QuestionId,OptionText="Pass responsibility to someone else immediately",IsCorrect=false});
                await context.SaveChangesAsync();
                // =============================
                // COURSE-SPECIFIC VIDEOS
                // =============================

                if (course.Title == "Hospitality Foundations")
                {
                    context.Videos.AddRange(
                        new Video
                        {
                            CourseId = course.CourseId,
                            Title = "Introduction to Hospitality",
                            Description = "Learn the basic concepts of hospitality and guest service.",
                            VideoUrl = "https://www.youtube.com/watch?v=pmP2RZZqIe4&pp=ygUcaW50cm9kdWNjdGlvbiB0byBob3NwaXRhbGl0eQ%3D%3D", 
                            DurationMinutes = 25,
                            OrderIndex = 1,
                            IsPublished = true
                        },
                        new Video
                        {
                            CourseId = course.CourseId,
                            Title = "Professional Customer Service",
                            Description = "Learn the fundamentals of professional Customer service.",
                            VideoUrl = "https://www.youtube.com/watch?v=Eq7Z2i7m57Y&pp=ygUdcHJvZmVzc2lvbmFsIGN1c3RvbWVyIHNlcnZpY2U%3D", 
                            DurationMinutes = 8,
                            OrderIndex = 2,
                            IsPublished = true
                        }
                    );
                }
                else if (course.Title == "Front Desk Essentials")
                {
                    context.Videos.AddRange(
                        new Video
                        {
                            CourseId = course.CourseId,
                            Title = "Front Desk Operations",
                            Description = "Learn the basic responsibilities and operations of a hotel front desk.",
                            VideoUrl = "https://www.youtube.com/watch?v=tR8koWF3s2E&t=104s&pp=ygUWZnJvbnQgZGVzayAgb3BlcmF0aW9ucw%3D%3D", 
                            DurationMinutes = 15,
                            OrderIndex = 1,
                            IsPublished = true
                        },
                        new Video
                        {
                            CourseId = course.CourseId,
                            Title = "Hotel Check-in and Check-out",
                            Description = "Learn professional check-in and check-out procedures.",
                            VideoUrl = "https://www.youtube.com/watch?v=xhdKDjb_GFM&pp=ygUcSG90ZWwgQ2hlY2staW4gYW5kIENoZWNrLW91dA%3D%3D", 
                            DurationMinutes = 10,
                            OrderIndex = 2,
                            IsPublished = true
                        }
                    );
                }
                else if (course.Title == "Restaurant Service Fundamentals")
                {
                    context.Videos.AddRange(
                        new Video
                        {
                            CourseId = course.CourseId,
                            Title = "Restaurant Service Basics",
                            Description = "Learn the fundamentals of professional restaurant service.",
                            VideoUrl = "https://www.youtube.com/watch?v=iIIR2d_vIz0&list=PLxcVdHxGGxDG7PqajHUxL1Mcj4hTlYmJd", 
                            DurationMinutes = 10,
                            OrderIndex = 1,
                            IsPublished = true
                        },
                        new Video
                        {
                            CourseId = course.CourseId,
                            Title = "Table Service Standards",
                            Description = "Learn professional table setup and service standards.",
                            VideoUrl = "https://www.youtube.com/watch?v=eadlA0l8CGQ&pp=ygUbdGFibGUgc2VydmljZSBpbiByZXN0YXVyYW50", 
                            DurationMinutes = 5,
                            OrderIndex = 2,
                            IsPublished = true
                        }
                    );
                }
                else if (course.Title == "Professional Housekeeping")
                {
                    context.Videos.AddRange(
                        new Video
                        {
                            CourseId = course.CourseId,
                            Title = "Hotel Housekeeping Basics",
                            Description = "Learn the basic principles of professional hotel housekeeping.",
                            VideoUrl = "https://www.youtube.com/watch?v=ggZ8QxMN8Uw&pp=ygUUSG91c2VrZWVwaW5nIGJhc2ljcyA%3D", 
                            DurationMinutes = 10,
                            OrderIndex = 1,
                            IsPublished = true
                        },
                        new Video
                        {
                            CourseId = course.CourseId,
                            Title = "Room Cleaning Standards",
                            Description = "Learn professional room cleaning and hygiene standards.",
                            VideoUrl = "https://www.youtube.com/watch?v=5Xva5BAdRGk&pp=ygUhcm9vbSBjbGVhbmluZyBzdGFuZGFyZHMgaW4gaG90ZWxz", 
                            DurationMinutes = 3,
                            OrderIndex = 2,
                            IsPublished = true
                        }
                    );
                }
                else if (course.Title == "Advanced Guest Relations & Complaint Handling")
                {
                    context.Videos.AddRange(
                        new Video
                        {
                            CourseId = course.CourseId,
                            Title = "Guest Relations",
                            Description = "Learn advanced guest relationship and communication skills.",
                            VideoUrl = "https://www.youtube.com/watch?v=x27mkgcUQ5Y&t=406s&pp=ygUsQWR2YW5jZSBndWVzdCAgcmVsYXRpb25zIGFuZCBjb21tdW5pY2F0aW9ucyA%3D", 
                            DurationMinutes = 8,
                            OrderIndex = 1,
                            IsPublished = true
                        },
                        new Video
                        {
                            CourseId = course.CourseId,
                            Title = "Handling Guest Complaints",
                            Description = "Learn professional techniques for handling difficult guest situations.",
                            VideoUrl = "https://www.youtube.com/watch?v=baCQpnxZxQQ&pp=ygUhaG93ICB0byBoYW5kbGUgZ3Vlc3QgY29tcGxhaW5udHMg", 
                            DurationMinutes = 10,
                            OrderIndex = 2,
                            IsPublished = true
                        }
                    );
                }
                else if (course.Title == "Advanced Food & Beverage Operations")
                {
                    context.Videos.AddRange(
                        new Video
                        {
                            CourseId = course.CourseId,
                            Title = "Food and Beverage Operations",
                            Description = "Learn advanced food and beverage operational practices.",
                            VideoUrl = "https://www.youtube.com/watch?v=zabfmu0Ac0A&pp=ygUnZm9vZCBhbmQgYmV2ZXJhZ2Ugb3BlcmF0aW9uYWwgcHJhY3RpY2Vz", 
                            DurationMinutes = 5,
                            OrderIndex = 1,
                            IsPublished = true
                        },
                        new Video
                        {
                            CourseId = course.CourseId,
                            Title = "Restaurant Management",
                            Description = "Learn professional restaurant management and service operations.",
                            VideoUrl = "https://www.youtube.com/watch?v=W6kbZHbEJrk&pp=ygUWIHJlc3RhdXJhbnQgbWFuYWdlbWVudA%3D%3D", 
                            DurationMinutes = 8,
                            OrderIndex = 2,
                            IsPublished = true
                        }
                    );
                }
                else if (course.Title == "Advanced Culinary & Food Safety")
                {
                    context.Videos.AddRange(
                        new Video
                        {
                            CourseId = course.CourseId,
                            Title = "Advanced Culinary Skills",
                            Description = "Learn advanced culinary techniques and kitchen practices.",
                            VideoUrl = "https://www.youtube.com/watch?v=8A_i-PFRieQ&pp=ygUcYWR2YW5jZWQgY3VsaW5hcnkgdGVjaG5pcXVlcw%3D%3D",  
                            DurationMinutes = 10,
                            OrderIndex = 1,
                            IsPublished = true
                        },
                        new Video
                        {
                            CourseId = course.CourseId,
                            Title = "Food Safety and HACCP",
                            Description = "Learn advanced food safety practices and HACCP principles.",
                            VideoUrl = "https://www.youtube.com/watch?v=j-XNzBUKOoE&pp=ygUVZm9vZCBzYWZldHkgYW5kIGhhY2Nw0gcJCS0MAYcqIYzv", 
                            DurationMinutes = 30,
                            OrderIndex = 2,
                            IsPublished = true
                        }
                    );
                }

                await context.SaveChangesAsync(); ;
            }
        }
        private static async Task EnsureUser(UserManager<ApplicationUser> userManager,string email,string password,string name,string role){var user=await userManager.FindByEmailAsync(email);if(user!=null)return;user=new ApplicationUser{UserName=email,Email=email,EmailConfirmed=true,FullName=name,Role=role,Workplace="ServeSkill Academy",CreatedAt=DateTime.UtcNow,IsActive=true};var r=await userManager.CreateAsync(user,password);if(r.Succeeded)await userManager.AddToRoleAsync(user,role);}
        private static string BuildLessonContent(string course,string theme,string chapter,int day)=>$"Learning objective\nBy the end of Day {day}, {course} learners should be able to explain the key standards behind {theme.ToLower()}, apply the correct workplace sequence and make a professional decision in a realistic situation.\n\nCore lesson\n{chapter} focuses on {theme}. Start by identifying the expected outcome for the guest, colleague or operation. A professional response is not only about being polite; it is about accuracy, timing, safety, consistency and clear communication. Review the relevant workplace standard, then break the task into small actions that can be repeated consistently.\n\nReal-world example\nImagine you are working a busy shift and a guest asks for help while another task is already in progress. Pause long enough to acknowledge the person, clarify what they need, decide what is urgent, and communicate what you will do next. If another department is involved, record or hand over the information so that the guest does not need to repeat the story.\n\nPractice task\nWrite a five-step procedure for the situation described in this chapter. Include the opening communication, information you need to confirm, action you would take, safety or quality check, and follow-up. Then explain why each step matters.\n\nReflection\nBefore moving on, answer: What could go wrong if this procedure is skipped? What would a supervisor expect to see? How would you know the task was completed to standard?\n\nChapter checkpoint\nYou are ready for the next chapter when you can explain the process without reading the notes and can apply it to a new workplace scenario.";
    }
}
