using AutoMapper;
using DatingApp.API.DTOs;
using DatingApp.API.Models;
using System.Linq;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            /*
            Map data to set output (akin to selecting data from a record to be shown in popup)
            Param1 - Source Class
            Param2 - Destination Class
            */
            CreateMap<User,UserForListDTO>()
                /*
                 For a specific member,
                 + Defining the PhotoURL and where it is sourced from.
                 + Defining the Age and where it is sourced from and then using an extension that calculates the age (using Helpers\Extensions.cs).
                */ 
                .ForMember(dest => 
                    dest.PhotoUrl, opt =>
                    opt.MapFrom(src => 
                    src.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => 
                    dest.Age, opt => 
                    opt.MapFrom(src => 
                    src.DateOfBirth.CalculateAge()));
            CreateMap<User,UserForDetailedDTO>()
                // For a specific member, defining the PhotoURL and where it is sourced from.
                .ForMember(dest => 
                    dest.PhotoUrl, opt =>
                    opt.MapFrom(src => 
                    src.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => 
                    dest.Age, opt => 
                    opt.MapFrom(src => 
                    src.DateOfBirth.CalculateAge()));
            CreateMap<Photo,PhotosForDetailedDTO>();
            CreateMap<UserForUpdateDTO,User>();
            
        }
    }
}