using SchoolManagementSystem.Data.Entities;
using SchoolManagementSystem.Repositories;

public interface IGradeRepository : IGenericRepository<Grade>
{
    Task<List<Grade>> GetGradesByStudentIdAsync(int studentId);
    Task<List<Grade>> GetAllGradesAsync();

    Task<List<Subject>> GetSubjectsByStudentIdAsync(int studentId);

    Task<Grade> GetGradeWithDetailsByIdAsync(int id);


}
