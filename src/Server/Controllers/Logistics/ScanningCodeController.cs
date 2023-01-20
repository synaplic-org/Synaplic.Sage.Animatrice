using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Collections.Generic;
using System.Threading.Tasks;
using Uni.Scan.Infrastructure.Services.v1;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.DataModel;

namespace Uni.Scan.Server.Controllers.Logistics
{
    public class ScanningCodeController : BaseApiController<ScanningCodeController>
    {
        private readonly IScanningCodeService _scanningCodeService;

        public ScanningCodeController(IScanningCodeService scanningCodeService)
        {
            _scanningCodeService = scanningCodeService;
        }
        /// <summary>
        /// Get All
        /// </summary>
        /// <returns>Status 200 OK</returns>

        [HttpGet(nameof(GetAllAsync))]
        public async Task<Result<List<ScanningCodeDTO>>> GetAllAsync()
        {
            return await _scanningCodeService.GetAllAsync();

        }
        /// <summary>
        /// Get by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        /// 
        [HttpGet(nameof(GetByIdAsync) + "/{id}")]
        public async Task<ScanningCodeDTO> GetByIdAsync(int id)
        {
            return await _scanningCodeService.GetByIdAsync(id);
        }


        /// <summary>
        /// Save
        /// </summary>
        /// <returns>Status 200 OK</returns>

        [HttpPost(nameof(SaveCodeAsync))]
        public async Task<Result<bool>> SaveCodeAsync(ScanningCodeDTO request)
        {
            return await _scanningCodeService.SaveAsync(request);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <returns>Status 200 OK</returns>

        [HttpPost(nameof(UpdateCodeAsync))]
        public async Task<Result<bool>> UpdateCodeAsync(List<ScanningCodeDTO> request)
        {
            return await _scanningCodeService.UpdateAsync(request);
        }
        /// <summary>
        /// Save All
        /// </summary>
        /// <returns>Status 200 OK</returns>

        [HttpPost(nameof(SaveAllCodesAsync))]
        public async Task<Result<bool>> SaveAllCodesAsync(List<ScanningCodeDTO> request)
        {
            return await _scanningCodeService.SaveAllAsync(request);
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
            return await _scanningCodeService.DeleteAsync(id);
        }

    }
}
