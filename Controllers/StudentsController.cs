using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Repositories;
using SchoolManagementSystem.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data.Entities;
using Microsoft.AspNetCore.Authorization;

namespace SchoolManagementSystem.Controllers
{
    [Authorize(Roles = "Employee,Admin")]

    public class StudentsController : Controller
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IBlobHelper _blobHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly ISchoolClassRepository _schoolClassRepository;
        private readonly IUserHelper _userHelper;
        private readonly ILogger<StudentsController> _logger; // Inject ILogger

        public StudentsController(
            IStudentRepository studentRepository,
            IBlobHelper blobHelper,
            IConverterHelper converterHelper,
            ISchoolClassRepository schoolClassRepository,
            IUserHelper userHelper,
            ILogger<StudentsController> logger) // Inject ILogger
        {
            _studentRepository = studentRepository;
            _blobHelper = blobHelper;
            _converterHelper = converterHelper;
            _schoolClassRepository = schoolClassRepository;
            _userHelper = userHelper;
            _logger = logger; // Assign the ILogger to the local field
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            var students = await _studentRepository.GetAllWithIncludesAsync();
            return View(students); // Pass the Student list directly to the view
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return new NotFoundViewResult("StudentNotFound");

            var student = await _studentRepository.GetStudentWithCoursesAsync(id.Value);
            if (student == null)
                return new NotFoundViewResult("StudentNotFound");

            return View(_converterHelper.ToStudentViewModel(student));
        }

        // GET: Students/Create
        public async Task<IActionResult> Create()
        {
            var pendingUsers = await _userHelper.GetAllUsersInRoleAsync("Pending");

            // Initialize the ViewModel with the list of pending users
            var model = new StudentViewModel
            {
                PendingUsers = pendingUsers,
                UserId = string.Empty,
                FirstName = string.Empty,
                LastName = string.Empty
            };

            ViewBag.PendingUsers = new SelectList(pendingUsers, "Id", "Email");
            await LoadDropdownData();

            return View(model);
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Checks if the selected SchoolClass has an associated CourseId
                if (model.SchoolClassId.HasValue)
                {
                    var schoolClass = await _schoolClassRepository.GetByIdAsync(model.SchoolClassId.Value);
                    if (schoolClass == null || schoolClass.CourseId == null)
                    {
                        TempData["ErrorMessage"] = "The selected school class does not have an associated course.";
                        ModelState.AddModelError("SchoolClassId", "The selected school class does not have an associated course.");
                        await LoadDropdownData();
                        var pendingUsers = await _userHelper.GetAllUsersInRoleAsync("Pending");
                        ViewBag.PendingUsers = new SelectList(pendingUsers, "Id", "Email");
                        return View(model);  // Returns the view with the current state of the model to keep the data filled
                    }
                }

                try
                {
                    Guid imageId = Guid.Empty;
                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "students");
                    }

                    var student = await _converterHelper.ToStudentAsync(model, imageId, true);
                    await _studentRepository.CreateAsync(student);

                    // Update the user from the "Pending" role to "Student"
                    var user = await _userHelper.GetUserByIdAsync(model.UserId);
                    await _userHelper.RemoveUserFromRoleAsync(user, "Pending");
                    await _userHelper.AddUserToRoleAsync(user, "Student");

                    TempData["SuccessMessage"] = "Student created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating student");
                    ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                }
            }

            // Reload dropdowns in case of error
            var updatedPendingUsers = await _userHelper.GetAllUsersInRoleAsync("Pending");
            ViewBag.PendingUsers = new SelectList(updatedPendingUsers, "Id", "Email");
            await LoadDropdownData();
            return View(model);
        }



        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return new NotFoundViewResult("StudentNotFound");

            var student = await _studentRepository.GetByIdAsync(id.Value);
            if (student == null) return new NotFoundViewResult("StudentNotFound");

            // Converts Student entity to StudentViewModel
            var model = _converterHelper.ToStudentViewModel(student);

            // Load the list of classes to the dropdown
            await LoadDropdownData();

            return View(model);
        }

        // POST: Students/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StudentViewModel model)
        {
            if (id != model.Id) return new NotFoundViewResult("StudentNotFound");

            if (ModelState.IsValid)
            {
                // Verifica se a SchoolClass selecionada possui um CourseId associado
                if (model.SchoolClassId.HasValue)
                {
                    var schoolClass = await _schoolClassRepository.GetByIdAsync(model.SchoolClassId.Value);
                    if (schoolClass == null || schoolClass.CourseId == null)
                    {
                        TempData["ErrorMessage"] = "The selected school class does not have an associated course.";
                        ModelState.AddModelError("SchoolClassId", "The selected school class does not have an associated course.");
                        await LoadDropdownData();
                        var pendingUsers = await _userHelper.GetAllUsersInRoleAsync("Pending");
                        ViewBag.PendingUsers = new SelectList(pendingUsers, "Id", "Email");
                        return View(model);  // Retorna a view com o estado atual do modelo para manter os dados preenchidos
                    }
                }

                try
                {
                    Guid imageId = model.ImageId; // Use existing image

                    // Checks if a new image has been loaded
                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "students");
                    }

                    // Convert to Student entity
                    var student = await _converterHelper.ToStudentAsync(model, imageId, false);
                    await _studentRepository.UpdateAsync(student);

                    // Update associated user data
                    var user = await _userHelper.GetUserByIdAsync(model.UserId);
                    if (user != null)
                    {
                        user.FirstName = model.FirstName;
                        user.LastName = model.LastName;
                        await _userHelper.UpdateUserAsync(user);
                    }

                    TempData["SuccessMessage"] = "Student updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await StudentExists(model.Id)) return new NotFoundViewResult("StudentNotFound");
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating student");
                    ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                }
            }

            // Reload dropdowns in case of error
            var updatedPendingUsers = await _userHelper.GetAllUsersInRoleAsync("Pending");
            ViewBag.PendingUsers = new SelectList(updatedPendingUsers, "Id", "Email");
            await LoadDropdownData();
            return View(model);
        }


        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return new NotFoundViewResult("StudentNotFound");

            var student = await _studentRepository.GetStudentWithCoursesAsync(id.Value); 

            if (student == null) return new NotFoundViewResult("StudentNotFound");

            return View(_converterHelper.ToStudentViewModel(student));
        }


        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _studentRepository.GetByIdAsync(id);

            if (student == null) return new NotFoundViewResult("StudentNotFound");

            try
            {
                await _studentRepository.DeleteAsync(student);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    ViewBag.ErrorTitle = $"{student.Id} está a ser usado!";
                    ViewBag.ErrorMessage = $"Este estudante não pode ser apagado. Tente apagar primeiro as associações.";
                }

                return View("Error");
            }
        }

        private async Task<bool> StudentExists(int id)
        {
            return await _studentRepository.ExistAsync(id);
        }

        // Load the list of classes to the dropdown
        private async Task LoadDropdownData()
        {
            var schoolClasses = await _schoolClassRepository.GetAllAsync();
            var classList = schoolClasses.ToList();

            // Load existing classes
            ViewBag.SchoolClassId = new SelectList(classList, "Id", "ClassName");
        }

        public IActionResult StudentNotFound()
        {
            return View();
        }

    }
}
