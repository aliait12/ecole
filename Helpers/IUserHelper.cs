using Microsoft.AspNetCore.Identity;
using SchoolManagementSystem.Data.Entities;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Helpers
{
    public interface IUserHelper
    {
        // Gets a user by email.
        Task<User> GetUserByEmailAsync(string email);

        // Adds a new user with a password.
        Task<IdentityResult> AddUserAsync(User user, string password);

        // Logs in a user with LoginViewModel data.
        Task<SignInResult> LoginAsync(LoginViewModel model);

        // Logs out the current user.
        Task LogoutAsync();

        // Updates an existing user.
        Task<IdentityResult> UpdateUserAsync(User user);

        // Changes a user's password.
        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);

        // Checks if a role exists, creates it if not.
        Task CheckRoleAsync(string roleName);

        // Adds a user to a specific role.
        Task AddUserToRoleAsync(User user, string roleName);

        // Checks if a user is in a specific role.
        Task<bool> IsUserInRoleAsync(User user, string roleName);

        // Validates a user's password.
        Task<SignInResult> ValidatePasswordAsync(User user, string password);

        // Generates an email confirmation token.
        Task<string> GenerateEmailConfirmationTokenAsync(User user);

        // Confirms a user's email with a token.
        Task<IdentityResult> ConfirmEmailAsync(User user, string token);

        // Gets a user by their ID.
        Task<User> GetUserByIdAsync(string userId);

        // Generates a password reset token.
        Task<string> GeneratePasswordResetTokenAsync(User user);

        // Resets a user's password with a token.
        Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);

        // Removes a user from a role.
        Task RemoveUserFromRoleAsync(User user, string roleName);

        // Gets all users in a specific role.
        Task<List<User>> GetAllUsersInRoleAsync(string roleName);

        // Notifies administrative staff about pending users.
        Task NotifyAdministrativeEmployeesPendingUserAsync(User user);

        Task<IdentityResult> ResetPasswordWithoutTokenAsync(User user, string password);

        Task<string> GetRoleAsync(User user);

        Task UpdateUserDataByRoleAsync(User user);

        Task<Employee> GetEmployeeByUserAsync(string userEmail);

    }
}
