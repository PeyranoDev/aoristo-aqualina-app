using AutoMapper;
using Common.Models.Responses;
using Data.Entities;
using System;

namespace Data.Models.Profiles
{
    public class VehicleProfile : Profile
    {
        public VehicleProfile()
        {
            CreateMap<Request, RequestForResponseDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.RequestedBy, opt => opt.MapFrom(src => src.RequestedBy.Username));

            CreateMap<Vehicle, VehicleForResponseDTO>()
                .ForMember(dest => dest.Requests, opt => opt.MapFrom(src => src.Requests));
        }
    }
}
