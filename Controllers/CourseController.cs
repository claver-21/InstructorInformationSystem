
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InstructorInformationSystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace InstructorInformationSystem.Controllers
{
    [Authorize]
    public class CourseController : Controller
    {
        private static List<Course> _courses = new List<Course>
        {
            new Course
            {
                Id = 1,
                CourseCode = "IT314",
                CourseName = "System Integration and Architecture",
                Units = 3,
                Schedule = "MWF 9:00 AM"
            },
            new Course
            {
                Id = 2,
                CourseCode = "IT315",
                CourseName = "Web Development",
                Units = 3,
                Schedule = "TTH 1:00 PM"
            }
        };

        public static IReadOnlyList<Course> GetDashboardCourses()
        {
            return _courses.ToList();
        }

        // ==========================================
        // COURSE LIST
        // ==========================================

        [HttpGet]
        public IActionResult Index()
        {
            return View(_courses);
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
        public IActionResult Create(Course course)
        {
            if (!ModelState.IsValid)
            {
                return View(course);
            }

            course.Id = _courses.Count > 0
                ? _courses.Max(c => c.Id) + 1
                : 1;

            _courses.Add(course);

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // EDIT - GET
        // ==========================================

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var course = _courses.FirstOrDefault(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // ==========================================
        // EDIT - POST
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Course course)
        {
            if (!ModelState.IsValid)
            {
                return View(course);
            }

            var existingCourse =
                _courses.FirstOrDefault(c => c.Id == course.Id);

            if (existingCourse == null)
            {
                return NotFound();
            }

            existingCourse.CourseCode = course.CourseCode;
            existingCourse.CourseName = course.CourseName;
            existingCourse.Units = course.Units;
            existingCourse.Schedule = course.Schedule;

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // DETAILS
        // ==========================================

        [HttpGet]
        public IActionResult Details(int id)
        {
            var course = _courses.FirstOrDefault(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // ==========================================
        // DELETE - GET
        // ==========================================

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var course = _courses.FirstOrDefault(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // ==========================================
        // DELETE - POST
        // ==========================================

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var course = _courses.FirstOrDefault(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            _courses.Remove(course);

            return RedirectToAction(nameof(Index));
        }
    }
}
