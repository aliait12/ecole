using SchoolManagementSystem.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Repositories
{
    public interface IAttendanceRepository : IGenericRepository<Attendance>
    {
        Task<List<Attendance>> GetAttendancesByStudentIdAsync(int studentId);
        Task<Attendance> GetAttendanceWithDetailsByIdAsync(int id);
        Task<List<Attendance>> GetAttendancesByStudentIdAndSubjectIdAsync(int studentId, int subjectId);

        Task AddAttendanceAsync(Attendance attendance);

        Task<List<Subject>> GetSubjectsByStudentIdAsync(int studentId);

    }
}
