namespace SchoolManagementSystem.Data.Entities
{
    public class TeacherSchoolClass
    {
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }

        public int SchoolClassId { get; set; }
        public SchoolClass SchoolClass { get; set; }
    }
}
