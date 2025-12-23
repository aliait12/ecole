namespace SchoolManagementSystem.Models
{
    public class HomeViewModel
    {
        public required IEnumerable<CourseViewModel> Courses { get; set; }
        public required IEnumerable<SchoolClassViewModel> SchoolClasses { get; set; } 

    }
}
