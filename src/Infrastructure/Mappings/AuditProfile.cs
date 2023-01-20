using AutoMapper;
using Uni.Scan.Infrastructure.Models.Audit;
using Uni.Scan.Transfer.Responses.Audit;

namespace Uni.Scan.Infrastructure.Mappings
{
    public class AuditProfile : Profile
    {
        public AuditProfile()
        {
            CreateMap<AuditResponse, Audit>().ReverseMap();
        }
    }
}