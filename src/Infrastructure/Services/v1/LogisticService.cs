using Mapster;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uni.Scan.Application.Interfaces.Services;
using Uni.Scan.Application.Interfaces.Services.Identity;
using Uni.Scan.Infrastructure.ByDesign.Service;
using Uni.Scan.Infrastructure.Contexts;
using Uni.Scan.Infrastructure.Services.Synchronisation;
using Uni.Scan.Shared.Localizers;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.DataModel;

namespace Uni.Scan.Infrastructure.Services.v1
{
    //public interface ILogisticService
    //{
    //    Task<IResult<List<LogisticTaskDTO>>> GetUnassignedTasks(string logisticType);
    //    Task<IResult<List<LogisticTaskDTO>>> GetMyTasks(string logisticType);

    //    Task<IResult<List<LogisticTaskDTO>>> GetTasksBy(string searchString);
    //    //Task<IResult<bool>> SetTaskResponsible(LogisticTaskDTO task);
    //    //Task<IResult<bool>> RemoveTaskResponsible(LogisticTaskDTO task);

    //    Task<IResult<LogisticTaskDTO>> GetTaskDetails(string taskId);

    //    //ValidateTaskScan(taskID)
    //    //SyncTask(taskID)
    //    //Task<IResult<List<LogisticTaskDTO2>>> GetAllowedTasks(string operationType);
    //    //Task<IResult<LogisticTaskDTO2>> GetTaskDetail(string objectID);
    //}

    //public class LogisticService : ILogisticService
    //{
    //    private readonly ICurrentUserService _currentUserService;
    //    private readonly IUserService _userService;
    //    private readonly UniContext _uniContext;
    //    private readonly IStringLocalizer<BackendLocalizer> _localizer;
    //    private readonly BydLogisticTaskService _bydLogisticTaskService;
    //    private readonly SyncLogisticTaskService _syncLogisticTaskService;

    //    public LogisticService(ICurrentUserService currentUserService,
    //        IUserService userService,
    //        UniContext uniContext, BydLogisticTaskService bydLogisticTaskService,
    //        SyncLogisticTaskService syncLogisticTaskService,
    //        IStringLocalizer<BackendLocalizer> localizer)
    //    {
    //        _currentUserService = currentUserService;
    //        _userService = userService;
    //        _uniContext = uniContext;
    //        _localizer = localizer;
    //        _bydLogisticTaskService = bydLogisticTaskService;
    //        _syncLogisticTaskService = syncLogisticTaskService;
    //    }

    //    public async Task<IResult<List<LogisticTaskDTO>>> GetMyTasks(string logisticType)
    //    {
    //        var user = await _userService.GetAsync(_currentUserService.UserId);
    //        var employeeId = user.Data.EmployeeID;
    //        var result = _uniContext.LogisticTasks.Where(e =>
    //            e.ResponsibleId.Equals(employeeId) && e.ProcessType.Equals(logisticType) &&
    //            !e.ProcessingStatusCode.Equals("3")).ToList();
    //        var logisticTasks = result.Adapt<List<LogisticTaskDTO>>();

    //        return await Result<List<LogisticTaskDTO>>.SuccessAsync(logisticTasks);
    //    }

    //    public async Task<IResult<List<LogisticTaskDTO>>> GetUnassignedTasks(string logisticType)
    //    {
    //        var user = await _userService.GetAsync(_currentUserService.UserId);
    //        var userSiteId = user.Data.SiteID;
    //        var result = _uniContext.LogisticTasks.Where(e => e.ResponsibleId != null
    //                                                          && e.ResponsibleId != string.Empty
    //                                                          && e.ProcessType.Equals(logisticType)
    //                                                          && !e.ProcessingStatusCode.Equals("3")
    //                                                          && (e.SiteId.Equals(userSiteId) ||
    //                                                              userSiteId.Equals("*"))).ToList();
    //        var logisticTasks = result.Adapt<List<LogisticTaskDTO>>();


