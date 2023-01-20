using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Uni.Scan.Application.Interfaces.Services;
using Uni.Scan.Application.Interfaces.Services.Identity;
using Uni.Scan.Infrastructure.ByDesign.Service;
using Uni.Scan.Infrastructure.Contexts;
using Uni.Scan.Infrastructure.Services.Synchronisation;
using Uni.Scan.Shared.Extensions;
using Uni.Scan.Shared.Localizers;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.DataModel;

namespace Uni.Scan.Infrastructure.Services.Inventory
{
    public interface IInventoryService
    {
        Task<IResult<List<InventoryTaskDTO>>> GetAllowedTasks();
        Task<IResult<List<InventoryTaskDTO>>> GetMyTasks();
        Task<IResult<List<InventoryTaskDTO>>> GetUnassignedTasks();
        Task<IResult<InventoryTaskDTO>> GetTaskDetails(string taskId);
        Task<IResult<bool>> SetTaskResponsible(InventoryTaskDTO task);
        Task<IResult<bool>> RemoveTaskResponsible(InventoryTaskDTO task);
        Task<Result<List<string>>> UpdateInventoryTaskDetails(List<InventoryTaskItemDTO> requesList);
        Task<Result<bool>> FinishInventoryTask(string taskId);
    }

    public class InventoryService : IInventoryService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserService _userService;
        private readonly UniContext _uniContext;
        private readonly IStringLocalizer<BackendLocalizer> _localizer;
        private readonly ISyncService _syncService;
        private readonly BydInventoryService _bydInventoryService;


        public InventoryService(ICurrentUserService currentUserService,
            IUserService userService,
            UniContext uniContext,
            IStringLocalizer<BackendLocalizer> localizer,
            BydInventoryService bydInventoryService,
            ISyncService syncService)
        {
            _currentUserService = currentUserService;
            _userService = userService;
            _uniContext = uniContext;
            _localizer = localizer;
            _syncService = syncService;
            _bydInventoryService = bydInventoryService;
        }

        public async Task<IResult<List<InventoryTaskDTO>>> GetAllowedTasks()
        {
            var myresult = _uniContext.InventoryTasks.Where(e => e.ResponsibleId.Equals(
                                                                     _currentUserService.EmployeeID)
                                                                 && !e.ProcessingStatus.Equals("3")).ToList();

            var unissignedresult = _uniContext.InventoryTasks.Where(e => (e.ResponsibleId == null ||
                                                                          e.ResponsibleId == "")
                                                                         && !e.ProcessingStatus.Equals("3")
                                                                         && (e.SiteId.Equals(_currentUserService
                                                                             .SiteId) || _currentUserService.SiteId
                                                                             .Equals("*"))).ToList();

            var result = myresult.Union(unissignedresult).Adapt<List<InventoryTaskDTO>>();

            return await Result<List<InventoryTaskDTO>>.SuccessAsync(result);
        }

        public async Task<IResult<List<InventoryTaskDTO>>> GetMyTasks()
        {
            var result = _uniContext.InventoryTasks.Where(e => e.ResponsibleId.Equals(_currentUserService.EmployeeID)
                                                               && !e.ProcessingStatus.Equals("3")).ToList();


            return await Result<List<InventoryTaskDTO>>.SuccessAsync(result.Adapt<List<InventoryTaskDTO>>());
        }

        public async Task<IResult<List<InventoryTaskDTO>>> GetUnassignedTasks()
        {
            var result = _uniContext.InventoryTasks.Where(e =>
                (e.ResponsibleId == null || e.ResponsibleId == string.Empty)
                && !e.ProcessingStatus.Equals("3")
                && (e.SiteId.Equals(_currentUserService.SiteId) || _currentUserService.SiteId.Equals("*"))).ToList();

            return await Result<List<InventoryTaskDTO>>.SuccessAsync(result.Adapt<List<InventoryTaskDTO>>());
        }

        public async Task<IResult<InventoryTaskDTO>> GetTaskDetails(string taskId)
        {
            var query = _uniContext.InventoryTasks.Include(o => o.InventoryTaskOperations)
                .ThenInclude(o => o.InventoryTaskItems).FirstOrDefault(o => o.ObjectID == taskId);
            var result = query.Adapt<InventoryTaskDTO>();
            return await Result<InventoryTaskDTO>.SuccessAsync(result);
        }

