using SchoolManagementSystem.Data.Entities;
using SchoolManagementSystem.Models;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Repositories
{
    public interface ICourseRepository : IGenericRepository<Course>
    {
        Task<Course> GetCourseWithDetailsAsync(int id);
        Task<List<Course>> GetAllWithDetailsAsync();

        Task<List<CourseViewModel>> GetAllCourseViewModelsAsync();
        Task<CourseViewModel> GetCourseDetailsViewModelAsync(int id);

        Task<List<Course>> GetCoursesByIdsAsync(List<int> ids);


    }
}
