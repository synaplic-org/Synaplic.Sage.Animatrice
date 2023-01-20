using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Uni.Scan.Infrastructure.Services.v1;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.DataModel;

namespace Uni.Scan.Server.Controllers.Stock
{
    public class StockAnomalyController : BaseApiController<StockAnomalyController>
    {
        private readonly IStockAnomalyService _stockAnomalyService;

        public StockAnomalyController(IStockAnomalyService stockAnomalyService)
        {
            _stockAnomalyService = stockAnomalyService;
        }

        /// <summary>
        /// Get All Anomalies
        /// </summary>
        /// <returns>ProcessingStatusCode 200 OK</returns>
        [HttpGet(nameof(GetAllAsync))]
        public async Task<IResult<List<StockAnomalyDTO>>> GetAllAsync()
        {
            return await _stockAnomalyService.GetAllAsync();
        }

        /// <summary>
        /// Get Anomaly by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>ProcessingStatusCode 200 OK</returns>
        
        [HttpGet(nameof(GetByIdAsync) + "/{id}")]
        public async Task<IResult<StockAnomalyDTO>> GetByIdAsync(int id)
        {
            return await _stockAnomalyService.GetByIdAsync(id);
        }

        [HttpPost(nameof(SaveAnomaly))]
        public async Task<IResult<StockAnomalyDTO>> SaveAnomaly(StockAnomalyDTO request)
        {
            try
            {
                var result = await _stockAnomalyService.SaveAnomalyAsync(request);
                return result;
            }
            catch (Exception e)
            {
                return await Result<StockAnomalyDTO>.FailAsync(e.Message, request);
            }
        }

        /// <summary>
        /// Save Anomaly
        /// </summary>
        /// <returns>ProcessingStatusCode 200 OK</returns>
        [HttpPost(nameof(SaveAsync))]
        public async Task<Result<int>> SaveAsync(StockAnomalyDTO request)
        {
            return await _stockAnomalyService.SaveAllAsync(request);
        }

        /// <summary>
        /// Update Anomaly
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>ProcessingStatusCode 200 OK</returns>
        /// 
        [HttpPut(nameof(UpdateAsync) + "/{Id}")]
        public async Task<IResult<bool>> UpdateAsync(StockAnomalyDTO Id)
        {
            return await _stockAnomalyService.UpdateAsync(Id);
        }

        /// <summary>
        /// Validate Anomaly
        /// </summary>
        /// <param name="request"></param>
        /// <returns>ProcessingStatusCode 200 OK</returns>
  
        [HttpPut(nameof(ValidateAsync))]
        public async Task<IResult<int>>  ValidateAsync(StockAnomalyDTO request)
        {
            return await _stockAnomalyService.ValidateAnomalie(request);
        }
        /// <summary>
        /// Reject Anomaly
        /// </summary>
        /// <param name="request"></param>
        /// <returns>ProcessingStatusCode 200 OK</returns>
       
        [HttpPut(nameof(RejectAsync))]
        public async Task<IResult<int>> RejectAsync(StockAnomalyDTO request)
        {
            return await _stockAnomalyService.RejectAnomalie(request);
        }

        /// <summary>
        /// Cancel Anomaly
        /// </summary>
        /// <param name="id"></param>
        /// <returns>ProcessingStatusCode 200 OK</returns>
   
        [HttpPut(nameof(CancelAsync))]
        public async Task<IResult<int>> CancelAsync(int id)
        {
            return await _stockAnomalyService.CancelAnomalie(id);
        }

        /// <summary>
        /// Delete Anomaly
        /// </summary>
        /// <param name="id"></param>
        /// <returns>ProcessingStatusCode 200 OK</returns>
   
        [HttpDelete(nameof(DeleteAsync) + "/{id}")]
        public async Task<IResult<bool>> DeleteAsync(int id)
        {
            return await _stockAnomalyService.DeleteAsync(id);
        }
    }
}