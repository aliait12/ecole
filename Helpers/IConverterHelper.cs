using SchoolManagementSystem.Data.Entities;
using SchoolManagementSystem.Models;
using System;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Helpers
{
    public interface IConverterHelper
    {
        // Converts StudentViewModel to Student (async)
        Task<Student> ToStudentAsync(StudentViewModel model, Guid imageId, bool isNew);

        // Converts Student to StudentViewModel
        StudentViewModel ToStudentViewModel(Student student);

        // Converts TeacherViewModel to Teacher (async)
        Task<Teacher> ToTeacherAsync(TeacherViewModel model, Guid imageId, bool isNew);

        // Converts Teacher to TeacherViewModel
        TeacherViewModel ToTeacherViewModel(Teacher teacher);

        // Converts EmployeeViewModel to Employee (async)
        Task<Employee> ToEmployeeAsync(EmployeeViewModel model, Guid imageId, bool isNew);

        // Converts Employee to EmployeeViewModel
        EmployeeViewModel ToEmployeeViewModel(Employee employee);

        // Converts SchoolClassViewModel to SchoolClass (async)
        Task<Course> ToCourseAsync(CourseViewModel model, bool isNew);

        // Convert from Course Entity to CourseViewMode
        CourseViewModel ToCourseViewModel(Course course);

        // Convert from SchoolClassViewModel to SchoolClass Entity
        Task<SchoolClass> ToSchoolClassAsync(SchoolClassViewModel model, bool isNew);

        // Convert from SchoolClass Entity to SchoolClassViewModel
        SchoolClassViewModel ToSchoolClassViewModel(SchoolClass schoolClass);

        // Convert from SubjectViewModel to Subject Entity
        Task<Subject> ToSubjectAsync(SubjectViewModel model, bool isNew);

        // Convert from Subject Entity to SubjectViewModel
        SubjectViewModel ToSubjectViewModel(Subject subject);

        Task<Grade> ToGradeAsync(GradeViewModel model, bool isNew);
        GradeViewModel ToGradeViewModel(Grade grade);

        // Methods for Attendance
        Task<Attendance> ToAttendanceAsync(AttendanceViewModel model, bool isNew);
        AttendanceViewModel ToAttendanceViewModel(Attendance attendance);

    }
}
