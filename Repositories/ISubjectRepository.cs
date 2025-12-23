using SchoolManagementSystem.Data.Entities;

namespace SchoolManagementSystem.Repositories
{
    public interface ISubjectRepository : IGenericRepository<Subject>
    {
        Task<List<Subject>> GetAllSubjectsAsync();

        Task<List<Subject>> GetSubjectsByIdsAsync(List<int> ids);

        Task<List<Subject>> GetSubjectsByStudentIdAsync(int studentId);

    }
}
