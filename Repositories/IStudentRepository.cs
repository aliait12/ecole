using SchoolManagementSystem.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Repositories
{
    public interface IStudentRepository : IGenericRepository<Student>
    {
        // Method to get students by class ID
        Task<IEnumerable<Student>> GetStudentsByClassIdAsync(int classId);

        // Method to get students by status
        Task<IEnumerable<Student>> GetStudentsByStatusAsync(string status);

        // Method to get a student with their associated courses
        Task<Student> GetStudentWithCoursesAsync(int studentId);

        // Method to get a student by full name
        Task<Student> GetByFullNameAsync(string fullName);

        // Method to get all students with related entities
        Task<IEnumerable<Student>> GetAllWithIncludesAsync();

        // Method to get students by school class ID
        Task<List<Student>> GetStudentsBySchoolClassIdAsync(int schoolClassId);

        Task<int?> GetStudentIdByUserIdAsync(string userId);

        Task<Student> GetStudentByUserIdAsync(string userId);

    }
}
