using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data.Entities;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Repositories;

namespace SchoolManagementSystem.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IAlertRepository _alertRepository;

        public UserHelper(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager,
            IEmployeeRepository employeeRepository,
            IStudentRepository studentRepository,
            ITeacherRepository teacherRepository,
            IAlertRepository alertRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _employeeRepository = employeeRepository;
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
            _alertRepository = alertRepository;
        }

        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task AddUserToRoleAsync(User user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        }

        public async Task CheckRoleAsync(string roleName)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = roleName
                });
            }
        }

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<bool> IsUserInRoleAsync(User user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            return await _signInManager.PasswordSignInAsync(
                model.Username,
                model.Password,
                model.RememberMe,
                false);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string password)
        {
            return await _userManager.ResetPasswordAsync(user, token, password);
        }

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<SignInResult> ValidatePasswordAsync(User user, string password)
        {
            return await _signInManager.CheckPasswordSignInAsync(
                user,
                password,
                false);
        }

        public async Task RemoveUserFromRoleAsync(User user, string roleName)
        {
            await _userManager.RemoveFromRoleAsync(user, roleName);
        }

        public async Task<List<User>> GetAllUsersInRoleAsync(string roleName)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
            return usersInRole.ToList();
        }

        public async Task NotifyAdministrativeEmployeesPendingUserAsync(User user)
        {
            var administrativeEmployees = await _employeeRepository.GetAdministrativeEmployeesAsync();

            foreach (var adminEmployee in administrativeEmployees)
            {
                var alert = new Alert
                {
                    Message = $"New User 'Pending': {user.FullName}",
                    CreatedAt = DateTime.UtcNow,
                    IsResolved = false,
                    EmployeeId = adminEmployee.Id
                };

                await _alertRepository.CreateAsync(alert);
            }
        }

        public async Task<IdentityResult> ResetPasswordWithoutTokenAsync(User user, string password)
        {
            // Update the password directly, creating a hash from the new password
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, password);
            return await _userManager.UpdateAsync(user);
        }

        public async Task<string> GetRoleAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles.FirstOrDefault(); // Returns the first role associated with the user
        }

        public async Task UpdateUserDataByRoleAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Employee"))
            {
                var employee = await _employeeRepository.GetEmployeeByUserIdAsync(user.Id);
                if (employee != null)
                {
                    employee.FirstName = user.FirstName;
                    employee.LastName = user.LastName;
                    await _employeeRepository.UpdateAsync(employee);
                }
            }
            else if (roles.Contains("Student"))
            {
                var student = await _studentRepository.GetStudentByUserIdAsync(user.Id);
                if (student != null)
                {
                    student.FirstName = user.FirstName;
                    student.LastName = user.LastName;
                    await _studentRepository.UpdateAsync(student);
                }
            }
            else if (roles.Contains("Teacher"))
            {
                var teacher = await _teacherRepository.GetTeacherByUserIdAsync(user.Id);
                if (teacher != null)
                {
                    teacher.FirstName = user.FirstName;
                    teacher.LastName = user.LastName;
                    await _teacherRepository.UpdateAsync(teacher);
                }
            }
        }

        public async Task<Employee> GetEmployeeByUserAsync(string userEmail)
        {
            var user = await GetUserByEmailAsync(userEmail);  // Search for the user by email
            if (user == null) return null;

            return await _employeeRepository.GetEmployeeByUserIdAsync(user.Id); // Search for the employee by UserId
        }


    }
}
