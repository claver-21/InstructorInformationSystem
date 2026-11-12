
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace InstructorInformationSystem.Models
{
    public class Instructor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Instructor ID is required.")]
        [Display(Name = "Instructor ID")]
        public string InstructorID { get; set; } = "";

        [Required(ErrorMessage = "First name is required.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Last name is required.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = "";

        [NotMapped]
        [Display(Name = "Full Name")]
        public string FullName =>
            $"{FirstName} {LastName}".Trim();

        [Required(ErrorMessage = "Age is required.")]
        [Range(18, 100, ErrorMessage = "Age must be between 18 and 100.")]
        public int? Age { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; } = "";

        [Required(ErrorMessage = "Designation is required.")]
        public string Designation { get; set; } = "";

        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; } = "";

        [Required(ErrorMessage = "Contact number is required.")]
        [RegularExpression(
            @"^09\d{9}$",
            ErrorMessage = "Enter an 11-digit number starting with 09."
        )]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; } = "";

        [Required(ErrorMessage = "Majorship is required.")]
        public string Majorship { get; set; } = "";

        [Required(ErrorMessage = "Bachelor degree is required.")]
        [Display(Name = "Bachelor Degree")]
        public string BachelorDegree { get; set; } = "";

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [Display(Name = "E-mail Address")]
        public string Email { get; set; } = "";

        // Saved in SQLite. Never store the plain password.
        [BindNever]
        public string? PasswordHash { get; set; }

        // Used by the registration form only.
        [NotMapped]
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Use at least 8 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        // Used by the registration form only.
        [NotMapped]
        [Required(ErrorMessage = "Please confirm your password.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = "";
    }
}
