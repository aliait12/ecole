namespace SchoolManagementSystem.Models
{
    public class AssignRoleViewModel
    {
        public required string UserId { get; set; }
        public required string SelectedRole { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}