        public async Task<IResult<bool>> SetTaskResponsible(InventoryTaskDTO task)
        {
            if (string.IsNullOrWhiteSpace(_currentUserService.EmployeeID) ||
                _currentUserService.EmployeeID.Trim() == "*")
            {
                return await Result<bool>.FailAsync(_localizer["NO-EMPLOYEEID"]);
            }

            try
            {
                var response =
                    await _bydInventoryService.SetInevntoryTaskResponsibleAsync(task.ObjectID, _currentUserService.EmployeeID);
                if (response)
                {
                    // il faut faire une synchronisation rapide depuis bydesign
                    var tsk = _uniContext.InventoryTasks.Find(task.ObjectID);
                    tsk.ResponsibleId = _currentUserService.EmployeeID;
                    await _uniContext.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                return await Result<bool>.FailAsync(e.Message);
            }

            return await Result<bool>.SuccessAsync(true);
        }

        public async Task<IResult<bool>> RemoveTaskResponsible(InventoryTaskDTO task)
        {
            if (string.IsNullOrWhiteSpace(_currentUserService.EmployeeID) ||
                _currentUserService.EmployeeID.Trim() == "*")
            {
                return await Result<bool>.FailAsync(_localizer["NO-EMPLOYEEID"]);
            }

            try
            {
                var response =
                    await _bydInventoryService.RemoveInevntoryTaskResponsibleAsync(task.ObjectID);
                if (response)
                {
                    // il faut faire une synchronisation rapide depuis bydesign
                    var tsk = _uniContext.InventoryTasks.Find(task.ObjectID);
                    tsk.ResponsibleId = null;
                    await _uniContext.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                return await Result<bool>.FailAsync(e.Message);
            }

            return await Result<bool>.SuccessAsync(true);
        }


        public async Task<Result<bool>> FinishInventoryTask(string objectID)
        {
            var obydtask = await _bydInventoryService.GetPhysicalInventoryTaskByIDAsync(objectID);
            if (obydtask == null)
                return await Result<bool>.FailAsync(_localizer["La tache est introuvable dans SAP "]);
            if (obydtask.ProcessingStatusCode.Equals("3"))
            {
                try
                {
                    var dbtask = _uniContext.InventoryTasks.FirstOrDefault(o => o.ObjectID == obydtask.ObjectID);
                    if (dbtask != null)
                    {
                        dbtask.ProcessingStatus = "3";
                        await _uniContext.SaveChangesAsync();
                    }

                    return await Result<bool>.SuccessAsync(true);
                }
                catch (Exception e)
                {
                    Serilog.Log.Error(e, $"##FinishInventoryTask : {obydtask.ObjectID}");
                }
            }


            var syncResult = await _bydInventoryService.PhysicalInventoryTaskFinish(objectID);
            if (syncResult == false)
            {
                return await Result<bool>.FailAsync(
                    _localizer["Une erreur lors de la clôture est survenue"]);
            }
            else
            {
                try
                {
                    var dbtask = _uniContext.InventoryTasks.FirstOrDefault(o => o.ObjectID == objectID);
                    if (dbtask != null)
                    {
                        dbtask.ProcessingStatus = "3";
                        await _uniContext.SaveChangesAsync();
                    }
                }
                catch (Exception e)
                {
                    Serilog.Log.Error(e, $"##FinishInventoryTask : {objectID}");
                }
            }

            return await Result<bool>.SuccessAsync(syncResult);
        }

        public async Task<Result<List<string>>> UpdateInventoryTaskDetails(List<InventoryTaskItemDTO> requesList)
        {
            var taskID = requesList.First().TaskID;
            var obydtask = await _bydInventoryService.GetPhysicalInventoryTaskAsync(taskID);
            if (obydtask == null)
                return await Result<List<string>>.FailAsync(_localizer["La task est introvable dans SAP "]);
            if (obydtask.ProcessingStatusCode.Equals("3"))
            {
                try
                {
                    var dbtask = _uniContext.InventoryTasks.FirstOrDefault(o => o.ObjectID == obydtask.ObjectID);
                    if (dbtask != null)
                    {
                        dbtask.ProcessingStatus = "3";
                        await _uniContext.SaveChangesAsync();
                    }
                }
                catch (Exception e)
                {
                    Serilog.Log.Error(e, $"##FinishInventoryTask : {obydtask.ObjectID}");
                }

                return await Result<List<string>>.FailAsync(_localizer["La tache est terminée dans SAP "]);
            }

            var updatedIds = new List<string>();
            var messageList = new List<string>();

            var dbList = _uniContext.InventoryTaskItems.Where(w => w.TaskID == taskID).ToList();

            foreach (var item in requesList)
            {
                var dbitem = dbList.FirstOrDefault(o => o.ObjectID == item.ObjectID);
                if (dbitem == null) continue;
                if (dbitem.ZeroCountedQuantityConfirmedIndicator == item.ZeroCountedQuantityConfirmedIndicator && dbitem.CountedQuantity == item.CountedQuantity) continue;

                try
                {
                    var syncResult = await _bydInventoryService.UpdateInventoryTaskItem(item.QuantitObjectID,
                        item.CountedQuantity,
                        item.ZeroCountedQuantityConfirmedIndicator);
                    if (syncResult)
                    {
                        updatedIds.Add(item.ObjectID);
                        dbitem.ZeroCountedQuantityConfirmedIndicator = item.ZeroCountedQuantityConfirmedIndicator;
                        dbitem.CountedQuantity = item.CountedQuantity;
                    }
                    else
                    {
                        messageList.Add(
                            $"Erreur de mise à jour article {item.ProductID} ,Item {item.InventoryItemNumberValue} ");
                    }
                }
                catch (Exception e)
                {
                    messageList.Add(e.Message);
                }
            }

            try
            {
                await _uniContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                messageList.Add(_localizer["Erreure de mise a jour de la base"]);
            }

            if (messageList.IsNullOrEmpty())
            {
                return await Result<List<string>>.SuccessAsync(updatedIds);
            }

            return await Result<List<string>>.FailAsync(messageList);
        }
    }
}