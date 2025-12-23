using SchoolManagementSystem.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models
{
    public class SchoolClassViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public required string ClassName { get; set; }

        public int? CourseId { get; set; } 

        [Display(Name = "Start Date")]
        [Required]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End Date")]
        [Required]
        public DateTime? EndDate { get; set; }

        
        public List<int> StudentIds { get; set; } = new List<int>();

        
        public List<int> TeacherIds { get; set; } = new List<int>();

        
        public IEnumerable<Student> Students { get; set; } = new List<Student>();
        public IEnumerable<Teacher> Teachers { get; set; } = new List<Teacher>();

        public IEnumerable<string> Subjects { get; set; } = new List<string>();

    }
}
