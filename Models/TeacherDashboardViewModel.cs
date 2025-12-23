namespace SchoolManagementSystem.Models
{
    public class TeacherDashboardViewModel
    {
        public required string TeacherName { get; set; }

        public int TotalClasses { get; set; }
        public int TotalSubjects { get; set; }
        public int TotalStudents { get; set; }

        public List<ClassCard> Classes { get; set; } = new();
        public List<SimpleItem> Subjects { get; set; } = new();
    }

    public class ClassCard
    {
        public int Id { get; set; }
        public required string ClassName { get; set; }
        public required string CourseName { get; set; }
        public int StudentsCount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class SimpleItem
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}
