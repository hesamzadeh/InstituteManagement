using AutoMapper;
using InstituteManagement.Core.Entities;
using InstituteManagement.Shared.DTOs.Signup;

namespace InstituteManagement.API.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<SignupDto, Person>()
                .ForMember(dest => dest.Birthday, opt => opt.MapFrom(src => src.Birthday))
                .ForMember(dest => dest.SignupDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.IsLocked, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.NationalId, opt => opt.MapFrom(src => src.NationalId))
                .ForMember(dest => dest.AppUserId, opt => opt.Ignore()); // will be assigned manually
            // Add other mappings as needed
        }
    }
}
