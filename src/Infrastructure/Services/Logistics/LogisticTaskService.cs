using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uni.Scan.Application.Interfaces.Services;
using Uni.Scan.Application.Interfaces.Services.Identity;
using Uni.Scan.Infrastructure.ByDesign.Service;
using Uni.Scan.Infrastructure.Contexts;
using Uni.Scan.Infrastructure.Services.Synchronisation;
using Uni.Scan.Shared.Localizers;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.DataModel.LogisticTask;
using Uni.Scan.Transfer.DataModel;
using Soap.Byd.QuerySiteLogisticsTaskIn;
using MudBlazor;

namespace Uni.Scan.Infrastructure.Services.Logistics
{
    public interface ILogisticTaskService
    {
        Task<IResult<List<LogisticTaskDTO2>>> GetAllowedTasks(string processType);
        Task<IResult<LogisticTaskDTO2>> GetTaskDetail(string objectID, bool mytaskOnly = true);
        Task<IResult<bool>> SetTaskResponsible(string objectId);
        Task<IResult<bool>> RemoveTaskResponsible(string objectId);
        Task<IResult<LogisticTaskDTO2>> SaveTask(LogisticTaskDTO2 logisticTaskDto);
    }

    public class LogisticTaskService : ILogisticTaskService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly UniContext _uniContext;
        private readonly IStringLocalizer<BackendLocalizer> _localizer;
        private readonly BydLogisticTaskService _bydLogisticTaskService;
        private readonly SyncLogisticTaskService _syncLogisticTaskService;

        public LogisticTaskService(ICurrentUserService currentUserService,
            UniContext uniContext, BydLogisticTaskService bydLogisticTaskService,
            SyncLogisticTaskService syncLogisticTaskService,
            IStringLocalizer<BackendLocalizer> localizer)
        {
            _currentUserService = currentUserService;
            _uniContext = uniContext;
            _localizer = localizer;
            _bydLogisticTaskService = bydLogisticTaskService;
            _syncLogisticTaskService = syncLogisticTaskService;
        }


        public async Task<IResult<List<LogisticTaskDTO2>>> GetAllowedTasks(string processType)
        {
            var myresult = _uniContext.LogisticTasks.Where(e => e.ProcessType.Equals(processType)
                                                                && e.ResponsibleId.Equals(
                                                                    _currentUserService.EmployeeID)
                                                                && !e.ProcessingStatusCode.Equals("3")).ToList();

            var unissignedresult = _uniContext.LogisticTasks.Where(e => e.ProcessType.Equals(processType) && !e.ProcessingStatusCode.Equals("3")
                                                                                                          && (e.ResponsibleId == null || e.ResponsibleId == "")
                                                                                                          && (e.SiteId.Equals(_currentUserService.SiteId) || _currentUserService.SiteId.Equals("*"))).ToList();

            var result = myresult.Union(unissignedresult).Adapt<List<LogisticTaskDTO2>>();

            return await Result<List<LogisticTaskDTO2>>.SuccessAsync(result);
        }

        public async Task<IResult<LogisticTaskDTO2>> GetTaskDetail(string objectID, bool mytaskOnly = true)
        {
            var dbTask = _uniContext.LogisticTasks.Where(e => e.ObjectID.Equals(objectID))
                .Include(s => s.LogisticTaskDetails).FirstOrDefault();
            if (dbTask.OperationTypeCode.Equals("0"))
            {
                bool syncResult = await _syncLogisticTaskService.SyncTaskAsync(objectID);
                if (!syncResult)
                {
                    throw new ApplicationException("Erreur de synchronisation depuis SAP");
                }

                dbTask = _uniContext.LogisticTasks.Where(e => e.ObjectID.Equals(objectID))
                    .Include(s => s.LogisticTaskDetails).FirstOrDefault();
            }

            if (mytaskOnly && !string.IsNullOrWhiteSpace(dbTask.ResponsibleId)
                           && !dbTask.ResponsibleId.Equals(_currentUserService.EmployeeID)
                           && !_currentUserService.EmployeeID.Equals("*"))
            {
                return await Result<LogisticTaskDTO2>.SuccessAsync("Tache non autorisée !");
            }

            var result = dbTask.Adapt<LogisticTaskDTO2>();
            var Items = dbTask.LogisticTaskDetails.Select(o => new
            {
                LineItemID = o.LineItemID,
                ProductID = o.ProductID,
                ProductDescription = o.ProductDescription,
                PlanQuantity = o.PlanQuantity,
                PlanQuantityUnitCode = o.PlanQuantityUnitCode
            }).Distinct().ToList();

            var lables = _uniContext.LogisticTaskLabels.Where(o => o.TaskId == dbTask.Id);

            foreach (var o in Items)
            {
                var item = new LogisticTaskItemDTO2()
                {
                    LineItemID = o.LineItemID,
                    ProductID = o.ProductID,
                    ProductDescription = o.ProductDescription,
                    PlanQuantity = o.PlanQuantity,
                    PlanQuantityUnitCode = o.PlanQuantityUnitCode
                };
                item.Details = dbTask.LogisticTaskDetails.Where(o => o.LineItemID == item.LineItemID)
                    .Adapt<List<LogisticTaskItemDetailDTO2>>();
                item.Lables = lables.Where(o => o.LineItemID == item.LineItemID)
                    .Adapt<List<LogisticTaskLabelDTO2>>();
                var mtr = _uniContext.Materials.FirstOrDefault(o => o.ProductID == item.ProductID);
                if (mtr != null)
                {
                    item.Material = mtr.Adapt<MaterialDTO>();
                }

                result.Items.Add(item);
            }

            return await Result<LogisticTaskDTO2>.SuccessAsync(result);
        }


