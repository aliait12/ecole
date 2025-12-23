using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Helpers;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Repositories;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace SchoolManagementSystem.Controllers
{
    [Authorize(Roles = "Employee,Admin")]

    public class SchoolClassesController : Controller
    {
        private readonly ISchoolClassRepository _schoolClassRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly ILogger<SchoolClassesController> _logger;

        public SchoolClassesController(
            ISchoolClassRepository schoolClassRepository,
            IConverterHelper converterHelper,
            ILogger<SchoolClassesController> logger)
        {
            _schoolClassRepository = schoolClassRepository;
            _converterHelper = converterHelper;
            _logger = logger;
        }

        // GET: SchoolClass/Index
        public async Task<IActionResult> Index()
        {
            var schoolClasses = await _schoolClassRepository.GetAll().ToListAsync();
            var schoolClassViewModels = schoolClasses.Select(sc => _converterHelper.ToSchoolClassViewModel(sc)).ToList();
            return View(schoolClassViewModels);
        }

        // GET: SchoolClass/Create
        public IActionResult Create()
        {
            var model = new SchoolClassViewModel
            {
                ClassName = string.Empty
            };
            return View(model);
        }

        // POST: SchoolClass/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SchoolClassViewModel model)
        {
            if (ModelState.IsValid)
            {
                var schoolClass = await _converterHelper.ToSchoolClassAsync(model, true);
                await _schoolClassRepository.CreateAsync(schoolClass);
                _logger.LogInformation("Turma criada com sucesso: {ClassName}", schoolClass.ClassName);
                return RedirectToAction(nameof(Index));
            }

            
            return View(model);
        }

        // GET: SchoolClass/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var schoolClass = await _schoolClassRepository.GetByIdAsync(id);
            if (schoolClass == null)
            {
                return new NotFoundViewResult("SchoolClassNotFound"); 
            }

            var model = _converterHelper.ToSchoolClassViewModel(schoolClass);
            return View(model);
        }

        // POST: SchoolClass/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SchoolClassViewModel model)
        {
            if (ModelState.IsValid)
            {
                var schoolClass = await _converterHelper.ToSchoolClassAsync(model, false);
                await _schoolClassRepository.UpdateAsync(schoolClass);
                _logger.LogInformation("Turma editada com sucesso: {ClassName}", schoolClass.ClassName);
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: SchoolClass/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var schoolClass = await _schoolClassRepository.GetByIdAsync(id);
            if (schoolClass == null)
            {
                return new NotFoundViewResult("SchoolClassNotFound"); 
            }

            var model = _converterHelper.ToSchoolClassViewModel(schoolClass);
            return View(model);
        }

        // POST: SchoolClass/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var schoolClass = await _schoolClassRepository.GetByIdAsync(id);
            if (schoolClass == null)
            {
                return new NotFoundViewResult("SchoolClassNotFound"); 
            }

            try
            {
                await _schoolClassRepository.DeleteAsync(schoolClass);
                _logger.LogInformation("Turma excluída com sucesso: {ClassId}", id);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                
                _logger.LogError(ex, "Erro ao excluir turma: {ClassId}", id);
                ModelState.AddModelError("", "Ocorreu um erro ao excluir a turma. Tente novamente.");
                return View(schoolClass);
            }
        }

        public IActionResult SchoolClassNotFound()
        {
            return View();
        }
    }
}
