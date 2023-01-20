using AutoMapper;
using Uni.Scan.Transfer.Requests.Identity;
using Uni.Scan.Transfer.Responses.Identity;

namespace Uni.Scan.Client.Infrastructure.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<PermissionResponse, PermissionRequest>().ReverseMap();
            CreateMap<RoleClaimResponse, RoleClaimRequest>().ReverseMap();
        }
    }
}