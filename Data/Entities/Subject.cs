using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Data.Entities
{
    public class Subject : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        public string? Description { get; set; } 

        public int Credits { get; set; }

        public int TotalClasses { get; set; }

        // Collection of teachers associated with this subject
        public ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();

        // Collection of courses associated with this discipline
        public ICollection<CourseSubject> CourseSubjects { get; set; } = new List<CourseSubject>();

    }
}
