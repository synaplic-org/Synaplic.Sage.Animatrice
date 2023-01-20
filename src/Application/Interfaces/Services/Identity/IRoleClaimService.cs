using System.Collections.Generic;
using System.Threading.Tasks;
using Uni.Scan.Application.Interfaces.Common;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.Requests.Identity;
using Uni.Scan.Transfer.Responses.Identity;

namespace Uni.Scan.Application.Interfaces.Services.Identity
{
    public interface IRoleClaimService : IService
    {
        Task<Result<List<RoleClaimResponse>>> GetAllAsync();

        Task<int> GetCountAsync();

        Task<Result<RoleClaimResponse>> GetByIdAsync(int id);

        Task<Result<List<RoleClaimResponse>>> GetAllByRoleIdAsync(string roleId);

        Task<Result<string>> SaveAsync(RoleClaimRequest request);

        Task<Result<string>> DeleteAsync(int id);
    }
}