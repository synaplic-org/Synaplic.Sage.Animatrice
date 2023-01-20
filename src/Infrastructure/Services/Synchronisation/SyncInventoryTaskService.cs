using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using Uni.Scan.Domain.Entities;
using Uni.Scan.Infrastructure.ByDesign.Service;
using Uni.Scan.Infrastructure.Contexts;
using Uni.Scan.Shared.Configurations;
using Uni.Scan.Shared.Extensions;
using Uni.Scan.Shared.Localizers;

namespace Uni.Scan.Infrastructure.Services.Synchronisation
{
    public class SyncInventoryTaskService
    {
        private readonly AppConfiguration _config;
        private readonly UniContext _db;
        private readonly IStringLocalizer<BackendLocalizer> _localizer;
        private readonly BydInventoryService _bydInventoryService;
        private readonly BydMaterialService _bydMaterialService;


        public SyncInventoryTaskService(IOptions<AppConfiguration> config, UniContext db,
            BydInventoryService bydInventoryService,
            BydMaterialService bydMaterialService,
            IStringLocalizer<BackendLocalizer> localizer)
        {
            _config = config.Value;
            _db = db;
            _localizer = localizer;
            _bydInventoryService = bydInventoryService;
            _bydMaterialService = bydMaterialService;
        }

        public async Task SyncAllTasks()
        {
            Serilog.Log.Information("Sync Odata Inventory Tasks starting");

            try
            {
                var bydTasks = await _bydInventoryService.GetPhysicalInventoryTasksAsync();
                foreach (var x in bydTasks)
                {
                    var operations = x.PhysicalInventoryTaskReferencedObject?.FirstOrDefault()
                        ?.PhysicalInventoryCountPhysicalInventoryCountOperationActivity
                        ?.PhysicalInventoryCountOperationActivityCountInventory;

                    if (operations.IsNullOrEmpty())
                    {
                        continue;
                    }

                    var task = await _db.InventoryTasks.FirstOrDefaultAsync(o => o.ObjectID == x.ObjectID);
                    if (task == null)
                    {
                        task = new InventoryTask();

                        task.Number = x.ID;
                        task.ProcessingStatus = x.ProcessingStatusCode;
                        task.ResponsibleId = x.ResponsibleEmployeeID;
                        task.Notes = string.Empty;
                        task.PriorityCode = x.PriorityCode;
                        task.EndDateTime = x.EndDateTime?.DateTime ?? new DateTime(1900, 1, 1);
                        task.CreationDateTime = x.CreationDateTime?.DateTime ?? new DateTime(1900, 1, 1);

                        if (operations.IsNullOrEmpty() == false)
                        {
                            task.SiteId = operations.First().SiteID;
                            task.SiteName = operations.First().SiteName;

                            foreach (var op in operations)
                            {
                                var operation =
                                    await _db.InventoryTaskOperations.FirstOrDefaultAsync(
                                        o => o.ObjectID == op.ObjectID);
                                if (operation == null)
                                {
                                    operation = new InventoryTaskOperation();

                                    operation.ParentObjectID = op.ParentObjectID;
                                    operation.SiteID = op.SiteID;
                                    operation.LogisticsAreaUUID = op.LogisticsAreaUUID.ToString();
                                    operation.ApprovalProcessingStatusCode = op.ApprovalProcessingStatusCode;
                                    operation.CountLifeCycleStatusCode = op.CountLifeCycleStatusCode;
                                    operation.LogisticsAreaID = op.LogisticsAreaID;
                                    operation.InventoryManagedLocationID = op.InventoryManagedLocationID;
                                    operation.InventoryManagedLocationUUID = op.InventoryManagedLocationUUID.ToString();
                                    operation.LogisticsAreaTypeCode = op.LogisticsAreaTypeCode;
                                    operation.LogisticsAreaLifeCycleStatusCode = op.LogisticsAreaLifeCycleStatusCode;
                                    if (string.IsNullOrEmpty(operation.ObjectID))
                                    {
                                        operation.ObjectID = op.ObjectID;
                                        task.InventoryTaskOperations.Add(operation);
                                    }

                                    foreach (var it in op.PhysicalInventoryCountOperationActivityInventoryItem)
                                    {
                                        var item = await _db.InventoryTaskItems.FirstOrDefaultAsync(o =>
                                            o.ObjectID == it.ObjectID);
                                        if (item == null)
                                        {
                                            item = new InventoryTaskItem();

                                            var oItemQuantity = it.PhysicalInventoryCountOperationActivityInventoryItemQuantity
                                                    ?.FirstOrDefault()
                                                ;

                                            item.TaskID = task.Number;
                                            item.CountCounterValue = it.CountCounterValue ?? 0;
                                            item.DeviationReasonCode = it.DeviationReasonCode;
                                            item.ForceSkippedIndicator = it.ForceSkippedIndicator ?? false;
                                            item.IncludeIndicator = it.IncludeIndicator ?? false;
                                            item.InventoryItemNumberValue = it.InventoryItemNumberValue ?? 0;
                                            item.ProductID = it.ProductID;
                                            item.LogisticPackageUUID = it.LogisticPackageUUID.ToString();
                                            item.ApprovalResultStatusCode = it.ApprovalResultStatusCode;
                                            item.CountApprovalStatusCode = it.CountApprovalStatusCode;
                                            item.IdentifiedStockID = it.IdentifiedStockID;
                                            item.QuantitObjectID = oItemQuantity?.ObjectID;
                                            item.ApprovalDiscrepancyPercent = oItemQuantity?.ApprovalDiscrepancyPercent ?? 0;
                                            item.CountedQuantityTypeCode = oItemQuantity?.CountedQuantityTypeCode;
                                            item.ZeroCountedQuantityConfirmedIndicator = oItemQuantity?.ZeroCountedQuantityConfirmedIndicator ?? false;
                                            item.CountedQuantity = oItemQuantity?.CountedQuantity ?? 0;
                                            item.CountedQuantityUnitCode = oItemQuantity?.CountedQuantityUnitCode;
                                            item.BookInventoryQuantity = oItemQuantity?.BookInventoryQuantity ?? 0;


                                            if (string.IsNullOrEmpty(item.ObjectID))
                                            {
                                                item.ObjectID = it.ObjectID;
                                                operation.InventoryTaskItems.Add(item);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            task.ProcessingStatus = x.ProcessingStatusCode;
                            task.ResponsibleId = x.ResponsibleEmployeeID;
                        }


                        if (string.IsNullOrEmpty(task.ObjectID))
                        {
                            task.ObjectID = x.ObjectID;
                            _db.Add(task);
                        }
                    }
                }


                await _db.SaveChangesAsync();
                Serilog.Log.Information("Sync Odata Inventory Tasks success");
            }
            catch (Exception ex)
            {
                Serilog.Log.Error("Sync Odata Inventory Tasks  error", ex);
                throw;
            }
        }
    }
}