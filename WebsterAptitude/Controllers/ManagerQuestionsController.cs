using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsterAptitude.Data;
using WebsterAptitude.Models;

[Authorize(Roles = "Manager")]
public class ManagerQuestionsController : Controller
{
    private readonly ApplicationDbContext _db;

    public ManagerQuestionsController(ApplicationDbContext db)
    {
        _db = db;
    }

    // LIST QUESTIONS BY SECTION
    public async Task<IActionResult> Index(int sectionId)
    {
        var section = await _db.TestSections.FindAsync(sectionId);
        if (section == null) return NotFound();

        var questions = await _db.Questions
            .Where(q => q.SectionId == sectionId)
            .OrderBy(q => q.Id)
            .ToListAsync();

        var vm = new ManagerQuestionsViewModel
        {
            Section = section,
            Questions = questions
        };

        return View(vm);
    }


    // ADD QUESTION (GET)
    public IActionResult Create(int sectionId)
    {
        return View(new Question { SectionId = sectionId });
    }

    // ADD QUESTION (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Question model)
    {
        if (!ModelState.IsValid)
            return View(model);

        _db.Questions.Add(model);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new { sectionId = model.SectionId });
    }

    // EDIT QUESTION (GET)
    public async Task<IActionResult> Edit(int id)
    {
        var question = await _db.Questions.FindAsync(id);
        if (question == null) return NotFound();

        return View(question);
    }

    // EDIT QUESTION (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Question model)
    {
        if (!ModelState.IsValid)
            return View(model);

        _db.Questions.Update(model);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new { sectionId = model.SectionId });
    }

    // DELETE QUESTION
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var question = await _db.Questions.FindAsync(id);
        if (question == null) return NotFound();

        int sectionId = question.SectionId;

        _db.Questions.Remove(question);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new { sectionId });
    }
}
