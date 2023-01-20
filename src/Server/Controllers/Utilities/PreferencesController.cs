﻿using System.Threading.Tasks;
using Uni.Scan.Server.Managers.Preferences;
using Uni.Scan.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Uni.Scan.Server.Controllers.Utilities
{
	[Route("api/[controller]")]
	[ApiController]
	[ApiExplorerSettings(IgnoreApi = true)]
	public class PreferencesController : ControllerBase
	{
		private readonly ServerPreferenceManager _serverPreferenceManager;

		public PreferencesController(ServerPreferenceManager serverPreferenceManager)
		{
			_serverPreferenceManager = serverPreferenceManager;
		}

		/// <summary>
		/// Change Language Preference
		/// </summary>
		/// <param name="languageCode"></param>
		/// <returns>ProcessingStatusCode 200 OK</returns>
		[Authorize(Policy = Permissions.Preferences.ChangeLanguage)]
		[HttpPost("changeLanguage")]
		public async Task<IActionResult> ChangeLanguageAsync(string languageCode)
		{
			var result = await _serverPreferenceManager.ChangeLanguageAsync(languageCode);
			return Ok(result);
		}

		//TODO - add actions
	}
}