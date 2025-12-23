using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models
{
    public class RecoverPasswordViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public required string Email { get; set; }
    }
}
