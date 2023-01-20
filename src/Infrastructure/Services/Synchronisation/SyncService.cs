using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uni.Scan.Domain.Entities;
using Uni.Scan.Infrastructure.ByDesign.Service;
using Uni.Scan.Infrastructure.Contexts;
using Uni.Scan.Shared.Extensions;
using Uni.Scan.Shared.Localizers;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.DataModel;
using Log = Serilog.Log;

namespace Uni.Scan.Infrastructure.Services.Synchronisation
{
    public interface ISyncService
    {
        string ExecuteJobNow();

        bool RemoveRecurentJob();

        bool StartRecurringJob();

        Task SyncAll();
    }

    public class SyncService : ISyncService
    {
        private readonly IStringLocalizer<BackendLocalizer> _localizer;
        private readonly BydStockService _bydStockService;
        private readonly SyncLogisticTaskService _syncLogisticTaskService;
        private readonly SyncInventoryTaskService _syncInventoryTaskService;
        private readonly SyncMasterDataService _syncMasterDataService;

        public SyncService(
            IStringLocalizer<BackendLocalizer> localizer,
            SyncLogisticTaskService syncLogisticTaskService,
            SyncInventoryTaskService syncInventoryTaskService,
            SyncMasterDataService syncMasterDataService)
        {
            _localizer = localizer;
            _syncLogisticTaskService = syncLogisticTaskService;
            _syncInventoryTaskService = syncInventoryTaskService;
            _syncMasterDataService = syncMasterDataService;
        }

        public bool StartRecurringJob()
        {
            RecurringJob.RemoveIfExists(nameof(SyncService) + "." + nameof(SyncAll));
            RecurringJob.AddOrUpdate<SyncService>(x => x.SyncAll(), "*/15 * * * *");
            return true;
        }


        [DisableConcurrentExecution(timeoutInSeconds: 10 * 60)]
        [AutomaticRetry(Attempts = 1)]
        public async Task SyncAll()
        {
            await _syncInventoryTaskService.SyncAllTasks();
            await _syncLogisticTaskService.SyncAllTasks();
            await _syncMasterDataService.SyncLogisticsAreas();
        }


        public bool RemoveRecurentJob()
        {
            try
            {
                RecurringJob.RemoveIfExists(nameof(SyncService) + "." + nameof(SyncAll));
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("RemoveRecurentJob  failed!", ex);
            }
        }

        public string ExecuteJobNow()
        {
            try
            {
                var uid = BackgroundJob.Enqueue<ISyncService>(x => x.SyncAll());
                return uid;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Executing Job Now failed!", ex);
            }
        }
    }
}