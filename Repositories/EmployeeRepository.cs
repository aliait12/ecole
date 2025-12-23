using SchoolManagementSystem.Data;
using SchoolManagementSystem.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SchoolManagementSystem.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        private readonly SchoolDbContext _context;

        public EmployeeRepository(SchoolDbContext context) : base(context)
        {
            _context = context;
        }

        // Gets employees by department
        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(Department department)
        {
            return await _context.Employees
                .Where(e => e.Department == department)
                .ToListAsync();
        }

        // Gets employees by status (Active, Inactive, Pending)
        public async Task<IEnumerable<Employee>> GetEmployeesByStatusAsync(EmployeeStatus status)
        {
            return await _context.Employees
                .Where(e => e.Status == status)
                .ToListAsync();
        }

        // Gets employees hired in the last 30 days
        public async Task<IEnumerable<Employee>> GetRecentlyHiredEmployeesAsync()
        {
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            return await _context.Employees
                .Where(e => e.HireDate >= thirtyDaysAgo)
                .ToListAsync();
        }

        // Gets administrative employees (Administration and Human Resources)
        public async Task<IEnumerable<Employee>> GetAdministrativeEmployeesAsync()
        {
            return await _context.Employees
                .Where(e => e.Department == Department.Administration || e.Department == Department.HumanResources)
                .ToListAsync();
        }

        // Checks if an employee can manage user creation
        public async Task<bool> CanEmployeeManageUserCreationAsync(int employeeId)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null)
            {
                return false;
            }

            // Checks if the employee is active and in a department that can create users
            return employee.Status == EmployeeStatus.Active &&
                   (employee.Department == Department.Administration || employee.Department == Department.HumanResources);
        }

        // Counts the number of employees by department
        public async Task<int> CountEmployeesByDepartmentAsync(Department department)
        {
            return await _context.Employees
                .CountAsync(e => e.Department == department);
        }

        // Gets employees with a complete profile (all fields filled)
        public async Task<IEnumerable<Employee>> GetEmployeesWithCompleteProfileAsync()
        {
            return await _context.Employees
                .Where(e => !string.IsNullOrEmpty(e.FirstName) &&
                            !string.IsNullOrEmpty(e.LastName) &&
                            !string.IsNullOrEmpty(e.PhoneNumber) &&
                            e.HireDate != null &&
                            e.Department != null)
                .ToListAsync();
        }

        // Gets all employees with complete data (including related entities)
        public async Task<IEnumerable<Employee>> GetAllWithIncludesAsync()
        {
            return await _context.Employees
                .Include(e => e.User) // Includes the associated User entity
                                      // Add additional includes if necessary
                .ToListAsync();
        }

        // Gets an employee by UserId
        public async Task<Employee> GetEmployeeByUserIdAsync(string userId)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.UserId == userId);
        }
    }
}
