using SchoolManagementSystem.Data.Entities;

namespace SchoolManagementSystem.Repositories
{
    public interface ITeacherRepository : IGenericRepository<Teacher>
    {
        // Method to retrieve all teachers with their associated subjects
        Task<IEnumerable<Teacher>> GetAllTeachersWithSubjectsAsync();

        // Method to retrieve teachers by subject ID
        Task<IEnumerable<Teacher>> GetTeachersByDisciplineAsync(int subjectId);

        // Method to retrieve a teacher by their full name
        Task<Teacher> GetTeacherByFullNameAsync(string fullName);

        // Method to retrieve a specific teacher with their associated subjects
        Task<Teacher> GetTeacherWithSubjectsAsync(int teacherId);

        // Method to update the subjects of a teacher
        Task UpdateTeacherSubjectsAsync(int teacherId, IEnumerable<int> subjectIds);

        // Method to retrieve teachers by their status
        Task<IEnumerable<Teacher>> GetTeachersByStatusAsync(TeacherStatus status);

        // Method to count the number of teachers by subject
        Task<int> CountTeachersByDisciplineAsync(int subjectId);

        // Method to retrieve all teachers including their subjects and classes
        Task<IEnumerable<Teacher>> GetAllWithIncludesAsync();

        // Method to retrieve all teachers
        Task<IEnumerable<Teacher>> GetAllAsync();

        // Method to retrieve a specific teacher with detailed information
        Task<Teacher> GetTeacherWithDetailsAsync(int teacherId);

        // Method to update the classes assigned to a teacher
        Task UpdateTeacherClassesAsync(int teacherId, IEnumerable<int> subjectIds);

        Task<Teacher> GetTeacherByUserIdAsync(string userId);

    }
}
