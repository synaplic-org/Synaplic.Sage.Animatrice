using AutoMapper;
using Uni.Scan.Infrastructure.Models.Identity;
using Uni.Scan.Transfer.Requests.Identity;
using Uni.Scan.Transfer.Responses.Identity;

namespace Uni.Scan.Infrastructure.Mappings
{
    public class RoleClaimProfile : Profile
    {
        public RoleClaimProfile()
        {
            CreateMap<RoleClaimResponse, UniRoleClaim>()
                .ForMember(nameof(UniRoleClaim.ClaimType), opt => opt.MapFrom(c => c.Type))
                .ForMember(nameof(UniRoleClaim.ClaimValue), opt => opt.MapFrom(c => c.Value))
                .ReverseMap();

            CreateMap<RoleClaimRequest, UniRoleClaim>()
                .ForMember(nameof(UniRoleClaim.ClaimType), opt => opt.MapFrom(c => c.Type))
                .ForMember(nameof(UniRoleClaim.ClaimValue), opt => opt.MapFrom(c => c.Value))
                .ReverseMap();
        }
    }
}