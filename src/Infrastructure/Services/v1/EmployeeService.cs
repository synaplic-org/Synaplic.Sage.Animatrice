using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Uni.Scan.Application.Interfaces.Services;
using Uni.Scan.Infrastructure.ByDesign.Service;
using Uni.Scan.Infrastructure.Contexts;
using Uni.Scan.Infrastructure.Services.Synchronisation;
using Uni.Scan.Shared.Localizers;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.DataModel;

namespace Uni.Scan.Infrastructure.Services.v1
{
    public interface IEmployeeService
    {
        Task<IResult<List<EmployeeDTO>>> GetAllEmployees();
    }

    public class EmployeeService : IEmployeeService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IStringLocalizer<BackendLocalizer> _localizer;
        private readonly BydLogisticTaskService _bydLogisticTaskService;


        public EmployeeService(ICurrentUserService currentUserService,
            BydLogisticTaskService bydLogisticTaskService,
            IStringLocalizer<BackendLocalizer> localizer)
        {
            _currentUserService = currentUserService;
            _localizer = localizer;
            _bydLogisticTaskService = bydLogisticTaskService;
        }


        public async Task<IResult<List<EmployeeDTO>>> GetAllEmployees()
        {
            var result = await _bydLogisticTaskService.GetResponsibleEmployeesAsync();

            if (result != null)
            {
                var employees = result.Adapt<List<EmployeeDTO>>();
                return await Result<List<EmployeeDTO>>.SuccessAsync(employees);
            }
            else
            {
                return await Result<List<EmployeeDTO>>.FailAsync();
            }
        }
    }
}