        public async Task<IResult<bool>> SetTaskResponsible(string objectId)
        {
            if (string.IsNullOrWhiteSpace(_currentUserService.EmployeeID) || _currentUserService.EmployeeID == "*")
            {
                return await Result<bool>.FailAsync(_localizer["Votre Employee ID est non defini"]);
            }

            try
            {
                var bydTask = await _bydLogisticTaskService.GetOdataTaskByOIDAsync(objectId);
                if (bydTask == null)
                {
                    throw new ApplicationException("tache introuvable sur SAP");
                }

                if (bydTask.ProcessingStatusCode.Equals("3"))
                {
                    await _syncLogisticTaskService.UpdateFromOdataAsync(bydTask, true);
                    throw new ApplicationException("tache terminée sur SAP");
                }

                if (!string.IsNullOrWhiteSpace(bydTask.ResponsibleEmployeeID) &&
                    !bydTask.ResponsibleEmployeeID.Equals(_currentUserService.EmployeeID))
                {
                    await _syncLogisticTaskService.UpdateFromOdataAsync(bydTask);
                    throw new ApplicationException("La tache est déjà prise");
                }

                var setResult =
                    await _bydLogisticTaskService.SetLogisticsTaskResponsibleAsync(objectId,
                        _currentUserService.EmployeeID);
                if (!setResult)
                {
                    await _syncLogisticTaskService.UpdateFromOdataAsync(bydTask, true);
                    throw new ApplicationException("Erreur d'affectation sur SAP");
                }

                bool syncResult = await _syncLogisticTaskService.SyncTaskAsync(bydTask.ObjectID);
                if (!syncResult)
                {
                    await _syncLogisticTaskService.UpdateFromOdataAsync(bydTask, true);
                    throw new ApplicationException("Erreur de synchronisation depuis SAP");
                }

                return await Result<bool>.SuccessAsync(true);
            }
            catch (Exception e)
            {
                return await Result<bool>.FailAsync(_localizer[e.Message]);
            }
        }

        public async Task<IResult<bool>> RemoveTaskResponsible(string objectId)
        {
            try
            {
                var bydTask = await _bydLogisticTaskService.GetOdataTaskByOIDAsync(objectId);
                if (bydTask == null)
                {
                    throw new ApplicationException("tache introuvable sur SAP");
                }

                if (bydTask.ProcessingStatusCode.Equals("3"))
                {
                    await _syncLogisticTaskService.UpdateFromOdataAsync(bydTask, true);
                    throw new ApplicationException("tache terminée sur SAP");
                }

                if (string.IsNullOrWhiteSpace(bydTask.ResponsibleEmployeeID))
                {
                    await _syncLogisticTaskService.UpdateFromOdataAsync(bydTask);
                    return await Result<bool>.SuccessAsync(true);
                }

                if (!string.IsNullOrWhiteSpace(bydTask.ResponsibleEmployeeID) &&
                    !bydTask.ResponsibleEmployeeID.Equals(_currentUserService.EmployeeID))
                {
                    throw new ApplicationException("La tache est affectée à un autre employée");
                }

                var setResult = await _bydLogisticTaskService.RemoveTaskResponsibleAsync(objectId);
                if (!setResult) throw new ApplicationException("Erreur désaffectation sur SAP");
                bydTask.ResponsibleEmployee = null;
                await _syncLogisticTaskService.UpdateFromOdataAsync(bydTask);
                bool syncResult = _syncLogisticTaskService.SyncTaskInBackgroundAsync(objectId);
                if (!syncResult) throw new ApplicationException("Erreur de synchronisation depuis SAP");

                return await Result<bool>.SuccessAsync(true);
            }
            catch (Exception e)
            {
                return await Result<bool>.FailAsync(_localizer[e.Message]);
            }
        }

        public async Task<IResult<LogisticTaskDTO2>> SaveTask(LogisticTaskDTO2 logisticTaskDto)
        {
            try
            {
                var bydTask = await _bydLogisticTaskService.GetOdataTaskByOIDAsync(logisticTaskDto.ObjectID);
                if (bydTask == null)
                {
                    logisticTaskDto.ProcessingStatusCode = "?";
                    throw new ApplicationException("tache introuvable sur SAP");
                }

                if (bydTask.ProcessingStatusCode.Equals("3"))
                {
                    logisticTaskDto.ProcessingStatusCode = "?";
                    throw new ApplicationException("La tache est terminée dans SAP");
                }

                if (!string.IsNullOrWhiteSpace(bydTask.ResponsibleEmployeeID) &&
                    !bydTask.ResponsibleEmployeeID.Equals(_currentUserService.EmployeeID))
                {
                    logisticTaskDto.ProcessingStatusCode = "?";
                    throw new ApplicationException("La tache est affectée à un autre employée");
                }


                var setResult = await _bydLogisticTaskService.PostTaskAsync(logisticTaskDto);
                if (setResult.Succeeded == false)
                {
                    return setResult;
                }


                bool syncResult = await _syncLogisticTaskService.SyncTaskAsync(logisticTaskDto.ObjectID);
                if (!syncResult) throw new ApplicationException("Erreur de synchronisation depuis SAP");

                return await GetTaskDetail(logisticTaskDto.ObjectID);
            }
            catch (Exception e)
            {
                return await Result<LogisticTaskDTO2>.FailAsync(_localizer[e.Message], logisticTaskDto);
            }
        }
    }
}