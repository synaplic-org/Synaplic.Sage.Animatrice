using Uni.Scan.Application.Interfaces.Services.Identity;
using Uni.Scan.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Uni.Scan.Transfer.Requests.Identity;

namespace Uni.Scan.Server.Controllers.Identity
{
	[Authorize]
	[Route("api/identity/user")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;

		public UserController(IUserService userService)
		{
			_userService = userService;
		}

		/// <summary>
		/// Get User Details
		/// </summary>
		/// <returns>ProcessingStatusCode 200 OK</returns>
		[Authorize(Policy = Permissions.Users.View)]
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var users = await _userService.GetAllAsync();
			return Ok(users);
		}

		/// <summary>
		/// Get User By Id
		/// </summary>
		/// <param name="id"></param>
		/// <returns>ProcessingStatusCode 200 OK</returns>
		//[Authorize(Policy = Permissions.Users.View)]
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(string id)
		{
			var user = await _userService.GetAsync(id);
			return Ok(user);
		}

		/// <summary>
		/// Get User Roles By Id
		/// </summary>
		/// <param name="id"></param>
		/// <returns>ProcessingStatusCode 200 OK</returns>
		[Authorize(Policy = Permissions.Users.View)]
		[HttpGet("roles/{id}")]
		public async Task<IActionResult> GetRolesAsync(string id)
		{
			var userRoles = await _userService.GetRolesAsync(id);
			return Ok(userRoles);
		}

		/// <summary>
		/// Update Roles for User
		/// </summary>
		/// <param name="request"></param>
		/// <returns>ProcessingStatusCode 200 OK</returns>
		[Authorize(Policy = Permissions.Users.Edit)]
		[HttpPut("roles/{id}")]
		public async Task<IActionResult> UpdateRolesAsync(UpdateUserRolesRequest request)
		{
			return Ok(await _userService.UpdateRolesAsync(request));
		}

		/// <summary>
		/// Register a User
		/// </summary>
		/// <param name="request"></param>
		/// <returns>ProcessingStatusCode 200 OK</returns>
		[AllowAnonymous]
		[HttpPost]
		public async Task<IActionResult> RegisterAsync(RegisterRequest request)
		{
			var origin = Request.Headers["origin"];
			return Ok(await _userService.RegisterAsync(request, origin));
		}

		/// <summary>
		/// Confirm Email
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="code"></param>
		/// <returns>ProcessingStatusCode 200 OK</returns>
		[HttpGet("confirm-email")]
		[AllowAnonymous]
		public async Task<IActionResult> ConfirmEmailAsync([FromQuery] string userId, [FromQuery] string code)
		{
			return Ok(await _userService.ConfirmEmailAsync(userId, code));
		}

		/// <summary>
		/// Toggle User ProcessingStatusCode (Activate and Deactivate)
		/// </summary>
		/// <param name="request"></param>
		/// <returns>ProcessingStatusCode 200 OK</returns>
		[HttpPost("toggle-status")]
		public async Task<IActionResult> ToggleUserStatusAsync(ToggleUserStatusRequest request)
		{
			return Ok(await _userService.ToggleUserStatusAsync(request));
		}

		/// <summary>
		/// Forgot Password
		/// </summary>
		/// <param name="request"></param>
		/// <returns>ProcessingStatusCode 200 OK</returns>
		[HttpPost("forgot-password")]
		[AllowAnonymous]
		public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordRequest request)
		{
			var origin = Request.Headers["origin"];
			return Ok(await _userService.ForgotPasswordAsync(request, origin));
		}

		/// <summary>
		/// Reset Password
		/// </summary>
		/// <param name="request"></param>
		/// <returns>ProcessingStatusCode 200 OK</returns>
		[HttpPost("reset-password")]
		[AllowAnonymous]
		public async Task<IActionResult> ResetPasswordAsync(ResetPasswordRequest request)
		{
			return Ok(await _userService.ResetPasswordAsync(request));
		}

		/// <summary>
		/// Update User
		/// </summary>
		/// <param name="request"></param>
		/// <returns>ProcessingStatusCode 200 OK</returns>
		[HttpPut("update-user")]
		[AllowAnonymous]
		public async Task<IActionResult> UpdateUserAsync(UpdateUserRequest request)
		{
			return Ok(await _userService.UpdateUser(request));
		}

		/// <summary>
		/// Export to Excel
		/// </summary>
		/// <param name="searchString"></param>
		/// <returns>ProcessingStatusCode 200 OK</returns>
		[Authorize(Policy = Permissions.Users.Export)]
		[HttpGet("export")]
		public async Task<IActionResult> Export(string searchString = "")
		{
			var data = await _userService.ExportToExcelAsync(searchString);
			return Ok(data);
		}
	}
}