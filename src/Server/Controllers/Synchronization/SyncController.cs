using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Uni.Scan.Server.Controllers.Inventory;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Infrastructure.Services.Synchronisation;
using Hangfire.Storage;
using Hangfire;

namespace Uni.Scan.Server.Controllers.Synchronization
{
    public class SyncController : BaseApiController<InventoryController>
    {
        private readonly ISyncService _syncService;

        public SyncController(ISyncService service)
        {
            _syncService = service;
        }


        [HttpPost(nameof(StartSync))]
        public async Task<IResult<string>> StartSync()
        {
            try
            {
                var executed = _syncService.ExecuteJobNow();
                return await Result<string>.SuccessAsync(data:executed);
            }
            catch (Exception e)
            {
                return await Result<string>.FailAsync(e.Message);
            }
        }


        [HttpGet(nameof(GetJobState)+ "/{jobId}")]
        public async Task<IResult<string>> GetJobState(string jobId)
        {
            try
            {
                IStorageConnection connection = JobStorage.Current.GetConnection();
                JobData jobData = connection.GetJobData(jobId);
                string stateName = jobData.State;
                return await Result<string>.SuccessAsync(data:stateName);
            }
            catch (Exception e)
            {
                return await Result<string>.FailAsync(e.Message);
            }
        }
    }
}