using System.ComponentModel.DataAnnotations;

namespace InstructorInformationSystem.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Course Code is required.")]
        [Display(Name = "Course Code")]
        public string CourseCode { get; set; }

        [Required(ErrorMessage = "Course Name is required.")]
        [Display(Name = "Course Name")]
        public string CourseName { get; set; }

        [Required(ErrorMessage = "Number of units is required.")]
        [Range(1, 10, ErrorMessage = "Units must be between 1 and 10.")]
        public int Units { get; set; }

        [Required(ErrorMessage = "Schedule is required.")]
        public string Schedule { get; set; }
    }
}