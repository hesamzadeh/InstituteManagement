
namespace InstituteManagement.Core.Entities.Profiles
{
    public class InstituteProfile : OrgProfile
    {
        // Add institute-specific properties here
        // Relationships
        public List<InstituteTeacherProfile> Teachers { get; set; } = [];
        public List<InstituteStudentProfile> Members { get; set; } = [];
    }
}
