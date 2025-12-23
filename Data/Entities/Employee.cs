using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Data.Entities
{
    public class Employee : IEntity
    {
        // Implementation of the IEntity interface
        public int Id { get; set; }

        public required string UserId { get; set; }

        // Navigation to the User entity (associated with the employee)
        public User User { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is required")]
        [MaxLength(50)]
        public required string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is required")]
        [MaxLength(50)]
        public required string LastName { get; set; }

        // Position of the employee
        [Display(Name = "Department")]
        public Department Department { get; set; }  // Enum added for departments

        // Academic degree of the employee (e.g., Bachelor's, Master's)
        [Display(Name = "Academic Degree")]
        public AcademicDegree AcademicDegree { get; set; }

        // Hire date of the employee
        [Display(Name = "Hire Date")]
        public DateTime? HireDate { get; set; }

        // Formatted string to display the hire date
        public string FormattedHireDate => HireDate?.ToString("dd/MM/yyyy");

        // Status of the employee (Active, Inactive, Pending)
        [Display(Name = "Status")]
        public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;

        // Phone number of the employee
        [Phone]
        [Display(Name = "Phone Number")]
        public required string PhoneNumber { get; set; }

        // Field for the employee's image
        [Display(Name = "Image")]
        public Guid ImageId { get; set; }

        // Full URL path to the employee's profile image
        public string ImageFullPath => ImageId == Guid.Empty
            ? "https://devartacademyfiles.blob.core.windows.net/images/noimage.png"
            : $"https://devartacademyfiles.blob.core.windows.net/employees/{ImageId}";
    }

    // Enum to standardize employee status
    public enum EmployeeStatus
    {
        Pending,
        Active,
        Inactive
    }

    // Enum for possible departments in a school
    public enum Department
    {
        [Display(Name = "Administration")]
        Administration,

        [Display(Name = "Human Resources")]
        HumanResources,

        [Display(Name = "Finance")]
        Finance,

        [Display(Name = "IT")]
        IT,

        [Display(Name = "Maintenance")]
        Maintenance,

        [Display(Name = "Teaching Support")]
        TeachingSupport,

        [Display(Name = "Security")]
        Security,

        [Display(Name = "Library")]
        Library
    }
}
