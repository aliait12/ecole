using SchoolManagementSystem.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Repositories
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(Department department);
        Task<IEnumerable<Employee>> GetEmployeesByStatusAsync(EmployeeStatus status);
        Task<IEnumerable<Employee>> GetRecentlyHiredEmployeesAsync();
        Task<IEnumerable<Employee>> GetAdministrativeEmployeesAsync();
        Task<bool> CanEmployeeManageUserCreationAsync(int employeeId);
        Task<int> CountEmployeesByDepartmentAsync(Department department);
        Task<IEnumerable<Employee>> GetEmployeesWithCompleteProfileAsync();
        Task<Employee> GetEmployeeByUserIdAsync(string userId);
        Task<IEnumerable<Employee>> GetAllWithIncludesAsync();

    }
}
