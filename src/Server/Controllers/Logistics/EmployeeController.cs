using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Uni.Scan.Infrastructure.Services.v1;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.DataModel;

namespace Uni.Scan.Server.Controllers.Logistics
{
    public class EmployeeController : BaseApiController<EmployeeController>
    {
        private readonly IEmployeeService _EmployeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _EmployeeService = employeeService;
        }

        [HttpGet(nameof(GetAll))]
        public async Task<IResult<List<EmployeeDTO>>> GetAll()
        {
            return await _EmployeeService.GetAllEmployees();
        }
    }
}