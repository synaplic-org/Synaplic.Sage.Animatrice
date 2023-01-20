using Uni.Scan.Application.Interfaces.Common;
using Uni.Scan.Shared.Wrapper;
using System.Threading.Tasks;
using Uni.Scan.Transfer.Requests.Identity;
using Uni.Scan.Transfer.Responses.Identity;

namespace Uni.Scan.Application.Interfaces.Services.Identity
{
    public interface ITokenService : IService
    {
        Task<Result<TokenResponse>> LoginAsync(TokenRequest model);

        Task<Result<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest model);
    }
}