using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Data.Entities
{
    public class Attendance : IEntity
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int SubjectId { get; set; }
        public Subject Subject { get; set; }

        public string? Description { get; set; }

        [Display(Name = "Attendance Date")]
        public DateTime Date { get; set; } = DateTime.Now;

        public string Status => string.IsNullOrEmpty(Description) ? "Absent" : "Present";

    }
}
