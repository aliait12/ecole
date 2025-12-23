using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models
{
    public class GradeViewModel
    {
        public int Id { get; set; }

        [Required]
        [Range(0, 20, ErrorMessage = "The grade must be between 0 and 20.")]
        public double Value { get; set; }

        [Required]
        public int StudentId { get; set; }
        public required string StudentName { get; set; }

        [Required]
        public int SubjectId { get; set; }
        public required string SubjectName { get; set; }

        [Display(Name = "Evaluation Date")]
        public DateTime EvaluationDate { get; set; } = DateTime.Now;

        // Calculated property to determine the pass/fail status
        public string Status => Value >= 9.5 ? "Passed" : "Failed";
    }
}
