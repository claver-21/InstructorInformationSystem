using System.ComponentModel.DataAnnotations;

namespace InstructionInformationSystem.Models
{
    public class Login
    {

        [Required(ErrorMessage = "Invalid Userame")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password not match.")]
        [Display(Name = "Password")]
        public string Password { get; set; }

    }
}
