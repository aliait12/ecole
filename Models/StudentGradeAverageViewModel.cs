using SchoolManagementSystem.Data.Entities;
using System.Linq;

namespace SchoolManagementSystem.Models
{
    public class StudentGradeAverageViewModel
    {
        public required Student Student { get; set; } // Includes the Student entity

        // Property that calculates the student's average grade
        public double AverageGrade => Student?.Grades != null && Student.Grades.Any()
            ? Student.Grades.Average(g => g.Value)
            : 0;

        // Property that determines the student's status based on the average grade
        public string Status => AverageGrade >= 9.5 ? "Passed" : "Failed";
    }
}
