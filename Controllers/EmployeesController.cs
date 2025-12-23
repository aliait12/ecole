using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Repositories;
using SchoolManagementSystem.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace SchoolManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IBlobHelper _blobHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IUserHelper _userHelper;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(
            IEmployeeRepository employeeRepository,
            IBlobHelper blobHelper,
            IConverterHelper converterHelper,
            IUserHelper userHelper,
            ILogger<EmployeesController> logger)
        {
            _employeeRepository = employeeRepository;
            _blobHelper = blobHelper;
            _converterHelper = converterHelper;
            _userHelper = userHelper;
            _logger = logger;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var employees = await _employeeRepository.GetAllWithIncludesAsync();
            return View(employees);
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return new NotFoundViewResult("EmployeeNotFound");

            var employee = await _employeeRepository.GetByIdAsync(id.Value);
            if (employee == null) return new NotFoundViewResult("EmployeeNotFound");

            var model = _converterHelper.ToEmployeeViewModel(employee);
            return View(model);
        }

        // GET: Employees/Create
        public async Task<IActionResult> Create()
        {
            var pendingUsers = await _userHelper.GetAllUsersInRoleAsync("Pending");

            // Initialize the ViewModel with the list of pending users
            var model = new EmployeeViewModel
            {
                PendingUsers = pendingUsers,
                UserId = string.Empty,
                FirstName = string.Empty,
                LastName = string.Empty,
                PhoneNumber = string.Empty
            };

            ViewBag.PendingUsers = new SelectList(pendingUsers, "Id", "Email");

            return View(model);
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Guid imageId = Guid.Empty;

                    
                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "employees");
                    }

                    
                    var employee = await _converterHelper.ToEmployeeAsync(model, imageId, true);

                   
                    await _employeeRepository.CreateAsync(employee);

     
                    var user = await _userHelper.GetUserByIdAsync(model.UserId);
                    await _userHelper.RemoveUserFromRoleAsync(user, "Pending");
                    await _userHelper.AddUserToRoleAsync(user, "Employee");

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating employee");
                    ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                }
            }

            // Reload dropdowns in case of error
            var pendingUsers = await _userHelper.GetAllUsersInRoleAsync("Pending");
            ViewBag.PendingUsers = new SelectList(pendingUsers, "Id", "Email");

            return View(model);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return new NotFoundViewResult("EmployeeNotFound");

            var employee = await _employeeRepository.GetByIdAsync(id.Value);
            if (employee == null) return new NotFoundViewResult("EmployeeNotFound");

            var model = _converterHelper.ToEmployeeViewModel(employee);
            return View(model);
        }

        // POST: Employees/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeViewModel model)
        {
            if (id != model.Id) return new NotFoundViewResult("EmployeeNotFound");

            if (ModelState.IsValid)
            {
                try
                {
                    Guid imageId = model.ImageId; 

 
                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "employees");
                    }

                    var employee = await _converterHelper.ToEmployeeAsync(model, imageId, false);
                    await _employeeRepository.UpdateAsync(employee);

                    // Update associated user data
                    var user = await _userHelper.GetUserByIdAsync(model.UserId);
                    if (user != null)
                    {
                        user.FirstName = model.FirstName;
                        user.LastName = model.LastName;
                        await _userHelper.UpdateUserAsync(user);
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await EmployeeExists(model.Id)) return new NotFoundViewResult("EmployeeNotFound");
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating employee");
                    ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                }
            }

            return View(model);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return new NotFoundViewResult("EmployeeNotFound");

            var employee = await _employeeRepository.GetByIdAsync(id.Value);
            if (employee == null) return new NotFoundViewResult("EmployeeNotFound");

            return View(_converterHelper.ToEmployeeViewModel(employee));
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null) return new NotFoundViewResult("EmployeeNotFound");

            try
            {
                await _employeeRepository.DeleteAsync(employee);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    ViewBag.ErrorTitle = $"{employee.Id} is being used!";
                    ViewBag.ErrorMessage = "This employee cannot be deleted. Please delete associations first.";
                }

                return View("Error");
            }
        }

        private async Task<bool> EmployeeExists(int id)
        {
            return await _employeeRepository.ExistAsync(id);
        }

        public IActionResult EmployeeNotFound()
        {
            return View();
        }
    }
}
