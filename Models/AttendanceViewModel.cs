using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models
{
    public class AttendanceViewModel
    {
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }
        public required string StudentName { get; set; }

        [Required]
        public int SubjectId { get; set; }
        public required string SubjectName { get; set; }

        public string? Description { get; set; }

        [Display(Name = "Attendance Date")]
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
