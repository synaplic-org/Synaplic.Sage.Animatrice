using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Uni.Scan.Infrastructure.Services.v1;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.DataModel;

namespace Uni.Scan.Server.Controllers.Logistics
{
    public class LogisticParametreController : BaseApiController<LogisticParametreController>
    {
        private readonly ILogisticParametreService _logisticParametreService;

        public LogisticParametreController(ILogisticParametreService logisticParametreService)
        {
            _logisticParametreService = logisticParametreService;
        }
        /// <summary>
        /// Get All
        /// </summary>
        /// <returns>Status 200 OK</returns>

        [HttpGet(nameof(GetAllAsync))]
        public async Task<Result<List<LogisticParametresDTO>>> GetAllAsync()
        {
            return await _logisticParametreService.GetAllAsync();

        }
        /// <summary>
        /// Get by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        /// 
        [HttpGet(nameof(GetByIdAsync) + "/{id}")]
        public async Task<LogisticParametresDTO> GetByIdAsync(int id)
        {
            return await _logisticParametreService.GetByIdAsync(id);
        }


        /// <summary>
        /// Save
        /// </summary>
        /// <returns>Status 200 OK</returns>

        [HttpPost(nameof(SaveAsync))]
        public async Task<Result<int>> SaveAsync(LogisticParametresDTO request)
        {
            return await _logisticParametreService.SaveAsync(request);
        }
      
        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        /// 
        [HttpDelete(nameof(DeleteAsync) + "/{id}")]
        public async Task<Result<int>> DeleteAsync(int id)
        {
            return await _logisticParametreService.DeleteAsync(id);
        }
    }
}
