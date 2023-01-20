using Uni.Scan.Application.Interfaces.Services;
using Uni.Scan.Application.Interfaces.Services.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Uni.Scan.Transfer.Requests.Identity;

namespace Uni.Scan.Server.Controllers.Identity
{
	[Route("api/identity/token")]
	[ApiController]
	public class TokenController : ControllerBase
	{
		private readonly ITokenService _identityService;

		public TokenController(ITokenService identityService, ICurrentUserService currentUserService)
		{
			_identityService = identityService;
		}

		/// <summary>
		/// Get Token (Email, Password)
		/// </summary>
		/// <param name="model"></param>
		/// <returns>ProcessingStatusCode 200 OK</returns>
		[HttpPost]
		public async Task<ActionResult> Get(TokenRequest model)
		{
			var response = await _identityService.LoginAsync(model);
			return Ok(response);
		}

		/// <summary>
		/// Refresh Token
		/// </summary>
		/// <param name="model"></param>
		/// <returns>ProcessingStatusCode 200 OK</returns>
		[HttpPost("refresh")]
		public async Task<ActionResult> Refresh([FromBody] RefreshTokenRequest model)
		{
			var response = await _identityService.GetRefreshTokenAsync(model);
			return Ok(response);
		}
	}
}