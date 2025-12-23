using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models
{
    public class ChangePasswordViewModel
    {
        // Current password is required
        [Required]
        [Display(Name = "Current password")]
        public required string OldPassword { get; set; }

        // New password is required
        [Required]
        [Display(Name = "New password")]
        public required string NewPassword { get; set; }

        // Confirmation for the new password
        [Required]
        // Validates that the confirmation matches the new password
        [Compare("NewPassword", ErrorMessage = "The confirmation password does not match the new password.")]
        public required string Confirm { get; set; }
    }
}
