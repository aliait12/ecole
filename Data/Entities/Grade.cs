using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Data.Entities
{
    public class Grade : IEntity
    {
        public int Id { get; set; }

        [Required]
        [Range(0, 20, ErrorMessage = "The grade must be between 0 and 20.")]
        public double Value { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int SubjectId { get; set; }
        public Subject Subject { get; set; }

        [Display(Name = "Evaluation Date")]
        public DateTime EvaluationDate { get; set; } = DateTime.Now;

        // Calculated property to determine pass/fail status
        public string Status => Value >= 9.5 ? "Passed" : "Failed";
    }
}
