using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Data.Entities;

namespace SchoolManagementSystem.Repositories
{
    public class SubjectRepository : GenericRepository<Subject>, ISubjectRepository
    {
        private readonly SchoolDbContext _context;

        public SubjectRepository(SchoolDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Subject>> GetAllSubjectsAsync()
        {
            return await _context.Subjects.ToListAsync();
        }

        public async Task<List<Subject>> GetSubjectsByIdsAsync(List<int> ids)
        {
            return await _context.Subjects
                .Where(s => ids.Contains(s.Id))
                .ToListAsync();
        }

        // Method to obtain subjects associated with a specific student's class
        public async Task<List<Subject>> GetSubjectsByStudentIdAsync(int studentId)
        {
            // Get the student's class (SchoolClassId)
            var schoolClassId = await _context.Students
                .Where(s => s.Id == studentId)
                .Select(s => s.SchoolClassId)
                .FirstOrDefaultAsync();

            if (schoolClassId == null)
                return new List<Subject>(); // Returns empty list if the student does not have an associated class

            // Get the subjects associated with the student's class
            return await _context.CourseSubjects
                .Where(cs => cs.Course.SchoolClasses.Any(sc => sc.Id == schoolClassId))
                .Select(cs => cs.Subject)
                .ToListAsync();
        }
    }

}
