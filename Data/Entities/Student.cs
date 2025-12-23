using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Data.Entities
{
    public class Student : IEntity
    {
        public int Id { get; set; }

        public required string UserId { get; set; }
        public User User { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is required")]
        [MaxLength(50)]
        public required string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is required")]
        [MaxLength(50)]
        public required string LastName { get; set; }

        [Display(Name = "Enrollment Date")]
        public DateTime? EnrollmentDate { get; set; }
        
        public string FormattedEnrollmentDate => EnrollmentDate?.ToString("dd/MM/yyyy");


        [Display(Name = "Status")]
        public StudentStatus Status { get; set; } = StudentStatus.Pending;

        public int? SchoolClassId { get; set; } 
        public SchoolClass? SchoolClass { get; set; }

        public ICollection<Grade> Grades { get; set; } = new List<Grade>();

        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();


        [Display(Name = "Image")]
        public Guid ImageId { get; set; }

        public string ImageFullPath => ImageId == Guid.Empty
           ? $"https://devartacademyfiles.blob.core.windows.net/images/student.png"
            : $"https://devartacademyfiles.blob.core.windows.net/students/{ImageId}";
    }

    // Enum to standardize student status
    public enum StudentStatus
    {
        Pending,
        Active,
        Inactive
    }
}