    //        return await Result<List<LogisticTaskDTO>>.SuccessAsync(logisticTasks);
    //    }

    //    public async Task<IResult<List<LogisticTaskDTO>>> GetTasksBy(string searchString)
    //    {
    //        var user = await _userService.GetAsync(_currentUserService.UserId);
    //        var employeeId = user.Data.EmployeeID;
    //        var result = _uniContext.LogisticTasks
    //            .Where(e => e.Id.Equals(searchString) && !e.ProcessingStatusCode.Equals("3")).ToList();
    //        if (result.Count == 0)
    //        {
    //            result = _uniContext.LogisticTasks
    //                .Where(e => e.RequestId.Equals(searchString) && !e.ProcessingStatusCode.Equals("3")).ToList();
    //        }

    //        var logisticTasks = result.Adapt<List<LogisticTaskDTO>>();
    //        return await Result<List<LogisticTaskDTO>>.SuccessAsync(logisticTasks);
    //    }

    //    //public async Task<IResult<bool>> SetTaskResponsible(LogisticTaskDTO task)
    //    //{
    //    //    var user = await _userService.GetAsync(_currentUserService.UserId);
    //    //    var employeeId = user.Data.EmployeeID;
    //    //    var response = await BydService.SetLogisticsTaskResponsibleAsync(task.ObjectID, employeeId);
    //    //    if (response)
    //    //    {
    //    //        var adaptedTask = task.Adapt<LogisticTask>();
    //    //        adaptedTask.ResponsibleId = employeeId;
    //    //        _uniContext.LogisticTasks.Update(adaptedTask);
    //    //        await _uniContext.SaveChangesAsync();
    //    //    }

    //    //    return await Result<bool>.SuccessAsync(response);
    //    //}

    //    //public async Task<IResult<bool>> RemoveTaskResponsible(LogisticTaskDTO task)
    //    //{
    //    //    var response = await BydService.RemoveTaskResponsibleAsync(task.ObjectID);
    //    //    if (response)
    //    //    {
    //    //        var adaptedTask = task.Adapt<LogisticTask>();
    //    //        adaptedTask.ResponsibleId = null;
    //    //        adaptedTask.ResponsibleName = null;
    //    //        _uniContext.LogisticTasks.Update(adaptedTask);
    //    //        await _uniContext.SaveChangesAsync();
    //    //    }

    //    //    return await Result<bool>.SuccessAsync(response);
    //    //}

    //    public async Task<IResult<LogisticTaskDTO>> GetTaskDetails(string taskId)
    //    {
    //        try
    //        {
    //            var task = _uniContext.LogisticTasks.FirstOrDefault(e => e.Id == taskId);

    //            if (task == null) return await Result<LogisticTaskDTO>.FailAsync("Task not found");

    //            var taskDetails = _uniContext.LogisticTaskDetails.Where(e => e.TaskId == taskId).ToList();
    //            var matrialsID = taskDetails.Select(x => x.ProductID).Distinct().ToList();
    //            var matrials = _uniContext.Materials.Where(x => matrialsID.Contains(x.ProductID)).ToList();
    //            var areas = _uniContext.LogisticAreas.Where(o => o.SiteID == task.SiteId).ToList();
    //            var labels = _uniContext.LogisticTaskLabels.Where(x => x.TaskId.Equals(taskId)).ToList();
    //            var dto = task.Adapt<LogisticTaskDTO>();

    //            dto.Labels = labels.Adapt<List<LogisticTaskLabelDTO>>();
    //            dto.LogisticTaskDetails = taskDetails.Adapt<List<LogisticTaskDetailDTO>>();
    //            dto.Martials = matrials.Adapt<List<MaterialDTO>>();
    //            dto.LogisticAreas = areas.Adapt<List<LogisticAreaDTO>>();
    //            return await Result<LogisticTaskDTO>.SuccessAsync(dto);
    //        }
    //        catch (Exception e)
    //        {
    //            return await Result<LogisticTaskDTO>.FailAsync(e.Message);
    //        }
    //    }
    //}
}