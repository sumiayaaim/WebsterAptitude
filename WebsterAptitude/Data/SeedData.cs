using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;              // 🔴 REQUIRED
using System.Threading.Tasks;
using WebsterAptitude.Models;

namespace WebsterAptitude.Data
{
    public static class SeedData
    {
        public static async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = { "Manager", "Candidate" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public static async Task CreateManager(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var email = "manager@webster.com";
            var password = "Manager@123";

            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, password);
                await userManager.AddToRoleAsync(user, "Manager");
            }
        }

        // ✅ FIXED
        public static async Task SeedSections(IServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetRequiredService<ApplicationDbContext>();

            if (!db.TestSections.Any())
            {
                db.TestSections.AddRange(
                    new TestSection { Name = "General Knowledge", Order = 1, TimeLimitSeconds = 1900 },
                    new TestSection { Name = "Mathematics", Order = 2, TimeLimitSeconds = 1900 },
                    new TestSection { Name = "Computer Technology", Order = 3, TimeLimitSeconds = 1900 }
                );


                await db.SaveChangesAsync();
            }
        }

        public static async Task SeedSampleQuestions(IServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetRequiredService<ApplicationDbContext>();

            if (db.Questions.Any())
                return;

            var gk = db.TestSections.First(s => s.Id == 1);
            var math = db.TestSections.First(s => s.Id == 2);
            var tech = db.TestSections.First(s => s.Id == 3);

            db.Questions.AddRange(

                // General Knowledge
                new Question
                {
                    SectionId = gk.Id,
                    QuestionText = "What is the capital of Pakistan?",
                    OptionA = "Karachi",
                    OptionB = "Lahore",
                    OptionC = "Islamabad",
                    OptionD = "Quetta",
                    CorrectOption = "C",
                    Marks = 1
                },
                new Question
                {
                    SectionId = gk.Id,
                    QuestionText = "Which ocean is largest?",
                    OptionA = "Indian",
                    OptionB = "Pacific",
                    OptionC = "Atlantic",
                    OptionD = "Arctic",
                    CorrectOption = "B",
                    Marks = 1
                },

                // Mathematics
                new Question
                {
                    SectionId = math.Id,
                    QuestionText = "What is 25 × 4?",
                    OptionA = "50",
                    OptionB = "75",
                    OptionC = "100",
                    OptionD = "125",
                    CorrectOption = "C",
                    Marks = 1
                },
                new Question
                {
                    SectionId = math.Id,
                    QuestionText = "What is the square root of 81?",
                    OptionA = "7",
                    OptionB = "8",
                    OptionC = "9",
                    OptionD = "10",
                    CorrectOption = "C",
                    Marks = 1
                },

                // Computer Technology
                new Question
                {
                    SectionId = tech.Id,
                    QuestionText = "What does CPU stand for?",
                    OptionA = "Central Processing Unit",
                    OptionB = "Computer Personal Unit",
                    OptionC = "Central Program Utility",
                    OptionD = "Control Processing Unit",
                    CorrectOption = "A",
                    Marks = 1
                },
                new Question
                {
                    SectionId = tech.Id,
                    QuestionText = "Which of the following is an operating system?",
                    OptionA = "MS Word",
                    OptionB = "Windows",
                    OptionC = "Google",
                    OptionD = "Intel",
                    CorrectOption = "B",
                    Marks = 1
                }
            );

            await db.SaveChangesAsync();
        }
    }
}
