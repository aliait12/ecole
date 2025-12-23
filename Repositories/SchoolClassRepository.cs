using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Data.Entities;

namespace SchoolManagementSystem.Repositories
{
    public class SchoolClassRepository : GenericRepository<SchoolClass>, ISchoolClassRepository
    {
        private readonly SchoolDbContext _context;

        public SchoolClassRepository(SchoolDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<SchoolClass>> GetAvailableSchoolClassesAsync()
        {
            return await _context.SchoolClasses
                .Where(sc => sc.CourseId == null) // Only classes without an associated course
                .ToListAsync();
        }

        // Method to get all classes
        public async Task<List<SchoolClass>> GetAllAsync()
        {
            return await _context.SchoolClasses
                .Include(c => c.Students) // Include students for additional information if needed
                .ToListAsync();
        }

        public async Task<List<SchoolClass>> GetSchoolClassesByIdsAsync(List<int> ids)
        {
            return await _context.SchoolClasses
                .Where(sc => ids.Contains(sc.Id))
                .ToListAsync();
        }

        public async Task<List<SchoolClass>> GetAllWithDetailsAsync()
        {
            return await _context.SchoolClasses
                .Include(sc => sc.Course)  
                .ThenInclude(c => c.CourseSubjects)  
                .ThenInclude(cs => cs.Subject)  
                .ToListAsync();
        }



    }

}
