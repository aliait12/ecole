using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Repositories;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICourseRepository _courseRepository;
        private readonly ISchoolClassRepository _schoolClassRepository;
        private readonly IAlertRepository _alertRepository; 

        public HomeController(ILogger<HomeController> logger,
                              ICourseRepository courseRepository,
                              ISchoolClassRepository schoolClassRepository,
                              IAlertRepository alertRepository)  
        {
            _logger = logger;
            _courseRepository = courseRepository;
            _schoolClassRepository = schoolClassRepository;
            _alertRepository = alertRepository; 
        }

        public async Task<IActionResult> Index()
        {

            // Process courses and classes
            var courses = await _courseRepository.GetAllCourseViewModelsAsync();
            var schoolClasses = await _schoolClassRepository.GetAllWithDetailsAsync();

            var schoolClassViewModels = schoolClasses
                .Where(sc => sc.Course != null && sc.Course.CourseSubjects.Any())
                .Select(sc => new SchoolClassViewModel
                {
                    Id = sc.Id,
                    ClassName = sc.ClassName,
                    StartDate = sc.StartDate,
                    EndDate = sc.EndDate,
                    Subjects = sc.Course.CourseSubjects.Select(cs => cs.Subject.Name).ToList()
                }).ToList();

            var model = new HomeViewModel
            {
                Courses = courses,
                SchoolClasses = schoolClassViewModels
            };

            return View(model);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Courses()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
