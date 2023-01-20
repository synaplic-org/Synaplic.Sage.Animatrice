using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Mapster;
using Uni.Scan.Domain.Entities;
using Uni.Scan.Infrastructure.ByDesign.Service;
using Uni.Scan.Infrastructure.Contexts;
using Uni.Scan.Infrastructure.Services.v1;
using Uni.Scan.Shared.Exceptions;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.DataModel;
using Uni.Scan.Transfer.Requests.Label;
using Uni.Scan.Transfer.Requests.LogisticArea;
using Uni.Scan.Transfer.Requests.Task;
using Uni.Scan.Infrastructure.Services.Labels;
using Microsoft.Extensions.Options;
using Uni.Scan.Shared.Configurations;

namespace Uni.Scan.Server.Controllers.Logistics
{
    public class LogisticLabelController : BaseApiController<LogisticLabelController>
    {
        private readonly ILogisticLabelService _logisticLabelService;
        private readonly AppConfiguration _config;


        public LogisticLabelController(IOptions<AppConfiguration> config, ILogisticLabelService logisticLabelService)
        {
            _config = config.Value;
            _logisticLabelService = logisticLabelService;
        }


        [HttpPost(nameof(SaveLabel))]
        public async Task<IResult<LogisticTaskLabelDTO2>> SaveLabel(LogisticTaskLabelDTO2 request)
        {
            try
            {
                var result = await _logisticLabelService.SaveLabelAsync(request);
                return result;
            }
            catch (Exception e)
            {
                return await Result<LogisticTaskLabelDTO2>.FailAsync(e.Message, request);
            }
        }

        [HttpDelete(nameof(DeleteLabel))]
        public async Task<IResult<bool>> DeleteLabel(LogisticTaskLabelDTO2 request)
        {
            try
            {
                var result = await _logisticLabelService.DeleteLabelAsync(request);
                return result;
            }
            catch (Exception e)
            {
                return await Result<bool>.FailAsync(e.Message, false);
            }
        }

        [HttpPost(nameof(GeneratePdf64File))]
        public async Task<Result<string>> GeneratePdf64File(LogisticPrintRequest request)
        {
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(_config.ReportUrl);
            var ressource = @"Logistic/";

            if (request.ModelType == LogisticPrintType.Label)
            {
                ressource += "PrintLabel";
            }

            if (request.ModelType == LogisticPrintType.Task)
            {
                ressource += "PrintTask";
            }

            try
            {
                var response = await httpClient.PostAsJsonAsync(ressource, request);
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