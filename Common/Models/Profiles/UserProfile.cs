using AutoMapper;
using Common.Models;
using Common.Models.Requests;
using Common.Models.Responses;
using Data.Entities;

public class UserProfile : Profile
{
    public UserProfile()
    {
        // Mapeo para actualización (conserva tu lógica original)
        CreateMap<UserForUpdateDTO, User>()
            .ForAllMembers(opts => opts.Condition(
                (src, dest, srcMember) => srcMember != null
            ));

       
        CreateMap<User, UserForResponse>()
            .ForMember(dest => dest.RoleType, opt => opt.MapFrom(src => src.Role.Type.ToString()))
            .ForMember(dest => dest.ApartmentInfo, opt => opt.MapFrom(src =>
                src.Apartment != null ? new Apartment
                {
                    Id = src.Apartment.Id,
                    Identifier = src.Apartment.Identifier,
                } : null));

     
        CreateMap<Role, RoleResponseDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

 
        CreateMap<User, UserForResponse>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.Type.ToString()))
            .ForMember(dest => dest.Apartment, opt => opt.MapFrom(src =>
                src.Apartment != null ? src.Apartment.Identifier : null))
            .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Role.Id))
            .ForMember(dest => dest.ApartmentId, opt => opt.MapFrom(src =>
                src.Apartment != null ? src.Apartment.Id : (int?)null));

       
        CreateMap<UserForCreateDTO, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Se maneja aparte
            .ForMember(dest => dest.Apartment, opt => opt.Ignore()) // Se maneja aparte
            .AfterMap(async (src, dest, context) =>
            {
               
            });
    }
}