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
    }
}