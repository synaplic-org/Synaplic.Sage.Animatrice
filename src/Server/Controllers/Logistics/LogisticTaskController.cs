using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Uni.Scan.Infrastructure.Services.Logistics;
using Uni.Scan.Infrastructure.Services.Synchronisation;
using Uni.Scan.Infrastructure.Services.v1;
using Uni.Scan.Server.Controllers.Inventory;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.DataModel;
using Uni.Scan.Transfer.DataModel.LogisticTask;

namespace Uni.Scan.Server.Controllers.Logistics
{
    public class LogisticTaskController : BaseApiController<LogisticTaskController>
    {
        private readonly ILogisticTasDetailService _logisticTasDetailService;
        private readonly ISyncService _syncService;
        private readonly ILogisticTaskService _logisticTaskService;

        public LogisticTaskController(
            ILogisticTaskService logisticTaskService,
            ILogisticTasDetailService logisticTasDetailService,
            ISyncService syncService)
        {
            _logisticTaskService = logisticTaskService;
            _logisticTasDetailService = logisticTasDetailService;
            _syncService = syncService;
        }


        [HttpGet(nameof(GetAllowedTasks) + "/{operationType}")]
        public async Task<IResult<List<LogisticTaskDTO2>>> GetAllowedTasks(string operationType)
        {
            return await _logisticTaskService.GetAllowedTasks(operationType);
        }

        [HttpGet(nameof(GetTaskDetail) + "/{objectID}")]
        public async Task<IResult<LogisticTaskDTO2>> GetTaskDetail(string objectID)
        {
            return await _logisticTaskService.GetTaskDetail(objectID);
        }

        [HttpGet(nameof(GetAnyTaskDetail) + "/{objectID}")]
        public async Task<IResult<LogisticTaskDTO2>> GetAnyTaskDetail(string objectID)
        {
            return await _logisticTaskService.GetTaskDetail(objectID, false);
        }


        [HttpPost(nameof(SetTaskResponsible))]
        public async Task<IResult<bool>> SetTaskResponsible(string objectID)
        {
            return await _logisticTaskService.SetTaskResponsible(objectID);
        }

        [HttpPost(nameof(RemoveTaskResponsible))]
        public async Task<IResult<bool>> RemoveTaskResponsible(string objectID)
        {
            return await _logisticTaskService.RemoveTaskResponsible(objectID);
        }


        [HttpPost(nameof(SaveTask))]
        public async Task<IResult<LogisticTaskDTO2>> SaveTask(LogisticTaskDTO2 logisticTaskDto)
        {
            return await _logisticTaskService.SaveTask(logisticTaskDto);
        }
    }
}