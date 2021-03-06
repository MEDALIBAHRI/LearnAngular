using System;
using System.Linq;
using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles :Profile
    {
     public AutoMapperProfiles()
     {
         CreateMap<AppUser, MemberDTO>()
                .ForMember(dest => dest.PhotoUrl, 
                opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x=>x.IsMain).Url))
                .ForMember(dest => dest.Age,
                 opt => opt.MapFrom(src => src.DateOfBirth.GetAge()));

          CreateMap<Photo, PhotoDTO>();
          CreateMap<MemberUpdateDTO, AppUser>();
          CreateMap<RegisterDTO, AppUser>();
          CreateMap<AppUser, LikeDto>()
                .ForMember(dest => dest.PhotoUrl, 
                opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x=>x.IsMain).Url))
                .ForMember(dest => dest.Age,
                 opt => opt.MapFrom(src => src.DateOfBirth.GetAge()));
          CreateMap<Messages, MessageDto>()
                .ForMember(dest => dest.SenderPhotoUrl, 
                opt => opt.MapFrom(src =>src.Sender.Photos.FirstOrDefault(x=>x.IsMain == true).Url))
                .ForMember(dest => dest.RecipientPhotoUrl, 
                opt => opt.MapFrom(src =>src.Recipient.Photos.FirstOrDefault(x=>x.IsMain == true).Url));
          CreateMap<DateTime, DateTime>().ConvertUsing(d=> DateTime.SpecifyKind(d, DateTimeKind.Utc) );
          CreateMap<DateTime?, DateTime?>().ConvertUsing(d=>d.HasValue ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : null);
     }   
    }
}