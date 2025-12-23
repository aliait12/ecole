using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data.Entities;
using SchoolManagementSystem.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolManagementSystem.Data;

public class AttendanceRepository : GenericRepository<Attendance>, IAttendanceRepository
{
    private readonly SchoolDbContext _context;

    public AttendanceRepository(SchoolDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<Attendance>> GetAttendancesByStudentIdAsync(int studentId)
    {
        return await _context.Attendances
            .Include(a => a.Subject)
            .Include(a => a.Student)
            .Where(a => a.StudentId == studentId)
            .ToListAsync();
    }

    public async Task<Attendance> GetAttendanceWithDetailsByIdAsync(int id)
    {
        return await _context.Attendances
            .Include(a => a.Student)
            .Include(a => a.Subject)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<Attendance>> GetAttendancesByStudentIdAndSubjectIdAsync(int studentId, int subjectId)
    {
        return await _context.Attendances
            .Include(a => a.Subject)
            .Include(a => a.Student)
            .Where(a => a.StudentId == studentId && a.SubjectId == subjectId)
            .ToListAsync();
    }

    public async Task AddAttendanceAsync(Attendance attendance)
    {
        await _context.Attendances.AddAsync(attendance);
        await _context.SaveChangesAsync();
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
}
