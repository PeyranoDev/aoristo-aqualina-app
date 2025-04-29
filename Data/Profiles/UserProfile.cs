using AutoMapper;
using Common.Models;
using Data.Entities;

public class UserProfile : Profile  
{
    public UserProfile()
    {
        
        CreateMap<UserForUpdateDTO, User>()
            .ForAllMembers(opts => opts.Condition(
                (src, dest, srcMember) => srcMember != null  
            ));

        CreateMap<User, UserForResponse>()
        .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
        .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.Surname))
        .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.Type.ToString()))
        .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
        .ForMember(dest => dest.Apartment, opt => opt.MapFrom(src => src.Apartment != null ? src.Apartment.Identifier : null));
    }
}