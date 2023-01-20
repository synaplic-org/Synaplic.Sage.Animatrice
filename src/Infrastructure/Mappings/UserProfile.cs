using AutoMapper;
using Uni.Scan.Infrastructure.Models.Identity;
using Uni.Scan.Transfer.Responses.Identity;

namespace Uni.Scan.Infrastructure.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserResponse, UniUser>().ReverseMap();
            //CreateMap<ChatUserResponse, UniUser>().ReverseMap()
            //    .ForMember(dest => dest.EmailAddress, source => source.MapFrom(source => source.Email)); //Specific Mapping
        }
    }
}