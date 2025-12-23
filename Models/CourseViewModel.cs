using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagementSystem.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models
{
    public class CourseViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [Range(1, 52, ErrorMessage = "The course duration must be between 1 and 52 weeks.")]
        public int Duration { get; set; } 

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;



        // IDs selected through checkboxes
        public List<int> SelectedSchoolClassIds { get; set; } = new List<int>();
        public List<int> SelectedSubjectIds { get; set; } = new List<int>();


        // Lists to display in checkboxes
        public List<SchoolClass> SchoolClasses { get; set; } = new List<SchoolClass>();
        public List<Subject> Subjects { get; set; } = new List<Subject>();

        // Checkboxes to display
        public List<SelectListItem> SchoolClassItems { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> SubjectItems { get; set; } = new List<SelectListItem>();
    }
}
