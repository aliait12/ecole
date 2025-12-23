using SchoolManagementSystem.Data.Entities;

namespace SchoolManagementSystem.Models
{
    public class StudentSubjectGradeViewModel
    {
        public required Subject Subject { get; set; }
        public required Grade Grade { get; set; }
        public int StudentId { get; set; }
        public required string StudentName { get; set; } 

    }
}
