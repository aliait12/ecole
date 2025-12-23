using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Data.Entities
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "The field {0} is required.")]
        [MaxLength(50, ErrorMessage = "The field {0} can only contain {1} characters.")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "The field {0} is required.")]
        [MaxLength(50, ErrorMessage = "The field {0} can only contain {1} characters.")]
        public required string LastName { get; set; }

        [MaxLength(100, ErrorMessage = "The field {0} can only contain {1} characters.")]
        public string? Address { get; set; }

        [Display(Name = "Profile Picture")]
        public Guid? ProfilePictureId { get; set; }

        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";

        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Last Login")]
        public DateTime? LastLogin { get; set; }
    }
}
