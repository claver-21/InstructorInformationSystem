
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InstructorInformationSystem.Data;
using InstructorInformationSystem.Models;

namespace InstructorInformationSystem.Controllers
{
    [Authorize]
    public class InstructorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InstructorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // INSTRUCTOR LIST
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var instructors = await _context.Instructors
                .AsNoTracking()
                .OrderBy(i => i.LastName)
                .ThenBy(i => i.FirstName)
                .ToListAsync();

            return View(instructors);
        }

        // ==========================================
        // CREATE - GET
        // ==========================================

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // ==========================================
        // CREATE - POST
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Instructor instructor)
        {
            ModelState.Remove(nameof(Instructor.Password));
            ModelState.Remove(nameof(Instructor.ConfirmPassword));

            if (!ModelState.IsValid)
            {
                return View(instructor);
            }

            instructor.InstructorID = instructor.InstructorID.Trim();
            instructor.Email = instructor.Email.Trim();

            bool duplicateId = await _context.Instructors
                .AnyAsync(i => i.InstructorID == instructor.InstructorID);

            if (duplicateId)
            {
                ModelState.AddModelError(
                    nameof(Instructor.InstructorID),
                    "This Instructor ID already exists.");

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

            instructor.PasswordHash = null;

            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Instructor added successfully.";

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // EDIT - GET
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var instructor = await _context.Instructors.FindAsync(id);

            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // ==========================================
        // EDIT - POST
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Instructor instructor)
        {
            if (id != instructor.Id)
            {
                return BadRequest();
            }

            ModelState.Remove(nameof(Instructor.Password));
            ModelState.Remove(nameof(Instructor.ConfirmPassword));

            if (!ModelState.IsValid)
            {
                return View(instructor);
            }

            var existing = await _context.Instructors.FindAsync(id);

            if (existing == null)
            {
                return NotFound();
            }

            instructor.InstructorID = instructor.InstructorID.Trim();
            instructor.Email = instructor.Email.Trim();

            bool duplicateId = await _context.Instructors
                .AnyAsync(i =>
                    i.Id != id &&
                    i.InstructorID == instructor.InstructorID);

            if (duplicateId)
            {
                ModelState.AddModelError(
                    nameof(Instructor.InstructorID),
                    "This Instructor ID already exists.");

                return View(instructor);
            }

            bool duplicateEmail = await _context.Instructors
                .AnyAsync(i =>
                    i.Id != id &&
                    i.Email.ToLower() == instructor.Email.ToLower());

            if (duplicateEmail)
            {
                ModelState.AddModelError(
                    nameof(Instructor.Email),
                    "This email is already registered.");

                return View(instructor);
            }

            existing.InstructorID = instructor.InstructorID;
            existing.FirstName = instructor.FirstName;
            existing.LastName = instructor.LastName;
            existing.Age = instructor.Age;
            existing.Address = instructor.Address;
            existing.Designation = instructor.Designation;
            existing.Gender = instructor.Gender;
            existing.ContactNumber = instructor.ContactNumber;
            existing.Majorship = instructor.Majorship;
            existing.BachelorDegree = instructor.BachelorDegree;
            existing.Email = instructor.Email;

            // PasswordHash is intentionally preserved.

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Instructor updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // DETAILS
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var instructor = await _context.Instructors
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id);

            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // ==========================================
        // DELETE - GET
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var instructor = await _context.Instructors
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id);

            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // ==========================================
        // DELETE - POST
        // ==========================================

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instructor = await _context.Instructors.FindAsync(id);

            if (instructor == null)
            {
                return NotFound();
            }

            _context.Instructors.Remove(instructor);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Instructor deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}
