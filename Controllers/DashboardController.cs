using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models;


namespace SchoolManagementSystem.Controllers
{
    [Authorize] // Este atributo garante que o utilizador deve estar autenticado
    public class DashboardController : Controller
    {
        private readonly IAlertRepository _alertRepository;
        private readonly SchoolDbContext _context;

        public DashboardController(IAlertRepository alertRepository, SchoolDbContext context)
        {
            _alertRepository = alertRepository;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> AdminDashboard()
        {
            return View();
        }

        [Authorize(Roles = "Employee")] 
        public IActionResult EmployeeDashboard()
        {
            return View();
        }

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> TeacherDashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var teacher = await _context.Teachers
                .Include(t => t.TeacherSchoolClasses)
                    .ThenInclude(tsc => tsc.SchoolClass)
                        .ThenInclude(sc => sc.Course)
                .Include(t => t.TeacherSchoolClasses)
                    .ThenInclude(tsc => tsc.SchoolClass)
                        .ThenInclude(sc => sc.Students)
                .Include(t => t.TeacherSubjects)
                    .ThenInclude(ts => ts.Subject)
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (teacher == null)
            {
                // Si le user connecté a le rôle Teacher mais pas encore de ligne Teacher dans la DB
                ViewBag.Error = "Teacher profile not found. Ask admin/employee to assign your teacher record.";
                ViewBag.Error = "Teacher profile not found. Ask admin/employee to assign your teacher record.";
                return View(new TeacherDashboardViewModel 
                { 
                    TeacherName = "Unknown" 
                });
            }

            var classes = teacher.TeacherSchoolClasses
                .Select(x => x.SchoolClass)
                .Where(sc => sc != null)
                .Distinct()
                .ToList();

            var subjects = teacher.TeacherSubjects
                .Select(x => x.Subject)
                .Where(s => s != null)
                .Distinct()
                .ToList();

            var model = new TeacherDashboardViewModel
            {
                TeacherName = $"{teacher.FirstName} {teacher.LastName}",
                TotalClasses = classes.Count,
                TotalSubjects = subjects.Count,
                TotalStudents = classes.Sum(c => c.Students?.Count ?? 0),

                Classes = classes.Select(c => new ClassCard
                {
                    Id = c.Id,
                    ClassName = c.ClassName,
                    CourseName = c.Course?.Name ?? "-",
                    StudentsCount = c.Students?.Count ?? 0,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate
                }).OrderBy(x => x.ClassName).ToList(),

                Subjects = subjects.Select(s => new SimpleItem
                {
                    Id = s.Id,
                    Name = s.Name
                }).OrderBy(x => x.Name).ToList()
            };

            return View(model);
        }


        [Authorize(Roles = "Student")] 
        public IActionResult StudentDashboard()
        {
            return View();
        }
    }
}
