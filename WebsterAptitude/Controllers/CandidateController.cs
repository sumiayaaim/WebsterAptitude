using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsterAptitude.Data;
using WebsterAptitude.Models;

namespace WebsterAptitude.Controllers
{
    [Authorize(Roles = "Candidate")]
    public class CandidateController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public CandidateController(
            ApplicationDbContext db,
            UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // SINGLE, CORRECT DASHBOARD
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var testAttempt = await _db.CandidateTestAttempts
                .FirstOrDefaultAsync(x => x.CandidateUserId == userId);

            // Test not started yet
            if (testAttempt == null)
            {
                ViewBag.HasStarted = false;
                return View();
            }

            // Test started → show sections
            var sections = await _db.TestSections
                .OrderBy(s => s.Order)
                .ToListAsync();

            var completedSectionIds = await _db.CandidateSectionAttempts
                .Where(x => x.TestAttemptId == testAttempt.Id && x.IsCompleted)
                .Select(x => x.SectionId)
                .ToListAsync();

            ViewBag.HasStarted = true;
            ViewBag.Sections = sections;
            ViewBag.Completed = completedSectionIds;

            // ================================
            // ✅ ADD RESULT LOGIC HERE
            // ================================
            if (testAttempt.IsCompleted)
            {
                var score = await _db.CandidateAnswers
                    .Where(a => _db.CandidateSectionAttempts
                        .Any(s => s.Id == a.SectionAttemptId &&
                                  s.TestAttemptId == testAttempt.Id))
                    .SumAsync(a => a.MarksObtained);

                int maxScore = await _db.Questions.SumAsync(q => q.Marks);

                bool passed = score >= (maxScore * 0.5); // example: 50% pass rule

                ViewBag.Score = score;
                ViewBag.MaxScore = maxScore;
                ViewBag.ResultStatus = passed ? "Pass" : "Fail";
            }

            return View();
        }


        // START TEST
        public async Task<IActionResult> StartTest()
        {
            var userId = _userManager.GetUserId(User);

            var existing = await _db.CandidateTestAttempts
                .FirstOrDefaultAsync(x => x.CandidateUserId == userId);

            if (existing == null)
            {
                _db.CandidateTestAttempts.Add(new CandidateTestAttempt
                {
                    CandidateUserId = userId
                });
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // START SECTION (FINAL VERSION)
        public async Task<IActionResult> StartSection(int sectionId)
        {
            var userId = _userManager.GetUserId(User);

            var testAttempt = await _db.CandidateTestAttempts
                .FirstAsync(x => x.CandidateUserId == userId);

            var exists = await _db.CandidateSectionAttempts
                .AnyAsync(x => x.TestAttemptId == testAttempt.Id && x.SectionId == sectionId);

            if (!exists)
            {
                _db.CandidateSectionAttempts.Add(new CandidateSectionAttempt
                {
                    TestAttemptId = testAttempt.Id,
                    SectionId = sectionId
                });
                await _db.SaveChangesAsync();
            }

            // 🔴 ADD THIS BLOCK (TIMER INFO)
            var section = await _db.TestSections.FindAsync(sectionId);
            ViewBag.TimeLimitSeconds = section!.TimeLimitSeconds;

            var questions = await _db.Questions
                .Where(q => q.SectionId == sectionId)
                .ToListAsync();

            return View("SectionQuestions", questions);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitSection(int[] questionIds, string[] answers)
        {
            var userId = _userManager.GetUserId(User);

            var testAttempt = await _db.CandidateTestAttempts
                .FirstAsync(x => x.CandidateUserId == userId);

            // Get current active section attempt
            var sectionAttempt = await _db.CandidateSectionAttempts
                .Where(x => x.TestAttemptId == testAttempt.Id && !x.IsCompleted)
                .OrderByDescending(x => x.Id)
                .FirstAsync();

            // Save answers
            for (int i = 0; i < questionIds.Length; i++)
            {
                var question = await _db.Questions.FindAsync(questionIds[i]);
                if (question == null) continue;

                bool isCorrect = question.CorrectOption == answers[i];
                int marks = isCorrect ? question.Marks : 0;

                _db.CandidateAnswers.Add(new CandidateAnswer
                {
                    SectionAttemptId = sectionAttempt.Id,
                    QuestionId = question.Id,
                    SelectedOption = answers[i],
                    IsCorrect = isCorrect,
                    MarksObtained = marks
                });
            }

            // Mark section completed
            sectionAttempt.IsCompleted = true;
            sectionAttempt.CompletedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync(); // 🔴 MUST save BEFORE counting

            // 🔴 CHECK IF ALL SECTIONS ARE COMPLETED
            var totalSections = await _db.TestSections.CountAsync();

            var completedSections = await _db.CandidateSectionAttempts
                .CountAsync(x => x.TestAttemptId == testAttempt.Id && x.IsCompleted);

            if (completedSections == totalSections)
            {
                testAttempt.IsCompleted = true;
                await _db.SaveChangesAsync();

                return RedirectToAction("FinalResult");
            }

            // Otherwise go back to dashboard
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> FinalResult()
        {
            var userId = _userManager.GetUserId(User);

            var testAttempt = await _db.CandidateTestAttempts
                .FirstAsync(x => x.CandidateUserId == userId);

            var totalMarks = await _db.CandidateAnswers
                .Where(a => _db.CandidateSectionAttempts
                    .Any(s => s.Id == a.SectionAttemptId && s.TestAttemptId == testAttempt.Id))
                .SumAsync(a => a.MarksObtained);

            bool passed = totalMarks >= 4; // pass rule

            ViewBag.TotalMarks = totalMarks;
            ViewBag.Passed = passed;

            return View();
        }

    }
}
