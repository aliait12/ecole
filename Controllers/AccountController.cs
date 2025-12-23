using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Data.Entities;
using SchoolManagementSystem.Helpers;
using SchoolManagementSystem.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;

        public AccountController(IUserHelper userHelper, IMailHelper mailHelper)
        {
            _userHelper = userHelper;
            _mailHelper = mailHelper;
        }

        // =========================
        // LOGIN / LOGOUT
        // =========================

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _userHelper.LoginAsync(model);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Failed to log in.");
                return View(model);
            }

            var user = await _userHelper.GetUserByEmailAsync(model.Username);
            if (user == null)
            {
                await _userHelper.LogoutAsync();
                ModelState.AddModelError(string.Empty, "User not found.");
                return View(model);
            }

            var userRole = await _userHelper.GetRoleAsync(user);

            return userRole switch
            {
                "Admin" => RedirectToAction("AdminDashboard", "Dashboard"),
                "Employee" => RedirectToAction("EmployeeDashboard", "Dashboard"),
                "Teacher" => RedirectToAction("TeacherDashboard", "Dashboard"),
                "Student" => RedirectToAction("StudentDashboard", "Dashboard"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        // =========================
        // REGISTER (ADMIN CREATES USER)
        // =========================

        [HttpGet]
        public IActionResult Register()
        {
            // IMPORTANT: admin should not see any password here
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterNewUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingUser = await _userHelper.GetUserByEmailAsync(model.Username);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "User already exists.");
                return View(model);
            }

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Username,
                UserName = model.Username,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                DateCreated = DateTime.UtcNow,
                EmailConfirmed = true
            };

            // ✅ Temporary password (admin DOES NOT SEE it)
            var tempPassword = GenerateTemporaryPassword();

            var result = await _userHelper.AddUserAsync(user, tempPassword);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", result.Errors.FirstOrDefault()?.Description ?? "User creation failed.");
                return View(model);
            }

            // Default role when created
            await _userHelper.AddUserToRoleAsync(user, "Pending");

            // ✅ Send credentials by email (ONLY place where password exists)
            var emailBody = $@"
                <h2>Account Created</h2>
                <p>Your account has been created in School Management System.</p>
                <p><b>Email:</b> {user.Email}</p>
                <p><b>Temporary password:</b> {tempPassword}</p>
                <p>Please login and then change your password.</p>
                <p>If you forget it, use <b>Forgot your password?</b> on the login page.</p>
            ";

            var response = _mailHelper.SendEmail(user.Email, "School Management System - Account Created", emailBody);

            if (!response.IsSuccess)
            {
                // User is created, but email failed
                ViewBag.Message = "User created successfully, but email sending failed. Check mail configuration.";
                ViewBag.MailError = response.Message;
                return View();
            }

            ViewBag.Message = "User created successfully. Credentials sent by email.";
            return View();
        }

        // =========================
        // CHANGE USER INFO
        // =========================

        public async Task<IActionResult> ChangeUser()
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

            if (user == null)
                return RedirectToAction("NotAuthorized");

            var model = new ChangeUserViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUser(ChangeUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            if (user == null)
                return RedirectToAction("NotAuthorized");

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Address = model.Address;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userHelper.UpdateUserAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Failed to update user details.");
                return View(model);
            }

            await _userHelper.UpdateUserDataByRoleAsync(user);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult NotAuthorized()
        {
            return View();
        }

        // =========================
        // CHANGE PASSWORD (LOGGED IN)
        // =========================

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }

            var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", result.Errors.FirstOrDefault()?.Description ?? "Password change failed.");
                return View(model);
            }

            return RedirectToAction("ChangeUser");
        }

        // =========================
        // FORGOT PASSWORD (RECOVER + RESET)
        // =========================

        [HttpGet]
        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userHelper.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "The email does not correspond to a registered user.");
                return View(model);
            }

            var token = await _userHelper.GeneratePasswordResetTokenAsync(user);

            // IMPORTANT: include email in link so reset can find user
            var link = Url.Action("ResetPassword", "Account",
                new { token = token, email = user.Email },
                protocol: HttpContext.Request.Scheme);

            var emailBody = $@"
                <h2>Password Reset</h2>
                <p>Click the link below to reset your password:</p>
                <p><a href=""{link}"">Reset Password</a></p>
            ";

            var response = _mailHelper.SendEmail(user.Email, "School Management System - Password Reset", emailBody);

            if (response.IsSuccess)
                ViewBag.Message = "Instructions to recover your password have been sent to your email.";
            else
                ViewBag.Message = "Failed to send email. Check your mail settings.";

            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            // Prefill token/email into the viewmodel
            var model = new ResetPasswordViewModel
            {
                Token = token,
                Email = email,
                Password = string.Empty,
                ConfirmPassword = string.Empty
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userHelper.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                ViewBag.Message = "User not found.";
                return View(model);
            }

            var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                ViewBag.Message = "Password reset successfully. You can now login.";
                return View();
            }

            ViewBag.Message = result.Errors.FirstOrDefault()?.Description ?? "Error resetting the password.";
            return View(model);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        // =========================
        // UTILS
        // =========================

        private string GenerateTemporaryPassword()
        {
            // Strong enough for a temp password (meets common Identity rules)
            // Example: Ab3!xY9@kP
            var basePart = Guid.NewGuid().ToString("N").Substring(0, 8);
            return $"Ab3!{basePart}";
        }
    }
}
