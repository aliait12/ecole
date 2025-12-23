using System.ComponentModel;

namespace SchoolManagementSystem.Data.Entities
{
    public enum AcademicDegree
    {
        [Description("No Degree")]
        None,
        [Description("High School Diploma")]
        HighSchoolDiploma,
        [Description("Technical Diploma")]
        TechnicalDiploma,
        [Description("Bachelor's Degree")]
        BachelorsDegree,
        [Description("Licentiate Degree")]
        LicentiateDegree,
        [Description("Master's Degree")]
        MastersDegree,
        [Description("Doctorate Degree")]
        DoctorateDegree,
        [Description("Post-Doctorate")]
        PostDoctorate
    }
}
