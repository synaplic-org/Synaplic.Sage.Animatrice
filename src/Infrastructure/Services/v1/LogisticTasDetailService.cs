using Mapster;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uni.Scan.Infrastructure.Contexts;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.DataModel;

namespace Uni.Scan.Infrastructure.Services.v1
{
    public interface ILogisticTasDetailService
    {
        Task<IResult<List<LogisticTaskDetailDTO>>> GetTaskDetails(string taskID);
    }
    public class LogisticTasDetailService : ILogisticTasDetailService
    {
        private readonly UniContext _uniContext;
        public LogisticTasDetailService(UniContext uniContext)
        {
            _uniContext = uniContext;
        }
        public async Task<IResult<List<LogisticTaskDetailDTO>>> GetTaskDetails(string taskID)
        {
            var result = _uniContext.LogisticTaskDetails.Where(e => e.TaskId.Equals(taskID)).ToList();
            var logisticTaskDetails = result.Adapt<List<LogisticTaskDetailDTO>>();
            return await Result<List<LogisticTaskDetailDTO>>.SuccessAsync(logisticTaskDetails);
        }
    }
}
