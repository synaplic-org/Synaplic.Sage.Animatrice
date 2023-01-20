using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Uni.Scan.Client.Infrastructure.Extensions;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.DataModel;

namespace Uni.Scan.Client.Infrastructure.Managers.Logistics
{
    public interface ILogisticAreaManager : IManager
    {
        Task<IResult<List<SiteDTO>>> GetSites();
    }

    public class LogisticAreaManager : ILogisticAreaManager
    {
        private readonly HttpClient _httpClient;

        public LogisticAreaManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<List<SiteDTO>>> GetSites()
        {
            var response = await _httpClient.GetAsync(Routes.LogisticAreaEndpoints.GetSites);
            return await response.ToResult<List<SiteDTO>>();
        }
    }
}