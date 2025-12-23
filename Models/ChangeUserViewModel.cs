using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models
{
    public class ChangeUserViewModel
    {
        // FirstName is required
        [Required(ErrorMessage = "First Name is required.")]
        [Display(Name = "First Name")]
        public required string FirstName { get; set; }

        // LastName is required
        [Required(ErrorMessage = "Last Name is required.")]
        [Display(Name = "Last Name")]
        public required string LastName { get; set; }

        // Address with a max length of 100 characters
        [MaxLength(100, ErrorMessage = "The field {0} can only contain {1} characters.")]
        public string? Address { get; set; }

        // PhoneNumber with a max length of 20 characters and allows only digits and valid phone characters
        [MaxLength(20, ErrorMessage = "The field {0} can only contain {1} characters.")]
        [RegularExpression(@"^\+?[0-9\s\-()]*$", ErrorMessage = "The field {0} must contain only numbers and valid phone characters.")]
        public string? PhoneNumber { get; set; }
    }
}
