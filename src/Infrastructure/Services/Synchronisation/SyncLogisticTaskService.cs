using Infrastructure.Bydesign.OData.UniScan;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Soap.Byd.QuerySiteLogisticsTaskIn;
using System;
using System.Linq;
using System.Threading.Tasks;
using Uni.Scan.Domain.Entities;
using Uni.Scan.Infrastructure.ByDesign;
using Uni.Scan.Infrastructure.ByDesign.Service;
using Uni.Scan.Infrastructure.Contexts;
using Uni.Scan.Shared.Configurations;
using Uni.Scan.Shared.Extensions;
using Uni.Scan.Shared.Localizers;

namespace Uni.Scan.Infrastructure.Services.Synchronisation
{
    public class SyncLogisticTaskService
    {
        private readonly AppConfiguration _config;
        private readonly UniContext _db;
        private readonly IStringLocalizer<BackendLocalizer> _localizer;
        private readonly BydLogisticTaskService _bydLogisticTaskService;
        private readonly BydMaterialService _bydMaterialService;


        public SyncLogisticTaskService(IOptions<AppConfiguration> config, UniContext db,
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

        public async Task SyncAllTasks()
        {
            try
            {
                Serilog.Log.Information("Sync Logistic Task Odata Starting ...");
                var bydTasks = await _bydLogisticTaskService.GetOdataTasksAsync();
                foreach (var x in bydTasks)
                {
                    await UpdateFromOdataAsync(x, false, true, false);
                }

                await _db.SaveChangesAsync();
                Serilog.Log.Information("Sync Logistic Task Odata  success");
            }
            catch (Exception ex)
            {
                Serilog.Log.Error("Sync Logistic Task Odata  error", ex);
                throw;
            }


            try
            {
                Serilog.Log.Information("Sync Logistic Task Saop Starting ...");
                foreach (var task in _db.LogisticTasks.Where(x => x.OperationTypeCode == "0").ToList())
                {
                    await SyncTaskAsync(task.ObjectID, true);
                    Serilog.Log.Information($"{task.Id}  done .");
                }

                await _db.SaveChangesAsync();
                Serilog.Log.Information("Sync Logistic Task Saop succes");
            }
            catch (Exception ex)
            {
                Serilog.Log.Information("Sync Logistic Task Soap error", ex);
            }
        }

        public async Task<bool> SyncTaskAsync(string objectId, bool saveChanges = true)
        {
            var odataTask = await _bydLogisticTaskService.GetOdataTaskByOIDAsync(objectId);

            if (odataTask == null) return false;


            return await SyncTaskAsync(odataTask, saveChanges: saveChanges);
        }

        public async Task<bool> SyncTaskAsync(SiteLogisticsTask odataTask, bool saveChanges = true)
        {
            if (odataTask.ProcessingStatusCode.Equals("3"))
            {
                await UpdateFromOdataAsync(odataTask, saveChanges: saveChanges);
                return true;
            }


            var dbTask = _db.LogisticTasks.Include(o => o.LogisticTaskDetails)
                .FirstOrDefault(o => o.ObjectID == odataTask.ObjectID);


            var soapTask = await _bydLogisticTaskService.GetSoapTaskByIDAsync(odataTask.ID);

            if (soapTask != null)
            {
                if (dbTask == null)
                {
                    dbTask = new LogisticTask();
                    dbTask.Id = odataTask.ID;
                    dbTask.ObjectID = odataTask.ObjectID;
                    _db.LogisticTasks.Add(dbTask);
                }

                dbTask.BusinessTransactionDocumentReferenceID = soapTask.BusinessTransactionDocumentReferenceID?.Value;
                dbTask.OperationTypeCode = soapTask.OperationTypeCode?.Value;
                dbTask.ProcessingStatusCode = odataTask.ProcessingStatusCode;
                dbTask.ResponsibleId = odataTask.ResponsibleEmployee?.EmployeeID;
                dbTask.ResponsibleName = odataTask.ResponsibleEmployee?.BusinessPartnerFormattedName;
                dbTask.TaskFolderId = odataTask.LogisticsTaskFolder?.FirstOrDefault()?.ObjectID;
                dbTask.Notes = String.Empty;
                dbTask.PriorityCode = odataTask.PriorityCode;
                dbTask.RequestId = odataTask.LogisticsRequest?.FirstOrDefault()?.ID;
                dbTask.SiteId = odataTask.LogisticsTaskFolder?.FirstOrDefault()?.SiteID;
                dbTask.SiteName = odataTask.LogisticsTaskFolder?.FirstOrDefault()?.SiteName;
                dbTask.StartDate = odataTask.LatestExecutionStartDateTime?.DateTime ?? new DateTime(1900, 1, 1);
                dbTask.EndDate = odataTask.LatestExecutionEndDateTime?.DateTime ?? new DateTime(1900, 1, 1);
                dbTask.TaskUuid = odataTask.UUID.ToString();
                dbTask.ThirdPartyKey = String.Empty;
                dbTask.ThirdpartyName = String.Empty;
                dbTask.ProcessTypeCode = odataTask.LogisticsRequest?.FirstOrDefault()?.SiteLogisticsProcessTypeCode;
                dbTask.ProcessType = odataTask.LogisticsRequest?.FirstOrDefault()?.SiteLogisticsProcessTypeCodeText;
                dbTask.ItemsNumberValue = odataTask.LogisticsRequest?.FirstOrDefault()?.ItemsNumberValue ?? 0;

                dbTask.OperationType = dbTask.OperationTypeCode switch
                {
                    "1" => "Make",
                    "10" => "Supply",
                    "11" => "Put Away",
                    "12" => "Unload",
                    "13" => "Returns Put Away",
                    "14" => "Returns Unload",
                    "21" => "Pick",
                    "22" => "Load",
                    "23" => "Returns Pick",
                    "24" => "Returns Load",
                    "30" => "Replenish",
                    "31" => "Remove",
                    "8" => "Check",
                    _ => "UNKOWN"
                };


                if (soapTask.SiteLogisticsTaskReferencedObject?.SiteLogisticsLotOperationActivity?.MaterialOutput
                        .IsNullOrEmpty() == false)
                {
                    dbTask.ReferencedObjectUUID =
                        soapTask.SiteLogisticsTaskReferencedObject?.ReferencedObjectUUID.Value;
                    dbTask.OperationActivityUUID = soapTask.SiteLogisticsTaskReferencedObject
                        ?.SiteLogisticsLotOperationActivity?.SiteLogisticsLotOperationActivityUUID.Value;

                    if (soapTask.SiteLogisticsTaskReferencedObject?.SiteLogisticsLotOperationActivity?.MaterialOutput
                            .IsNullOrEmpty() == true) return true;


                    for (int i = 0;
                         i < soapTask.SiteLogisticsTaskReferencedObject?.SiteLogisticsLotOperationActivity
                             ?.MaterialOutput.Length;
                         i++)
                    {
                        var item = soapTask.SiteLogisticsTaskReferencedObject?.SiteLogisticsLotOperationActivity
                            ?.MaterialOutput[i];
                        var iteminput = soapTask.SiteLogisticsTaskReferencedObject?.SiteLogisticsLotOperationActivity
                            ?.MaterialInput[i];


                        var odetails = new Domain.Entities.LogisticTaskDetail()
                        {
                            TaskId = odataTask.ID,
                            OutputUUID = item.SiteLogisticsLotMaterialOutputUUID?.Value,
                            InputUUID = iteminput?.SiteLogisticsLotMaterialInputUUID?.Value,
                            ProductID = item.ProductID?.Value,
                            ProductDescription = item.ProductDescription?.Value,
                            PlanQuantity = item.PlanQuantity?.Value ?? 0,
                            PlanQuantityUnitCode = item.PlanQuantity?.unitCode,
                            OpenQuantity = item.OpenQuantity?.Value ?? 0,
                            OpenQuantityUnitCode = item.OpenQuantity?.unitCode,
                            ConfirmQuantity = item.ConfirmQuantity?.Value ?? 0,
                            ConfirmQuantityUnitCode = item.OpenQuantity?.unitCode,
                            TotalConfirmedQuantity = item.TotalConfirmedQuantity?.Value ?? 0,
                            TotalConfirmedQuantityUnitCode = item.TotalConfirmedQuantity?.unitCode,
                            IdentifiedStockID = item.IdentifiedStockID?.Value,
                            LineItemID = item.LineItemID,
                            PackageLogisticUnitUUID = item.LogisticPackageOutput?.LogisticUnitUUID.Value,
                            PackageLogisticUnitTotalConfirmedQuantity =
                                item.LogisticPackageOutput?.LogisticUnitTotalConfirmedQuantity?.Value ?? 0,
                            PackageLogisticUnitPlanQuantity =
                                item.LogisticPackageOutput?.LogisticUnitPlanQuantity?.Value ?? 0,
                            PackageLogisticUnitOpenQuantity =
                                item.LogisticPackageOutput?.LogisticUnitOpenQuantity?.Value ?? 0,
                            PackageLogisticUnitTotalConfirmedQuantityUnitCode = item.LogisticPackageOutput
                                ?.LogisticUnitTotalConfirmedQuantity?.unitCode,
                            PackageLogisticUnitPlanQuantityUnitCode = item.LogisticPackageOutput
                                ?.LogisticUnitTotalConfirmedQuantity?.unitCode,
                            PackageLogisticUnitOpenQuantityUnitCode =
                                item.LogisticPackageOutput?.LogisticUnitOpenQuantity?.unitCode,
                            MaterialDeviationStatusCode = item.MaterialDeviationStatusCode.ToString(),
                            MaterialDeviationStatusCodeSpecified = item.MaterialDeviationStatusCodeSpecified,
                            LogisticsDeviationReasonCode = item.LogisticsDeviationReasonCode?.Value,
                            LineItemIDSpecified = item.LineItemIDSpecified,
                            TargetLogisticsAreaID = item.TargetLogisticsAreaID,
                            SourceLogisticsAreaID = iteminput?.SourceLogisticsAreaID,
                            ProductSpecificationID = item.ProductSpecificationID?.Value,
                            RestrictedIndicator = item.RestrictedIndicator,
                            ProductRequirementSpecificationDescription =
                                item.ProductRequirementSpecificationDescription?.Value,
                            MaterialInspectionID = item.MaterialInspectionID?.Value
                        };


                        if (item.SerialNumberAssignment != null && item.SerialNumberAssignment.HasValues())
                        {
                            odetails.SerialNumberAssignments =
                                string.Join(";", item.SerialNumberAssignment?.Select(o => o.SerialID));
                        }

                        var oDataMaterial = await _bydMaterialService.GetMaterialAsync(odetails.ProductID);

                        var dbMaterial = _db.Materials.FirstOrDefault(x => x.ProductID == odetails.ProductID) ?? _db.Materials.Local.FirstOrDefault(x => x.ProductID == odetails.ProductID);

                        if (oDataMaterial != null)
                        {
                            if (dbMaterial == null)
                            {
                                dbMaterial = new Domain.Entities.Material()

                                {
                                    ObjectID = oDataMaterial.ObjectID
                                };
                                _db.Materials.Add(dbMaterial);
                            }

                            ;
                            dbMaterial.ObjectID = oDataMaterial.ObjectID;
                            dbMaterial.Description_KUT = oDataMaterial.Material.Description_KUT;
                            dbMaterial.InternalID = oDataMaterial.Material.InternalID;
                            dbMaterial.ProductID = oDataMaterial.ProductID;
                            dbMaterial.ProductIdentifierTypeCode = oDataMaterial.Material.IdentifiedStockTypeCode;
                            dbMaterial.ProductIdentifierTypeCodeText = oDataMaterial.Material.IdentifiedStockTypeCodeText;
                            dbMaterial.SerialIdentifierAssignmentProfileCode = oDataMaterial.Material.SerialIdentifierAssignmentProfileCode;
                            dbMaterial.SerialIdentifierAssignmentProfileCodeText = oDataMaterial.Material.SerialIdentifierAssignmentProfileCodeText;
                            dbMaterial.ProductValuationLevelTypeCode = oDataMaterial.Material.ProductValuationLevelTypeCode;
                            dbMaterial.ProductValuationLevelTypeCodeText = oDataMaterial.Material.ProductValuationLevelTypeCodeText;
                            dbMaterial.ProductTypeCode = oDataMaterial.ProductTypeCode;
                            dbMaterial.ProductTypeCodeText = oDataMaterial.ProductTypeCodeText;
                            dbMaterial.BaseMeasureUnitCode = oDataMaterial.Material.BaseMeasureUnitCode;
                            //task detail line
                            odetails.ProductDescription = dbMaterial.Description_KUT;
                        }


                        var dbdetails =
                            _db.LogisticTaskDetails.FirstOrDefault(x => x.OutputUUID == odetails.OutputUUID);
                        if (dbdetails != null)
                        {
                            _db.Entry(dbdetails).CurrentValues.SetValues(odetails);
                        }
                        else
                        {
                            dbTask.LogisticTaskDetails.Add(odetails);
                        }
                    }
                }

                if (saveChanges)
                {
                    await _db.SaveChangesAsync();
                }
            }

            return true;
        }

        public bool SyncTaskInBackgroundAsync(string objectId)
        {
            try
            {
                Hangfire.BackgroundJob.Enqueue<SyncLogisticTaskService>(x => x.SyncTaskAsync(objectId, true));
                return true;
            }
            catch (Exception e)
            {
                Serilog.Log.Error("Sync Task In Background   error", e);
            }

            return false;
        }

        public async Task UpdateFromOdataAsync(SiteLogisticsTask x, bool markUnknown = false, bool addIfNotExist = false, bool saveChanges = true)
        {
            var task = _db.LogisticTasks.FirstOrDefault(o => o.ObjectID == x.ObjectID);
            if (task == null)
            {
                if (addIfNotExist)
                {
                    task = new LogisticTask() { ObjectID = x.ObjectID, Id = x.ID, OperationTypeCode = "0", OperationType = "UNKOWN" };
                    _db.LogisticTasks.Add(task);
                }
                else
                {
                    return;
                }
            }
            else
            {
            }

            task.ProcessingStatusCode = x.ProcessingStatusCode;
            task.ResponsibleId = x.ResponsibleEmployee?.EmployeeID;
            task.ResponsibleName = x.ResponsibleEmployee?.BusinessPartnerFormattedName;
            task.TaskFolderId = x.LogisticsTaskFolder?.FirstOrDefault()?.ObjectID;
            task.Notes = String.Empty;
            task.PriorityCode = x.PriorityCode;
            task.RequestId = x.LogisticsRequest?.FirstOrDefault()?.ID;
            task.SiteId = x.LogisticsTaskFolder?.FirstOrDefault()?.SiteID;
            task.SiteName = x.LogisticsTaskFolder?.FirstOrDefault()?.SiteName;
            task.StartDate = x.LatestExecutionStartDateTime?.DateTime ?? new DateTime(1900, 1, 1);
            task.EndDate = x.LatestExecutionEndDateTime?.DateTime ?? new DateTime(1900, 1, 1);
            task.TaskUuid = x.UUID.ToString();
            task.ObjectID = x.ObjectID;
            task.ThirdPartyKey = String.Empty;
            task.ThirdpartyName = String.Empty;
            task.ProcessTypeCode = x.LogisticsRequest?.FirstOrDefault()?.SiteLogisticsProcessTypeCode;
            task.ProcessType = x.LogisticsRequest?.FirstOrDefault()?.SiteLogisticsProcessTypeCodeText;
            task.ItemsNumberValue = x.LogisticsRequest?.FirstOrDefault()?.ItemsNumberValue ?? 0;
            if (markUnknown)
            {
                task.OperationType = "UNKOWN";
                task.OperationTypeCode = "0";
                var details = _db.LogisticTaskDetails.Where(o => o.TaskId == x.ID).ToList();
                if (!details.IsNullOrEmpty()) _db.RemoveRange(details);
            }

            if (saveChanges)
            {
                await _db.SaveChangesAsync();
            }
        }
    }
}