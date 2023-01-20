using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Soap.Byd.QuerySiteLogisticsTaskIn;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Uni.Scan.Domain.Entities;
using Uni.Scan.Infrastructure.ByDesign.Service;
using Uni.Scan.Infrastructure.Contexts;
using Uni.Scan.Shared.Configurations;
using Uni.Scan.Shared.Localizers;

namespace Uni.Scan.Infrastructure.Services.Synchronisation
{
    public class SyncMasterDataService
    {
        private readonly AppConfiguration _config;
        private readonly UniContext _db;
        private readonly IStringLocalizer<BackendLocalizer> _localizer;
        private readonly BydLogisticTaskService _bydLogisticTaskService;
        private readonly BydMaterialService _bydMaterialService;


        public SyncMasterDataService(IOptions<AppConfiguration> config, UniContext db,
            BydLogisticTaskService bydLogisticTaskService,
            BydMaterialService bydMaterialService,
            IStringLocalizer<BackendLocalizer> localizer)
        {
            _config = config.Value;
            _db = db;
            _localizer = localizer;
            _bydLogisticTaskService = bydLogisticTaskService;
            _bydMaterialService = bydMaterialService;
        }

        public async Task SyncLogisticsAreas()
        {
            var nbrtoskip = await _db.LogisticAreas.CountAsync();


            Serilog.Log.Information("Sync Logistics Area Odata Starting ...");
            var results = await _bydLogisticTaskService.GetLogisticsAreasAsync(nbrtoskip);
            var i = 0;
            foreach (var item in results)
            {
                var area = await _db.LogisticAreas.FirstOrDefaultAsync(x => x.ObjectID == item.ObjectID);
                if (area == null) area = new LogisticArea();


                area.LogisticAreaID = item.ID;
                area.InventoryManagedLocationID = item.InventoryManagedLocationID;
                area.InventoryManagedLocationUUID = item.InventoryManagedLocationUUID?.ToString();
                area.SiteID = item.SiteID;
                area.SiteUUID = item.SiteUUID?.ToString();
                area.TypeCode = item.TypeCode;
                area.TypeCodeText = item.TypeCodeText;
                area.LogisticAreaUUID = item.UUID?.ToString();
                area.InventoryManagedLocationIndicator =
                    item.InventoryManagedLocationIndicator.GetValueOrDefault(false);
                area.SiteName = item.SiteName;
                area.LifeCycleStatusCode = item.LifeCycleStatusCode;
                area.LifeCycleStatusCodeText = item.LifeCycleStatusCodeText;
                area.ParentLocationUUID = item.ParentLocationUUID?.ToString();
                if (string.IsNullOrWhiteSpace(area.ObjectID))
                {
                    area.ObjectID = item.ObjectID;
                    _db.Add(area);
                }

                i++;
                if (i % 250 == 0)
                {
                    await _db.SaveChangesAsync();
                }
            }

            try
            {
                await _db.SaveChangesAsync();
                Serilog.Log.Information("Sync Logistics Area Odata succes");
            }
            catch (Exception ex)
            {
                Serilog.Log.Information("Sync Logistics Area Odata  error", ex);
            }
        }
    }
}