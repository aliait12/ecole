using SchoolManagementSystem.Data.Entities;
using System.Collections.Generic;
using System.Linq;

namespace SchoolManagementSystem.Models
{
    public class StudentSubjectAttendanceViewModel
    {
        public required Subject Subject { get; set; } // The subject related to the absence
        public Attendance? Attendance { get; set; } // The Attendance entity that contains absence details
        public int StudentId { get; set; } // Student ID
        public required string StudentName { get; set; } // Student name

        // Status based on presence or absence
        public string Status => Attendance != null ? "Absent" : "Present";

        // List of all absences for this subject
        public required List<Attendance> AllAttendances { get; set; }

        // Property that calculates the total absences
        public int TotalAbsences => AllAttendances?.Count() ?? 0; // Count of absences

        // Total number of classes
        public int TotalClasses => Subject.TotalClasses; // Gets the total number of classes for the subject

        // Checks if the student can add more absences
        public bool CanAddAttendance { get; set; } // True if the student can add absences, false otherwise

        // Checks if the student has failed this subject
        public string SubjectAttendanceStatus()
        {
            if (TotalClasses == 0) return "No classes available";

            double allowedAbsences = TotalClasses * 0.2; // 20% of the total number of classes
            return TotalAbsences > allowedAbsences ? "Failed" : "Passed"; // If exceeds the limit, failed
        }
    }
}
