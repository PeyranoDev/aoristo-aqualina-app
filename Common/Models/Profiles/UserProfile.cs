using AutoMapper;
using Common.Models;
using Common.Models.Requests;
using Common.Models.Responses;
using Data.Entities;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserForResponse>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.Type.ToString().ToLower()))
            .ForMember(dest => dest.ApartmentInfo, opt => opt.MapFrom(src => src.Apartment == null
                ? null
                : new ApartmentInfoDTO
                {
                    Id = src.Apartment.Id,
                    Identifier = src.Apartment.Identifier
                }));

        CreateMap<UserForCreateDTO, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.Apartment, opt => opt.Ignore());
    }
}
