using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Repositories;
using SchoolManagementSystem.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace SchoolManagementSystem.Controllers
{
    [Authorize(Roles = "Employee,Admin")]

    public class CoursesController : Controller
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ISchoolClassRepository _schoolClassRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly ILogger<CoursesController> _logger;

        public CoursesController(
            ICourseRepository courseRepository,
            ISchoolClassRepository schoolClassRepository,
            ISubjectRepository subjectRepository,
            IConverterHelper converterHelper,
            ILogger<CoursesController> logger)
        {
            _courseRepository = courseRepository;
            _schoolClassRepository = schoolClassRepository;
            _subjectRepository = subjectRepository;
            _converterHelper = converterHelper;
            _logger = logger;
        }

        // GET: Course/Index
        public async Task<IActionResult> Index()
        {
            var courseViewModels = await _courseRepository.GetAllCourseViewModelsAsync();
            return View(courseViewModels);
        }

        // GET: Course/Create
        public async Task<IActionResult> Create()
        {
            var model = new CourseViewModel
            {
                Name = string.Empty,
                SchoolClassItems = await GetAvailableSchoolClassItemsAsync(),
                SubjectItems = await GetAllSubjectItemsAsync()
            };

            return View(model);
        }

        // POST: Course/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var course = await _converterHelper.ToCourseAsync(model, true);
                    await _courseRepository.CreateAsync(course);
                    _logger.LogInformation("Curso criado com sucesso: {CourseName}", course.Name);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao criar curso: {CourseName}", model.Name);
                    ModelState.AddModelError(string.Empty, "Ocorreu um erro ao criar o curso. Tente novamente.");
                }
            }

            // Reload dropdowns in case of error
            model.SchoolClassItems = await GetAvailableSchoolClassItemsAsync();
            model.SubjectItems = await GetAllSubjectItemsAsync();
            return View(model);
        }

        // GET: Course/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _courseRepository.GetCourseWithDetailsAsync(id);
            if (course == null)
            {
                return new NotFoundViewResult("CourseNotFound");
            }

            var model = _converterHelper.ToCourseViewModel(course);
            model.SchoolClassItems = await GetAvailableSchoolClassItemsAsync();
            model.SubjectItems = await GetAllSubjectItemsAsync();
            model.SelectedSchoolClassIds = course.SchoolClasses.Select(sc => sc.Id).ToList();
            model.SelectedSubjectIds = course.CourseSubjects.Select(cs => cs.SubjectId).ToList();

            return View(model);
        }

        // POST: Course/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CourseViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var course = await _converterHelper.ToCourseAsync(model, false);
                    await _courseRepository.UpdateAsync(course);
                    _logger.LogInformation("Curso editado com sucesso: {CourseName}", course.Name);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao editar curso: {CourseName}", model.Name);
                    ModelState.AddModelError(string.Empty, "Ocorreu um erro ao editar o curso. Tente novamente.");
                }
            }

            model.SchoolClassItems = await GetAvailableSchoolClassItemsAsync();
            model.SubjectItems = await GetAllSubjectItemsAsync();
            return View(model);
        }

        // GET: Course/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var model = await _courseRepository.GetCourseDetailsViewModelAsync(id);
            if (model == null)
            {
                return new NotFoundViewResult("CourseNotFound"); 
            }

            return View(model);
        }

        // GET: Course/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return new NotFoundViewResult("CourseNotFound"); 

            var course = await _courseRepository.GetByIdAsync(id.Value);
            if (course == null)
                return new NotFoundViewResult("CourseNotFound"); 

            var model = _converterHelper.ToCourseViewModel(course);
            return View(model);
        }

        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
                return new NotFoundViewResult("CourseNotFound"); 

            try
            {
                await _courseRepository.DeleteAsync(course);
                _logger.LogInformation("Curso excluído com sucesso: {CourseId}", id);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    ViewBag.ErrorTitle = $"{course.Name} está sendo utilizado!";
                    ViewBag.ErrorMessage = "Este curso não pode ser excluído porque possui dados associados.";
                }
                return View("Error"); 
            }
        }

        // Private method to load available classes as SelectListItems
        private async Task<List<SelectListItem>> GetAvailableSchoolClassItemsAsync()
        {
            var availableSchoolClasses = await _schoolClassRepository.GetAvailableSchoolClassesAsync();
            return availableSchoolClasses.Select(sc => new SelectListItem
            {
                Value = sc.Id.ToString(),
                Text = sc.ClassName
            }).ToList();
        }

        // Private method to load all subjects as SelectListItems
        private async Task<List<SelectListItem>> GetAllSubjectItemsAsync()
        {
            var subjects = await _subjectRepository.GetAllSubjectsAsync();
            return subjects.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            }).ToList();
        }

        public IActionResult CourseNotFound()
        {
            return View();
        }
    }
}
