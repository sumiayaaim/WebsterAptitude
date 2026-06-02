using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebsterAptitude.Data;
using WebsterAptitude.Models;

namespace WebsterAptitude.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;

        public ManagerController(
            UserManager<IdentityUser> userManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        // Manager dashboard// Manager dashboard
        public IActionResult Index()
        {
            ViewBag.TotalCandidates = _db.CandidateProfiles.Count();
            ViewBag.TestAppeared = _db.CandidateTestAttempts.Count();
            ViewBag.HRCleared = 0;

            var candidates = _db.CandidateProfiles
                .OrderByDescending(c => c.Id)
                .ToList();

            return View(candidates);
        }


        // GET: Create Candidate
        public IActionResult CreateCandidate()
        {
            return View();
        }

        // POST: Create Candidate
        // POST: Create Candidate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCandidate(
            string fullName,
            string email,
            string password,
            string confirmPassword)
        {
            // 1️⃣ Required field validation
            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                ViewBag.Error = "All fields are required.";
                return View();
            }

            // 2️⃣ Password match validation
            if (password != confirmPassword)
            {
                ViewBag.Error = "Password and Confirm Password do not match.";
                return View();
            }


            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                ViewBag.Error = "User with this email already exists.";
                return View();
            }

            var user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                ViewBag.Error = string.Join(", ",
                    result.Errors.Select(e => e.Description));
                return View();
            }

            await _userManager.AddToRoleAsync(user, "Candidate");

            // Save candidate profile
            var profile = new CandidateProfile
            {
                UserId = user.Id,
                FullName = fullName,
                Email = email
            };

            _db.CandidateProfiles.Add(profile);
            await _db.SaveChangesAsync();

            ViewBag.Success = "Candidate created successfully.";
            return View();
        }
    }
}
