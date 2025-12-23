using Microsoft.AspNetCore.Mvc.ModelBinding;
using SchoolManagementSystem.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SchoolManagementSystem.Models
{
    public class TeacherViewModel
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

        [Display(Name = "Academic Degree")]
        public AcademicDegree AcademicDegree { get; set; }

        public DateTime? HireDate { get; set; }

        public string FormattedHireDate => HireDate?.ToString("dd/MM/yyyy");

        // Collection of associated SchoolClass IDs
        public ICollection<int> SchoolClassIds { get; set; } = new List<int>();

        // Collection of associated Subject IDs
        public ICollection<int> SubjectIds { get; set; } = new List<int>();

        // ID for the teacher's profile picture
        public Guid ImageId { get; set; }

        [Display(Name = "Image")]
        public IFormFile? ImageFile { get; set; }

        public string ImageFullPath => ImageId == Guid.Empty
            ? "https://devartacademyfiles.blob.core.windows.net/images/noimage.png"
            : $"https://devartacademyfiles.blob.core.windows.net/teachers/{ImageId}";

        // Teacher's status
        public TeacherStatus Status { get; set; } = TeacherStatus.Active;

        // Property for pending users
        public IEnumerable<User>? PendingUsers { get; set; }

        // Property for associated SchoolClasses
        public IEnumerable<SchoolClass>? SchoolClasses { get; set; }

        // Property for associated Subjects
        public IEnumerable<Subject>? Subjects { get; set; }
    }
}
