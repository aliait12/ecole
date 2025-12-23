using SchoolManagementSystem.Data.Entities;
using SchoolManagementSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace SchoolManagementSystem.Repositories
{
    public class TeacherRepository : GenericRepository<Teacher>, ITeacherRepository
    {
        private readonly SchoolDbContext _context;

        public TeacherRepository(SchoolDbContext context) : base(context)
        {
            _context = context;
        }

        // Retrieves all teachers along with their associated subjects
        public async Task<IEnumerable<Teacher>> GetAllTeachersWithSubjectsAsync()
        {
            return await _context.Teachers
                .Include(t => t.TeacherSubjects)
                    .ThenInclude(ts => ts.Subject) // Includes the Subject entity through the junction entity
                .ToListAsync();
        }

        // Retrieves teachers that teach a specific subject
        public async Task<IEnumerable<Teacher>> GetTeachersByDisciplineAsync(int subjectId)
        {
            return await _context.Teachers
                .Where(t => t.TeacherSubjects.Any(ts => ts.SubjectId == subjectId))
                .ToListAsync();
        }

        // Retrieves a teacher by full name
        public async Task<Teacher> GetTeacherByFullNameAsync(string fullName)
        {
            var names = fullName.Split(' '); // Assuming the full name is provided as "FirstName LastName"
            return await _context.Teachers
                .FirstOrDefaultAsync(t => t.FirstName == names[0] && t.LastName == names[1]);
        }

        // Retrieves a specific teacher along with their subjects
        public async Task<Teacher> GetTeacherWithSubjectsAsync(int teacherId)
        {
            return await _context.Teachers
                .Include(t => t.TeacherSubjects)
                    .ThenInclude(ts => ts.Subject) // Includes the Subject entity through the junction entity
                .FirstOrDefaultAsync(t => t.Id == teacherId);
        }

        // Updates a teacher's subjects
        public async Task UpdateTeacherSubjectsAsync(int teacherId, IEnumerable<int> subjectIds)
        {
            var teacher = await _context.Teachers
                .Include(t => t.TeacherSubjects)
                .FirstOrDefaultAsync(t => t.Id == teacherId);

            if (teacher != null)
            {
                // Clears current subjects
                teacher.TeacherSubjects.Clear();

                // Adds new subjects
                foreach (var subjectId in subjectIds)
                {
                    teacher.TeacherSubjects.Add(new TeacherSubject { TeacherId = teacherId, SubjectId = subjectId });
                }

                await _context.SaveChangesAsync();
            }
        }

        // Updates a teacher's classes
        public async Task UpdateTeacherClassesAsync(int teacherId, IEnumerable<int> schoolClassIds)
        {
            var teacher = await _context.Teachers
                .Include(t => t.TeacherSchoolClasses)
                .FirstOrDefaultAsync(t => t.Id == teacherId);

            if (teacher != null)
            {
                // Clears current classes
                teacher.TeacherSchoolClasses.Clear();

                // Adds new classes
                foreach (var schoolClassId in schoolClassIds)
                {
                    teacher.TeacherSchoolClasses.Add(new TeacherSchoolClass { TeacherId = teacherId, SchoolClassId = schoolClassId });
                }

                await _context.SaveChangesAsync();
            }
        }

        // Retrieves teachers by status
        public async Task<IEnumerable<Teacher>> GetTeachersByStatusAsync(TeacherStatus status)
        {
            return await _context.Teachers
                .Where(t => t.Status == status)
                .ToListAsync();
        }

        // Counts teachers by subject
        public async Task<int> CountTeachersByDisciplineAsync(int subjectId)
        {
            return await _context.Teachers
                .CountAsync(t => t.TeacherSubjects.Any(ts => ts.SubjectId == subjectId));
        }

        // Method that includes subjects and classes
        public async Task<IEnumerable<Teacher>> GetAllWithIncludesAsync()
        {
            return await _context.Teachers
                .Include(t => t.TeacherSubjects) // Includes associated subjects
                    .ThenInclude(ts => ts.Subject) // Includes subject entities
                .Include(t => t.TeacherSchoolClasses) // Includes the junction table for classes
                    .ThenInclude(tsc => tsc.SchoolClass) // Includes associated classes
                .ToListAsync();
        }

        // Method to get all teachers
        public async Task<IEnumerable<Teacher>> GetAllAsync()
        {
            return await _context.Teachers.ToListAsync();
        }

        public async Task<Teacher> GetTeacherWithDetailsAsync(int teacherId)
        {
            return await _context.Teachers
                .Include(t => t.TeacherSchoolClasses)
                    .ThenInclude(tsc => tsc.SchoolClass)
                .Include(t => t.TeacherSubjects)
                    .ThenInclude(ts => ts.Subject)
                .FirstOrDefaultAsync(t => t.Id == teacherId);
        }

        public async Task<Teacher> GetTeacherByUserIdAsync(string userId)
        {
            return await _context.Teachers
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.UserId == userId);
        }

    }
}
