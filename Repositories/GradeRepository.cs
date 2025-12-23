using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data.Entities;
using SchoolManagementSystem.Repositories;
using SchoolManagementSystem.Data;

public class GradeRepository : GenericRepository<Grade>, IGradeRepository
{
    private readonly SchoolDbContext _context;

    public GradeRepository(SchoolDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<Grade>> GetGradesByStudentIdAsync(int studentId)
    {
        return await _context.Grades
            .Include(g => g.Subject)
            .Include(g => g.Student)
            .Where(g => g.StudentId == studentId)
            .ToListAsync();
    }

    public async Task<List<Grade>> GetAllGradesAsync()
    {
        return await _context.Grades
            .Include(g => g.Subject)
            .Include(g => g.Student)
            .ToListAsync();
    }

    public async Task<List<Subject>> GetSubjectsByStudentIdAsync(int studentId)
    {
        // Get the student with the associated class and course
        var student = await _context.Students
            .Include(s => s.SchoolClass)
            .ThenInclude(sc => sc.Course)
            .ThenInclude(c => c.CourseSubjects)
            .ThenInclude(cs => cs.Subject)
            .FirstOrDefaultAsync(s => s.Id == studentId);

        if (student == null || student.SchoolClass == null || student.SchoolClass.Course == null)
        {
            return new List<Subject>(); // Return an empty list if no associations are found
        }

        // Return the list of subjects associated with the course of the student's class
        return student.SchoolClass.Course.CourseSubjects
                     .Select(cs => cs.Subject)
                     .ToList();
    }

    public async Task<Grade> GetGradeWithDetailsByIdAsync(int id)
    {
        return await _context.Grades
            .Include(g => g.Student)  // Include the Student entity
            .Include(g => g.Subject)  // Include the Subject entity
            .FirstOrDefaultAsync(g => g.Id == id);
    }
}
