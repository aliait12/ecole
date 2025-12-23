using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SchoolManagementSystem.Data.Entities
{
    public class Teacher : IEntity
    {
        // Primary key of the Teacher table
        public int Id { get; set; }

        public string UserId { get; set; }

        // Navigation property for the associated User entity
        public User User { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is required")]
        [MaxLength(50)]
        public required string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is required")]
        [MaxLength(50)]
        public required string LastName { get; set; }

        // Teacher's academic degree (e.g. Bachelor's, Master's)
        [Display(Name = "Academic Degree")]
        public AcademicDegree AcademicDegree { get; set; }

        // Teacher's hire date, required field
        [Display(Name = "Hire Date")]
        public DateTime? HireDate { get; set; }

        // String formatted to display the hire date
        public string FormattedHireDate => HireDate?.ToString("dd/MM/yyyy");

        // Teacher status (Active, Inactive, Pending)
        [Display(Name = "Status")]
        public TeacherStatus Status { get; set; } = TeacherStatus.Active;

        // Collection of classes associated with this teacher through the join table
        public ICollection<TeacherSchoolClass> TeacherSchoolClasses { get; set; } = new List<TeacherSchoolClass>();

        // Collection of subjects associated with this teacher through the junction entity
        public ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();

        // Image ID for the teacher's profile picture
        [Display(Name = "Image")]
        public Guid ImageId { get; set; }

        // Full URL path to the teacher's profile image
        public string ImageFullPath => ImageId == Guid.Empty
            ? "https://devartacademyfiles.blob.core.windows.net/images/noimage.png"
            : $"https://devartacademyfiles.blob.core.windows.net/teachers/{ImageId}";
    }

    // Enum to standardize teacher status
    public enum TeacherStatus
    {
        Pending,
        Active,
        Inactive
    }
}
