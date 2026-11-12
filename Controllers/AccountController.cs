
using System.Security.Claims;
using InstructorInformationSystem.Data;
using InstructorInformationSystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InstructorInformationSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly PasswordHasher<Instructor> _passwordHasher =
            new PasswordHasher<Instructor>();

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // REGISTER - GET
        // ==========================================

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction(nameof(Dashboard));
            }

            return View();
        }

        // ==========================================
        // REGISTER - POST
        // ==========================================

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Instructor instructor)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction(nameof(Dashboard));
            }

            if (!ModelState.IsValid)
            {
                return View(instructor);
            }

            instructor.InstructorID =
                instructor.InstructorID.Trim();

            instructor.Email =
                instructor.Email.Trim();

            bool duplicateId = await _context.Instructors
                .AnyAsync(i =>
                    i.InstructorID == instructor.InstructorID);

            if (duplicateId)
            {
                ModelState.AddModelError(
                    nameof(Instructor.InstructorID),
                    "This Instructor ID is already registered.");

                return View(instructor);
            }

            bool duplicateEmail = await _context.Instructors
                .AnyAsync(i =>
                    i.Email.ToLower() == instructor.Email.ToLower());

            if (duplicateEmail)
            {
                ModelState.AddModelError(
                    nameof(Instructor.Email),
                    "This email is already registered.");

                return View(instructor);
            }

            // Securely hash the password before saving.
            instructor.PasswordHash =
                _passwordHasher.HashPassword(
                    instructor,
                    instructor.Password);

            _context.Instructors.Add(instructor);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Registration successful! You can now log in.";

            return RedirectToAction(nameof(Login));
        }

        // ==========================================
        // LOGIN - GET
        // ==========================================

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction(nameof(Dashboard));
            }

            return View();
        }

        // ==========================================
        // LOGIN - POST
        // ==========================================

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            string email,
            string password,
            string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction(nameof(Dashboard));
            }

            // Check required fields.
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Message =
                    "Please enter your email or Instructor ID and password.";

                return View();
            }

            // The user can enter an email or Instructor ID.
            string identifier = email.Trim();

            // Search for the registered instructor.
            Instructor? instructor = await _context.Instructors
                .FirstOrDefaultAsync(i =>
                    i.Email.ToLower() == identifier.ToLower() ||
                    i.InstructorID == identifier);

            if (instructor == null ||
                string.IsNullOrWhiteSpace(instructor.PasswordHash))
            {
                ViewBag.Message =
                    "Invalid email, Instructor ID, or password.";

                return View();
            }

            // Verify the password.
            var result = _passwordHasher.VerifyHashedPassword(
                instructor,
                instructor.PasswordHash,
                password);

            if (result == PasswordVerificationResult.Failed)
            {
                ViewBag.Message =
                    "Invalid email, Instructor ID, or password.";

                return View();
            }

            // Update password hash when needed.
            if (result ==
                PasswordVerificationResult.SuccessRehashNeeded)
            {
                instructor.PasswordHash =
                    _passwordHasher.HashPassword(
                        instructor,
                        password);

                await _context.SaveChangesAsync();
            }

            // ======================================
            // AUTOMATICALLY RETRIEVE INSTRUCTOR NAME
            // ======================================

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    instructor.Id.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    instructor.FullName),

                new Claim(
                    ClaimTypes.GivenName,
                    instructor.FirstName),

                new Claim(
                    ClaimTypes.Surname,
                    instructor.LastName)
            };

            // Create authentication identity.
            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            // Sign in the instructor.
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            // Return to the originally requested page,
            // if the redirect is safe and local.
            if (!string.IsNullOrEmpty(returnUrl) &&
                Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            // Otherwise, open the Dashboard.
            return RedirectToAction(nameof(Dashboard));
        }

        // ==========================================
        // DASHBOARD - LOGIN REQUIRED
        // ==========================================

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            // Automatically get instructor's name
            // from their authenticated login session.

            ViewBag.FirstName =
                User.FindFirst(ClaimTypes.GivenName)?.Value ?? "";

            ViewBag.LastName =
                User.FindFirst(ClaimTypes.Surname)?.Value ?? "";

            // Count instructor records in SQLite.
            ViewBag.TotalInstructors =
                await _context.Instructors.CountAsync();

            // Retrieve course information.
            var courses =
                CourseController.GetDashboardCourses();

            return View(courses);
        }

        // ==========================================
        // WELCOME - LOGIN REQUIRED
        // ==========================================

        [Authorize]
        [HttpGet]
        public IActionResult Welcome()
        {
            return RedirectToAction(nameof(Dashboard));
        }

        // ==========================================
        // LOGOUT
        // ==========================================

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(nameof(Login));
        }
    }
}
