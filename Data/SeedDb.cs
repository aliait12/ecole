using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Data.Entities;
using SchoolManagementSystem.Helpers;


namespace SchoolManagementSystem.Data
{
    public class SeedDb
    {
        private readonly SchoolDbContext _context;
        private readonly IUserHelper _userHelper;

        public SeedDb(SchoolDbContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            // ✅ applique migrations si besoin (safe)

            // -----------------------------
            // ROLES
            // -----------------------------
            string[] roles = { "Admin", "Student", "Teacher", "Employee", "Anonymous", "Pending" };
            foreach (var role in roles)
                await _userHelper.CheckRoleAsync(role);

            // -----------------------------
            // USERS
            // -----------------------------
            var adminUser = await CreateUserAsync("admin@school.com", "Admin", "User", "Admin123!", "Admin");
            var studentUser1 = await CreateUserAsync("student1@school.com", "Student1", "User", "Student123!", "Student");
            var studentUser2 = await CreateUserAsync("student2@school.com", "Student2", "User", "Student123!", "Student");
            var teacherUser1 = await CreateUserAsync("teacher1@school.com", "Teacher1", "User", "Teacher123!", "Teacher");
            var employeeUser1 = await CreateUserAsync("employee1@school.com", "Employee1", "User", "Employee123!", "Employee");

            // -----------------------------
            // SCHOOL CLASSES
            // -----------------------------
            if (!await _context.SchoolClasses.AnyAsync())
            {
                _context.SchoolClasses.AddRange(
                    new SchoolClass
                    {
                        ClassName = "Class A",
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddMonths(6)
                    },
                    new SchoolClass
                    {
                        ClassName = "Class B",
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddMonths(6)
                    }
                );

                await _context.SaveChangesAsync();
            }

            var classA = await _context.SchoolClasses.FirstAsync(c => c.ClassName == "Class A");
            var classB = await _context.SchoolClasses.FirstAsync(c => c.ClassName == "Class B");

            // -----------------------------
            // SUBJECTS
            // -----------------------------
            if (!await _context.Subjects.AnyAsync())
            {
                _context.Subjects.AddRange(
                    new Subject
                    {
                        Name = "Algebra",
                        Description = "Basic Algebra",
                        Credits = 5,
                        TotalClasses = 30
                    },
                    new Subject
                    {
                        Name = "Physics",
                        Description = "Basic Physics",
                        Credits = 4,
                        TotalClasses = 25
                    }
                );

                await _context.SaveChangesAsync();
            }

            var algebra = await _context.Subjects.FirstAsync(s => s.Name == "Algebra");
            var physics = await _context.Subjects.FirstAsync(s => s.Name == "Physics");

            // -----------------------------
            // COURSES
            // -----------------------------
            if (!await _context.Courses.AnyAsync())
            {
                var math = new Course
                {
                    Name = "Mathematics",
                    Description = "Mathematics Course",
                    Duration = 16,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    SchoolClasses = new List<SchoolClass> { classA }
                };

                var science = new Course
                {
                    Name = "Science",
                    Description = "Science Course",
                    Duration = 16,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    SchoolClasses = new List<SchoolClass> { classB }
                };

                _context.Courses.AddRange(math, science);
                await _context.SaveChangesAsync();

                _context.CourseSubjects.AddRange(
                    new CourseSubject { CourseId = math.Id, SubjectId = algebra.Id },
                    new CourseSubject { CourseId = science.Id, SubjectId = physics.Id }
                );

                await _context.SaveChangesAsync();
            }

            // -----------------------------
            // TEACHER
            // -----------------------------
            if (!await _context.Teachers.AnyAsync() && teacherUser1 != null)
            {
                var teacher = new Teacher
                {
                    FirstName = "Teacher1",
                    LastName = "User",
                    UserId = teacherUser1.Id,
                    HireDate = DateTime.UtcNow,
                    Status = TeacherStatus.Active,
                    AcademicDegree = AcademicDegree.MastersDegree
                };

                _context.Teachers.Add(teacher);
                await _context.SaveChangesAsync();

                _context.TeacherSubjects.AddRange(
                    new TeacherSubject { TeacherId = teacher.Id, SubjectId = algebra.Id },
                    new TeacherSubject { TeacherId = teacher.Id, SubjectId = physics.Id }
                );

                _context.TeacherSchoolClasses.Add(
                    new TeacherSchoolClass { TeacherId = teacher.Id, SchoolClassId = classA.Id }
                );

                await _context.SaveChangesAsync();
            }

            // -----------------------------
            // EMPLOYEE
            // -----------------------------
            if (!await _context.Employees.AnyAsync() && employeeUser1 != null)
            {
                _context.Employees.Add(new Employee
                {
                    FirstName = "Employee1",
                    LastName = "User",
                    UserId = employeeUser1.Id,
                    Department = Department.Administration, // ✅ enum non-null
                    HireDate = DateTime.UtcNow,
                    Status = EmployeeStatus.Active,
                    PhoneNumber = "1234567890"
                });

                await _context.SaveChangesAsync();
            }
        }

        private async Task<User?> CreateUserAsync(
            string email,
            string firstName,
            string lastName,
            string password,
            string role)
        {
            var user = await _userHelper.GetUserByEmailAsync(email);
            if (user != null) return user;

            user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserName = email,
                EmailConfirmed = true,
                DateCreated = DateTime.UtcNow
            };

            var result = await _userHelper.AddUserAsync(user, password);
            if (!result.Succeeded) return null;

            await _userHelper.AddUserToRoleAsync(user, role);
            return user;
        }
    }
}
