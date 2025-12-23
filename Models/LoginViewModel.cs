using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models
{
    public class LoginViewModel
    {
        // Username (email) is required
        [Required(ErrorMessage = "Username (email) is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public required string Username { get; set; }

        // Password is required and must be at least 6 characters long
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least {1} characters long.")]
        public required string Password { get; set; }

        // Option to remember the user, so they don't have to log in again
        public bool RememberMe { get; set; }
    }
}
