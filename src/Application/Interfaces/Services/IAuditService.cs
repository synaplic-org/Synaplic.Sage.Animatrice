using Uni.Scan.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Uni.Scan.Transfer.Responses.Audit;

namespace Uni.Scan.Application.Interfaces.Services
{
    public interface IAuditService
    {
        Task<IResult<IEnumerable<AuditResponse>>> GetCurrentUserTrailsAsync(string userId);

        Task<IResult<string>> ExportToExcelAsync(string userId, string searchString = "", bool searchInOldValues = false, bool searchInNewValues = false);
    }
}