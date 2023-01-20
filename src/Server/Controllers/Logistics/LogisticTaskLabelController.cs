using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Uni.Scan.Domain.Entities;
using Uni.Scan.Infrastructure.Services.v1;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.DataModel;
using Uni.Scan.Transfer.Requests.Label;
using Uni.Scan.Transfer.Requests.LogisticArea;
using Uni.Scan.Transfer.Requests.Task;

namespace Uni.Scan.Server.Controllers.Logistics
{
    public class LogisticTaskLabelController : BaseApiController<LogisticTaskLabelController>
    {
        private readonly ILogisticTaskLabelService _taskLabelService;

        public LogisticTaskLabelController(ILogisticTaskLabelService taskLabelService)
        {
            _taskLabelService = taskLabelService;
        }

        /// <summary>
        /// Get All Labels
        /// </summary>
        /// <returns>ProcessingStatusCode 200 OK</returns>
        [HttpGet(nameof(GetAll))]
        public async Task<Result<List<LogisticTaskLabelDTO>>> GetAll()
        {
            return await _taskLabelService.GetAllAsync();
        }

        /// <summary>
        /// Get All Labels
        /// </summary>
        /// <returns>ProcessingStatusCode 200 OK</returns>
        [HttpGet(nameof(GetAllTaskLabelAsync) + "/{id}")]
        public async Task<Result<List<LogisticTaskLabelDTO>>> GetAllTaskLabelAsync(string id)
        {
            return await _taskLabelService.GetAllTaskLabels(id);
        }

        /// <summary>
        /// Delete Label
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete(nameof(Delete) + "/{id}")]
        public async Task<Result<int>> Delete(int id)
        {
            return await _taskLabelService.DeleteAsync(id);
        }

        /// <summary>
        /// Get  Labels by ID
        /// </summary>
        /// <returns>ProcessingStatusCode 200 OK</returns>
        /// 
        [HttpGet(nameof(GetTaskLabel) + "/{id}")]
        public async Task<IResult<LogisticTaskLabelDTO>> GetTaskLabel(string id)
        {
            return await _taskLabelService.GetTaskLabel(int.Parse(id));
        }


        /// <summary>
        /// Save Label
        /// </summary>
        /// <returns>ProcessingStatusCode 200 OK</returns>
        /// 
        [HttpPost(nameof(SaveTaskLabels))]
        public async Task<Result<bool>> SaveTaskLabels(List<LogisticTaskLabelDTO> request)
        {
            return await _taskLabelService.SaveAsync(request);
        }
        /// <summary>
        /// Save single Label
        /// </summary>
        /// <returns></returns>
        [HttpDelete(nameof(SaveSingleLabel))]
        public async Task<Result<int>> SaveSingleLabel(LogisticTaskLabelDTO dto)
        {
            return await _taskLabelService.SaveLineAsync(dto);
        }
        /// <summary>
        /// Get by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        /// 
        [HttpGet(nameof(GetByIdAsync) + "/{id}")]
        public async Task<LogisticTaskLabelDTO> GetByIdAsync(int id)
        {
            return await _taskLabelService.GetByIdAsync(id);
        }
        /// <summary>
        /// Send Labels 
        /// </summary>
        /// <returns>ProcessingStatusCode 200 OK</returns>
        /// 
        [HttpPost(nameof(SendLabelsAsync))]
        public async Task<Result<bool>> SendLabelsAsync(List<LogisticTaskLabelDTO> request)
        {
            return await _taskLabelService.SaveAsync(request);
        }

        [HttpPost(nameof(GenerateAreaLabelsPdf64))]
        public async Task<Result<string>> GenerateAreaLabelsPdf64(Area request)
        {
            var _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://scan.sapbydesign.app/reports/");

            try
            {
                var response = await _httpClient.PostAsJsonAsync("pdf/PrintAreaLabel", request);
                var pdf = await response.Content.ReadAsByteArrayAsync();
                string base64String = Convert.ToBase64String(pdf);
                var DocumentPath = "data:application/pdf;base64," + base64String;
                return await Result<string>.SuccessAsync(data: DocumentPath);
            }
            catch (Exception e)
            {
                return await Result<string>.SuccessAsync(e.Message);
            }
        }
        [HttpPost(nameof(GenerateLabelsPdf64))]
        public async Task<Result<string>> GenerateLabelsPdf64(LabelPrintRequest request)
        {
            var _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://scan.sapbydesign.app/reports/");

            try
            {
                var response = await _httpClient.PostAsJsonAsync("pdf/PrintLabel", request);
                var pdf = await response.Content.ReadAsByteArrayAsync();
                string base64String = Convert.ToBase64String(pdf);
                var DocumentPath = "data:application/pdf;base64," + base64String;
                return await Result<string>.SuccessAsync(data: DocumentPath);
            }
            catch (Exception e)
            {
                return await Result<string>.SuccessAsync(e.Message);
            }
        }
        
        [HttpPost(nameof(GenerateTaskPdf64))]
        public async Task<Result<string>> GenerateTaskPdf64(TaskPrintRequest request)
        {
            var _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://scan.sapbydesign.app/reports/");

            try
            {
                var response = await _httpClient.PostAsJsonAsync("pdf/PrintTask", request);
                var pdf = await response.Content.ReadAsByteArrayAsync();
                string base64String = Convert.ToBase64String(pdf);
                var DocumentPath = "data:application/pdf;base64," + base64String;
                return await Result<string>.SuccessAsync(data: DocumentPath);
            }
            catch (Exception e)
            {
                return await Result<string>.SuccessAsync(e.Message);
            }
        }
    }
}