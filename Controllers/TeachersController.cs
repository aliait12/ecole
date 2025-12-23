using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Repositories;
using SchoolManagementSystem.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace SchoolManagementSystem.Controllers
{
    [Authorize(Roles = "Employee,Admin")]
    public class TeachersController : Controller
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly ISchoolClassRepository _schoolClassRepository;
        private readonly IBlobHelper _blobHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IUserHelper _userHelper; // Added to get pending users
        private readonly ILogger<TeachersController> _logger;

        public TeachersController(
            ITeacherRepository teacherRepository,
            ISubjectRepository subjectRepository,
            ISchoolClassRepository schoolClassRepository,
            IBlobHelper blobHelper,
            IConverterHelper converterHelper,
            IUserHelper userHelper, 
            ILogger<TeachersController> logger)
        {
            _teacherRepository = teacherRepository;
            _subjectRepository = subjectRepository;
            _schoolClassRepository = schoolClassRepository;
            _blobHelper = blobHelper;
            _converterHelper = converterHelper;
            _userHelper = userHelper; 
            _logger = logger;
        }

        // GET: Teachers
        public async Task<IActionResult> Index()
        {
            var teachers = await _teacherRepository.GetAllWithIncludesAsync();
            return View(teachers);
        }

        // GET: Teachers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return new NotFoundViewResult("TeacherNotFound");

            // Search for the teacher with associated classes and subjects
            var selectedTeacher = await _teacherRepository.GetTeacherWithDetailsAsync(id.Value);

            if (selectedTeacher == null) return new NotFoundViewResult("TeacherNotFound");

            // Convert the teacher to the ViewModel
            var model = _converterHelper.ToTeacherViewModel(selectedTeacher);

            return View(model);
        }




        // GET: Teachers/Create
        public async Task<IActionResult> Create()
        {
            var pendingUsers = await _userHelper.GetAllUsersInRoleAsync("Pending");

            // Initialize the ViewModel with the list of pending users
            var model = new TeacherViewModel
            {
                PendingUsers = pendingUsers,
                UserId = string.Empty,
                FirstName = string.Empty,
                LastName = string.Empty
            };

            // Load subjects and classes for dropdowns
            await LoadDropdownData();
            ViewBag.PendingUsers = new SelectList(pendingUsers, "Id", "Email"); // Populates the dropdown for pending users

            return View(model);
        }

        // POST: Teachers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeacherViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownData(); // Reload dropdowns in case of error
                var pendingUsers = await _userHelper.GetAllUsersInRoleAsync("Pending");
                ViewBag.PendingUsers = new SelectList(pendingUsers, "Id", "Email");
                return View(model); // Return the view with validation errors
            }

            try
            {
                Guid imageId = Guid.Empty;

                // Check if an image has been loaded
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "teachers");
                }

                var teacher = await _converterHelper.ToTeacherAsync(model, imageId, true);

                await _teacherRepository.CreateAsync(teacher);

                var user = await _userHelper.GetUserByIdAsync(model.UserId);
                await _userHelper.RemoveUserFromRoleAsync(user, "Pending");
                await _userHelper.AddUserToRoleAsync(user, "Teacher");

                return RedirectToAction(nameof(Index)); // Redirect upon success
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating teacher");
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
            }

            await LoadDropdownData();// Reload dropdowns in case of error
            var pendingUsersReload = await _userHelper.GetAllUsersInRoleAsync("Pending");
            ViewBag.PendingUsers = new SelectList(pendingUsersReload, "Id", "Email");
            return View(model); // Return the view with validation errors
        }


        // GET: Teachers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return new NotFoundViewResult("TeacherNotFound");

            var teacher = await _teacherRepository.GetTeacherWithDetailsAsync(id.Value);
            if (teacher == null) return new NotFoundViewResult("TeacherNotFound");

            // Convert Teacher entity to TeacherViewModel
            var model = _converterHelper.ToTeacherViewModel(teacher);

            // Load subjects and classes for dropdowns
            await LoadDropdownData();

            // Ensure selected classes and subjects are included in the model
            model.SchoolClassIds = teacher.TeacherSchoolClasses.Select(tsc => tsc.SchoolClassId).ToList();
            model.SubjectIds = teacher.TeacherSubjects.Select(ts => ts.SubjectId).ToList();

            return View(model);
        }


        // POST: Teachers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TeacherViewModel model)
        {
            if (id != model.Id) return new NotFoundViewResult("TeacherNotFound");

            if (ModelState.IsValid)
            {
                try
                {
                    Guid imageId = model.ImageId; // Use existing image

                    // Checks if a new image has been loaded
                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "teachers");
                    }

                    // Converts the ViewModel to the Teacher entity
                    var teacher = await _converterHelper.ToTeacherAsync(model, imageId, false);

                    // Update subjects and classes
                    await _teacherRepository.UpdateTeacherSubjectsAsync(teacher.Id, model.SubjectIds);
                    await _teacherRepository.UpdateTeacherClassesAsync(teacher.Id, model.SchoolClassIds);

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
                    if (!await TeacherExists(model.Id)) return new NotFoundViewResult("TeacherNotFound");
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating teacher");
                    ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                }
            }

            // Reload data in case of error
            await LoadDropdownData();
            return View(model);
        }


        // GET: Teachers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return new NotFoundViewResult("TeacherNotFound");

            // Search for the teacher by ID
            var teacher = await _teacherRepository.GetTeacherWithDetailsAsync(id.Value);
            if (teacher == null) return new NotFoundViewResult("TeacherNotFound");

            // Convert Teacher to TeacherViewModel
            var model = _converterHelper.ToTeacherViewModel(teacher);

            return View(model);
        }


        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _teacherRepository.GetByIdAsync(id);
            if (teacher == null) return new NotFoundViewResult("TeacherNotFound");

            try
            {
                await _teacherRepository.DeleteAsync(teacher);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    ViewBag.ErrorTitle = $"{teacher.FirstName} {teacher.LastName} is being used!";
                    ViewBag.ErrorMessage = "This teacher cannot be deleted because it has associated data.";
                }
                return View("Error");
            }
        }


        private async Task<bool> TeacherExists(int id)
        {
            return await _teacherRepository.ExistAsync(id);
        }

        // Load subjects and classes for dropdowns
        private async Task LoadDropdownData()
        {
            var subjects = await _subjectRepository.GetAllSubjectsAsync();
            var classes = await _schoolClassRepository.GetAllAsync();

            ViewBag.Subjects = subjects.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name 
            });

            ViewBag.SchoolClasses = classes.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.ClassName 
            });
        }

        public IActionResult TeacherNotFound()
        {
            return View();
        }
    }
}
