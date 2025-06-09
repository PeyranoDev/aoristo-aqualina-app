using AutoMapper;
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
                    Identifier = src.Apartment.Identifier,

                    Tower = src.Apartment.Tower == null ? null : new TowerForUserResponseDTO
                    {
                        Id = src.Apartment.Tower.Id,
                        Name = src.Apartment.Tower.Name,
                        Description = src.Apartment.Tower.Description
                    }
                }))
            .ForMember(dest => dest.AssociatedTowers, opt => opt.MapFrom(src =>
                src.UserTowers.Select(ut => new TowerForUserResponseDTO
                {
                    Id = ut.Tower.Id,
                    Name = ut.Tower.Name,
                    Description = ut.Tower.Description 
                }).ToList()
            ));

        CreateMap<UserForCreateDTO, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.Apartment, opt => opt.Ignore());

        CreateMap<UserForUpdateDTO, User>()
        .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
        .ForMember(dest => dest.Role, opt => opt.Ignore())
        .ForMember(dest => dest.Apartment, opt => opt.Ignore())
        .ForMember(dest => dest.ApartmentId, opt => opt.MapFrom(src => src.Apartment_Id))
        .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone_Number))
        .ForAllMembers(opt =>
            opt.Condition((src, dest, srcMember) => srcMember != null))
        ;
    }
}