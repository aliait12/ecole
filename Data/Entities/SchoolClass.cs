using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Data.Entities
{
    public class SchoolClass : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public required string ClassName { get; set; }

        public int? CourseId { get; set; } // Foreign key for associated Course
        public Course? Course { get; set; } // Navigation property for Course

        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        // Collection of students associated with this class
        public ICollection<Student> Students { get; set; } = new List<Student>();

        // Collection of teachers associated with this class
        public ICollection<TeacherSchoolClass> TeacherSchoolClasses { get; set; } = new List<TeacherSchoolClass>();
    }
}
