using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models
{
    public class RegisterNewUserViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public required string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public required string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public required string Username { get; set; }

        [MaxLength(100, ErrorMessage = "The field {0} can only contain {1} characters.")]
        public string? Address { get; set; }

        [MaxLength(20, ErrorMessage = "The field {0} can only contain {1} characters.")]
        [RegularExpression(@"^\+?[0-9\s\-()]*$", ErrorMessage = "Invalid phone number.")]
        public string? PhoneNumber { get; set; }
    }
}
