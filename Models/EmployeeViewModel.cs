using SchoolManagementSystem.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Pending User is required")]
        [Display(Name = "Pending User")]
        public required string UserId { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is required")]
        [MaxLength(50)]
        public required string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is required")]
        [MaxLength(50)]
        public required string LastName { get; set; }

        [Display(Name = "Department")]
        [Required(ErrorMessage = "Department is required")]
        public Department Department { get; set; }  // Enum for departments

        [Display(Name = "Academic Degree")]
        public AcademicDegree AcademicDegree { get; set; }

        [Display(Name = "Hire Date")]
        public DateTime? HireDate { get; set; }

        [Display(Name = "Status")]
        public IEnumerable<User>? PendingUsers { get; set; } // List of pending users
        public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;

        [Phone]
        [Display(Name = "Phone Number")]
        public required string PhoneNumber { get; set; }

        // Field for image upload (not stored in the database)
        [Display(Name = "Image")]
        public IFormFile? ImageFile { get; set; }

        // ID of the stored image (stored in the database)
        public Guid ImageId { get; set; }

        // Full path of the image
        public string ImageFullPath => ImageId == Guid.Empty
            ? "https://devartacademyfiles.blob.core.windows.net/images/noimage.png"
            : $"https://devartacademyfiles.blob.core.windows.net/employees/{ImageId}";
    }
}
