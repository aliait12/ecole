using Microsoft.AspNetCore.Mvc.ModelBinding;
using SchoolManagementSystem.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models
{
    public class StudentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Pending User is required")]
        [Display(Name = "Pending User")]
        public required string UserId { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [Display(Name = "First Name")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [Display(Name = "Last Name")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "School Class is required")]
        [Display(Name = "School Class")]
        public int? SchoolClassId { get; set; }

        public Guid ImageId { get; set; }

        [Display(Name = "Image")]
        public IFormFile? ImageFile { get; set; }

        public DateTime? EnrollmentDate { get; set; }

        public StudentStatus Status { get; set; }

        public IEnumerable<User>? PendingUsers { get; set; } 

        public string ImageFullPath => ImageId == Guid.Empty
    ? "https://devartacademyfiles.blob.core.windows.net/images/student.png"
    : $"https://devartacademyfiles.blob.core.windows.net/students/{ImageId}";

        public SchoolClass? SchoolClass { get; set; } 
    }
}
