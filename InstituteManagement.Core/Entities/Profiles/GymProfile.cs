
namespace InstituteManagement.Core.Entities.Profiles
{
    public class GymProfile : OrgProfile
    {
        // Add gym-specific properties here
        // Relationships
        public List<GymTeacherProfile> Teachers { get; set; } = [];
        public List<GymStudentProfile> Members { get; set; } = [];
    }
}
