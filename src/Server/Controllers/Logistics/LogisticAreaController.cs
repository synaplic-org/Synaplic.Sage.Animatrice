using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Uni.Scan.Infrastructure.Services.v1;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.DataModel;

namespace Uni.Scan.Server.Controllers.Logistics
{
    public class LogisticAreaController : BaseApiController<LogisticAreaController>
    {
        private readonly ILogisticAreaService _logisticAreaService;

        public LogisticAreaController(ILogisticAreaService logisticAreaService)
        {
            _logisticAreaService = logisticAreaService;
        }

        [HttpGet(nameof(GetLogisticArea)+"/{id}")]
        public async Task<IResult<LogisticAreaDTO>> GetLogisticArea(string id)
        {
            return await _logisticAreaService.GetLogisticArea(id);
        }

        [HttpGet(nameof(GetLogisticAreasBySite) + "/{siteId}")]
        public async Task<IResult<List<LogisticAreaDTO>>> GetLogisticAreasBySite(string siteId)
        {
            return await _logisticAreaService.GetLogisticAreasBySite(siteId);
        }

        [HttpGet(nameof(GetSites))]
        public async Task<IResult<List<SiteDTO>>> GetSites()
        {
            return await _logisticAreaService.GetSites();
        }
        [HttpGet(nameof(GetAllAreas))]
        public async Task<IResult<List<LogisticAreaDTO>>> GetAllAreas()
        {
            return await _logisticAreaService.GetAllAreas();
        }
    }
}