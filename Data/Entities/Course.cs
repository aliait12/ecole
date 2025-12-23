using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Data.Entities
{
    public class Course : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Range(1, 52)]
        public int Duration { get; set; } // Duration in weeks, for example.

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // One-to-many relationship with SchoolClass
        public ICollection<SchoolClass> SchoolClasses { get; set; } = new List<SchoolClass>();

        // Many-to-many relationship through the CourseSubject entity
        public ICollection<CourseSubject> CourseSubjects { get; set; } = new List<CourseSubject>();
    }
}
