using AutoMapper;
using InstituteManagement.Core.Common.ValueObjects;
using InstituteManagement.Core.Entities.People;
using InstituteManagement.Shared.DTOs.Signup;
using InstituteManagement.Shared.DTOs.UserProfile;

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

            CreateMap<UpdatePersonDto, Person>()
                .ForMember(dest => dest.AppUserId, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    if (dest.PrimaryAddress == null)
                        dest.PrimaryAddress = new Address();

                    dest.PrimaryAddress.Country = src.Country;
                    dest.PrimaryAddress.Province = src.Province;
                    dest.PrimaryAddress.City = src.City;
                    dest.PrimaryAddress.District = src.District;
                    dest.PrimaryAddress.FullAddress = src.FullAddress;
                    dest.PrimaryAddress.PostalCode = src.PostalCode;
                    dest.PrimaryAddress.Latitude = src.Latitude;
                    dest.PrimaryAddress.Longitude = src.Longitude;
                });

            CreateMap<Person, PersonDto>()
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.PrimaryAddress != null ? src.PrimaryAddress.Country : null))
                .ForMember(dest => dest.Province, opt => opt.MapFrom(src => src.PrimaryAddress != null ? src.PrimaryAddress.Province : null))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.PrimaryAddress != null ? src.PrimaryAddress.City : null))
                .ForMember(dest => dest.District, opt => opt.MapFrom(src => src.PrimaryAddress != null ? src.PrimaryAddress.District : null))
                .ForMember(dest => dest.FullAddress, opt => opt.MapFrom(src => src.PrimaryAddress != null ? src.PrimaryAddress.FullAddress : null))
                .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.PrimaryAddress != null ? src.PrimaryAddress.PostalCode : null))
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.PrimaryAddress != null ? src.PrimaryAddress.Latitude : null))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.PrimaryAddress != null ? src.PrimaryAddress.Longitude : null));
        }
    }
}
