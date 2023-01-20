using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Uni.Scan.Infrastructure.Services.Inventory;
using Uni.Scan.Infrastructure.Services.v1;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.DataModel;

namespace Uni.Scan.Server.Controllers.Inventory
{
    public class InventoryController : BaseApiController<InventoryController>
    {
        private readonly IInventoryService _service;

        public InventoryController(IInventoryService service)
        {
            _service = service;
        }

        [HttpGet(nameof(GetAllowedTasks))]
        public async Task<IResult<List<InventoryTaskDTO>>> GetAllowedTasks()
        {
            return await _service.GetAllowedTasks();
        }

        [HttpGet(nameof(GetMyTasks))]
        public async Task<IResult<List<InventoryTaskDTO>>> GetMyTasks()
        {
            return await _service.GetMyTasks();
        }

        [HttpGet(nameof(GetUnassignedTasks))]
        public async Task<IResult<List<InventoryTaskDTO>>> GetUnassignedTasks()
        {
            return await _service.GetUnassignedTasks();
        }

        [HttpGet(nameof(GetTaskDetails))]
        public async Task<IResult<InventoryTaskDTO>> GetTaskDetails(string id)
        {
            return await _service.GetTaskDetails(id);
        }

        [HttpPost(nameof(SetTaskResponsible))]
        public async Task<IResult<bool>> SetTaskResponsible(InventoryTaskDTO request)
        {
            return await _service.SetTaskResponsible(request);
        }

        [HttpPost(nameof(RemoveTaskResponsible))]
        public async Task<IResult<bool>> RemoveTaskResponsible(InventoryTaskDTO request)
        {
            return await _service.RemoveTaskResponsible(request);
        }

        [HttpPost(nameof(UpdateInventoryTaskDetails))]
        public async Task<IResult<List<string>>> UpdateInventoryTaskDetails(List<InventoryTaskItemDTO> requesList)
        {
            return await _service.UpdateInventoryTaskDetails(requesList);
        }

        [HttpPost(nameof(FinishInventoryTask))]
        public async Task<IResult<bool>> FinishInventoryTask(string taskId)
        {
            return await _service.FinishInventoryTask(taskId);
        }
    }
}