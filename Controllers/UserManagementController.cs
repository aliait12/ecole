//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using SchoolManagementSystem.Helpers;
//using SchoolManagementSystem.Models;
//using System.Threading.Tasks;

//namespace SchoolManagementSystem.Controllers
//{
//    //[Authorize(Roles = "Admin")] // Only Admins can manage user roles
//    public class UserManagementController : Controller
//    {
//        private readonly IUserHelper _userHelper;

//        public UserManagementController(IUserHelper userHelper)
//        {
//            _userHelper = userHelper;
//        }

//        // Displays a list of users with the role "Pending"
//        public async Task<IActionResult> Index()
//        {
//            var users = await _userHelper.GetAllUsersInRoleAsync("Pending");
//            return View(users); // List all users in the "Pending" role
//        }

//        // Displays the form to assign a new role to a user
//        public async Task<IActionResult> AssignRole(string userId)
//        {
//            var user = await _userHelper.GetUserByIdAsync(userId);
//            if (user == null)
//            {
//                return NotFound();
//            }

//            var model = new AssignRoleViewModel
//            {
//                UserId = user.Id,
//                Roles = new List<string> { "Admin", "Employee", "Student", "Anonymous" } // Available roles
//            };

//            return View(model);
//        }

//        // Handles the role assignment
//        [HttpPost]
//        public async Task<IActionResult> AssignRole(AssignRoleViewModel model)
//        {
//            var user = await _userHelper.GetUserByIdAsync(model.UserId);
//            if (user == null)
//            {
//                return NotFound();
//            }

//            // Remove the user from the "Pending" role and assign the selected role
//            await _userHelper.RemoveUserFromRoleAsync(user, "Pending");
//            await _userHelper.AddUserToRoleAsync(user, model.SelectedRole);

//            return RedirectToAction("Index");
//        }
//    }
//}
