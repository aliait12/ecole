using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data.Entities;
using SchoolManagementSystem.Data;

namespace SchoolManagementSystem.Repositories
{
    public class StudentRepository : GenericRepository<Student>, IStudentRepository
    {
        private readonly SchoolDbContext _context;

        public StudentRepository(SchoolDbContext context) : base(context)
        {
            _context = context;
        }

        // Method to retrieve all students with related entities
        public async Task<IEnumerable<Student>> GetAllWithIncludesAsync()
        {
            return await _context.Students
                .Include(s => s.User)             // Include the User entity
                .Include(s => s.SchoolClass)      // Include the SchoolClass entity
                .AsNoTracking()                   // Do not track the entities
                .ToListAsync();
        }

        // Method to retrieve a student by full name
        public async Task<Student> GetByFullNameAsync(string fullName)
        {
            return await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => $"{s.User.FirstName} {s.User.LastName}" == fullName);
        }

        // Method to retrieve students by class ID
        public async Task<IEnumerable<Student>> GetStudentsByClassIdAsync(int classId)
        {
            return await _context.Students
                .Where(s => s.SchoolClassId == classId)
                .Include(s => s.SchoolClass) // Include the class
                .Include(s => s.User)        // Include the user
                .ToListAsync();
        }

        // Method to retrieve students by status
        public async Task<IEnumerable<Student>> GetStudentsByStatusAsync(string status)
        {
            // Convert the status string to the enum StudentStatus
            if (Enum.TryParse<StudentStatus>(status, out var studentStatus))
            {
                return await _context.Students
                    .Where(s => s.Status == studentStatus)  // Comparison using enum
                    .Include(s => s.SchoolClass)            // Include the class
                    .Include(s => s.User)                   // Include the user
                    .ToListAsync();
            }
            else
            {
                // If the provided status is not valid, return an empty list or throw an exception
                return new List<Student>();
            }
        }

        // Method to retrieve a student with courses
        public async Task<Student> GetStudentWithCoursesAsync(int studentId)
        {
            return await _context.Students
                .Include(s => s.SchoolClass)
                .ThenInclude(c => c.Course)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == studentId);
        }

        public async Task<List<Student>> GetStudentsBySchoolClassIdAsync(int schoolClassId)
        {
            return await _context.Students
                .Include(s => s.Grades) 
                .Where(s => s.SchoolClassId == schoolClassId)
                .ToListAsync();
        }

        public async Task<int?> GetStudentIdByUserIdAsync(string userId)
        {
            
            var student = await _context.Students
                .Include(s => s.User) 
                .FirstOrDefaultAsync(s => s.UserId == userId); 

            return student?.Id; 
        }

        public async Task<Student> GetStudentByUserIdAsync(string userId)
        {
            return await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }

    }
}
