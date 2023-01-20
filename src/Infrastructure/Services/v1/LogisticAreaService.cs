using Mapster;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uni.Scan.Application.Interfaces.Repositories;
using Uni.Scan.Application.Interfaces.Services;
using Uni.Scan.Infrastructure.Contexts;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.DataModel;

namespace Uni.Scan.Infrastructure.Services.v1
{
    public interface ILogisticAreaService
    {
        Task<IResult<LogisticAreaDTO>> GetLogisticArea(string id);
        Task<IResult<List<LogisticAreaDTO>>> GetLogisticAreasBySite(string siteID);
        Task<IResult<List<SiteDTO>>> GetSites();
        Task<IResult<List<LogisticAreaDTO>>> GetAllAreas();
    }

    public class LogisticAreaService : ILogisticAreaService
    {
        private readonly UniContext _uniContext;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly ICurrentUserService _currentUserService;


        public LogisticAreaService(IUnitOfWork<int> unitOfWork, UniContext uniContext, ICurrentUserService currentUserService)
        {
            _uniContext = uniContext;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<IResult<LogisticAreaDTO>> GetLogisticArea(string id)
        {
            var result = _uniContext.LogisticAreas.Where(m => m.LogisticAreaID == id).Single();
            if (result != null)
            {
                var material = result.Adapt<LogisticAreaDTO>();
                return await Result<LogisticAreaDTO>.SuccessAsync(material);
            }
            else
            {
                return await Result<LogisticAreaDTO>.FailAsync();
            }
        }

        public async Task<IResult<List<LogisticAreaDTO>>> GetLogisticAreasBySite(string siteID)
        {
            var result = _uniContext.LogisticAreas.Where(x => x.SiteID == siteID).ToList();
            if (result != null)
            {
                var logisticAreas = result.Adapt<List<LogisticAreaDTO>>();
                return await Result<List<LogisticAreaDTO>>.SuccessAsync(logisticAreas);
            }
            else
            {
                return await Result<List<LogisticAreaDTO>>.FailAsync();
            }
        }

        public async Task<IResult<List<SiteDTO>>> GetSites()
        {
            var result = _uniContext.LogisticAreas.Select(x => new SiteDTO { SiteID = x.SiteID, SiteName = x.SiteName }).Distinct().ToList();
            return await Result<List<SiteDTO>>.SuccessAsync(result);
        }

        public async Task<IResult<List<LogisticAreaDTO>>> GetAllAreas()
        {
            //var v = _currentUserService.UserId;
            var getAll = _uniContext.LogisticAreas.Where(x => x.SiteID == _currentUserService.SiteId).ToList().Adapt<List<LogisticAreaDTO>>();
            return await Result<List<LogisticAreaDTO>>.SuccessAsync(getAll);
        }
    }
}