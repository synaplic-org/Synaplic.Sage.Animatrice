using AutoMapper;
using Uni.Scan.Infrastructure.Models.Identity;
using Uni.Scan.Transfer.Responses.Identity;

namespace Uni.Scan.Infrastructure.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<RoleResponse, UniRole>().ReverseMap();
        }
    }
}