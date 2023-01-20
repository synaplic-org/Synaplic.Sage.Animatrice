using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Uni.Scan.Client.Infrastructure.Extensions;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.DataModel;
using Uni.Scan.Transfer.Requests.Logistics;

namespace Uni.Scan.Client.Infrastructure.Managers.Logistics
{
    public interface ILogisticsManager : IManager
    {
        Task<IResult<List<EmployeeDTO>>> GetEmployees();
    }

    public class LogisticsManager : ILogisticsManager
    {
        private readonly HttpClient _httpClient;

        public LogisticsManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<IResult<List<EmployeeDTO>>> GetEmployees()
        {
            var response = await _httpClient.GetAsync(Routes.LogisticEmployeesEndpoints.GetEmployees);
            return await response.ToResult<List<EmployeeDTO>>();
        }
    }
}