using AutoMapper;
using Common.Models.Requests;
using Common.Models.Responses;
using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Profiles
{
    public class TowerProfile : Profile
    {
        public TowerProfile()
        {
            CreateMap<Tower, TowerForUserResponseDTO>();
            
            CreateMap<TowerForCreateDTO, Tower>();

            CreateMap<TowerForUpdateDTO, Tower>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
