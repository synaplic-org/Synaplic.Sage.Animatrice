using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Uni.Scan.Infrastructure.Contexts;
using Uni.Scan.Infrastructure.Services.Synchronisation;
using Uni.Scan.Infrastructure.Services.v1;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.DataModel;

namespace Uni.Scan.Server.Controllers.Logistics
{
    public class LogisticsController : BaseApiController<LogisticsController>
    {
        private readonly ISyncService _syncService;
        private readonly UniContext _uniContext;

        public LogisticsController(UniContext uniContext,
            ISyncService syncService)
        {
            _syncService = syncService;
            _uniContext = uniContext;
        }

        //[HttpGet(nameof(GetMyTasks) + "/{operationType}")]
        //public async Task<IResult<List<LogisticTaskDTO>>> GetMyTasks(string operationType)
        //{
        //    return await _logisticService.GetMyTasks(operationType);
        //}

        //[HttpGet(nameof(GetTasksBy) + "/{searchString}")]
        //public async Task<IResult<List<LogisticTaskDTO>>> GetTasksBy(string searchString)
        //{
        //    return await _logisticService.GetTasksBy(searchString);
        //}

        //[HttpGet(nameof(GetUnassignedTasks) + "/{operationType}")]
        //public async Task<IResult<List<LogisticTaskDTO>>> GetUnassignedTasks(string operationType)
        //{
        //    return await _logisticService.GetUnassignedTasks(operationType);
        //}


        [HttpGet(nameof(GetTaskDetails) + "/{taskId}")]
        public async Task<IResult<LogisticTaskDTO>> GetTaskDetails(string taskId)
        {
            try
            {
                var task = _uniContext.LogisticTasks.FirstOrDefault(e => e.Id == taskId);

                if (task == null) return await Result<LogisticTaskDTO>.FailAsync("Task not found");

                var taskDetails = _uniContext.LogisticTaskDetails.Where(e => e.TaskId == taskId).ToList();
                var matrialsID = taskDetails.Select(x => x.ProductID).Distinct().ToList();
                var matrials = _uniContext.Materials.Where(x => matrialsID.Contains(x.ProductID)).ToList();
                var areas = _uniContext.LogisticAreas.Where(o => o.SiteID == task.SiteId).ToList();
                var labels = _uniContext.LogisticTaskLabels.Where(x => x.TaskId.Equals(taskId)).ToList();
                var dto = task.Adapt<LogisticTaskDTO>();

                dto.Labels = labels.Adapt<List<LogisticTaskLabelDTO>>();
                dto.LogisticTaskDetails = taskDetails.Adapt<List<LogisticTaskDetailDTO>>();
                dto.Martials = matrials.Adapt<List<MaterialDTO>>();
                dto.LogisticAreas = areas.Adapt<List<LogisticAreaDTO>>();
                return await Result<LogisticTaskDTO>.SuccessAsync(dto);
            }
            catch (Exception e)
            {
                return await Result<LogisticTaskDTO>.FailAsync(e.Message);
            }

             
        }

        //[HttpGet(nameof(GlobalSyncronization))]
        //public async Task<IResult<bool>> GlobalSyncronization()
        //{
        //    var executed = _syncService.ExecuteJobNow();
        //    return await Result<bool>.SuccessAsync(executed);
        //}
    }
}