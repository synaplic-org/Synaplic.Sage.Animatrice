using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Uni.Scan.Infrastructure.Services.v1;
using Uni.Scan.Shared.Constants.Permission;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.DataModel;

namespace Uni.Scan.Server.Controllers.Logistics
{
	public class LabelTemplateController : BaseApiController<LabelTemplateController>
	{
		private readonly ILabelTemplateService _parametresService;

		public LabelTemplateController(ILabelTemplateService parametresService)
		{
			_parametresService = parametresService;
		}

		/// <summary>
		/// Get Parametre
		/// </summary>
		/// <returns>ProcessingStatusCode 200 OK</returns>
		[HttpGet(nameof(GetAllAsync))]
		public async Task<Result<List<LabelTemplateDTO>>> GetAllAsync()
		{
			return await _parametresService.GetAllAsync();
		}


		/// <summary>
		/// Save Parametre
		/// </summary>
		/// <returns>ProcessingStatusCode 200 OK</returns>
		[HttpPost(nameof(SaveAsync))]
		public async Task<Result<int>> SaveAsync(LabelTemplateDTO request)
		{
			return await _parametresService.SaveAsync(request);
		}

		/// <summary>
		/// Delete Parametre
		/// </summary>
		/// <param name="id"></param>
		/// <returns>ProcessingStatusCode 200 OK</returns>
		/// 
		[HttpDelete(nameof(DeleteAsync) + "/{id}")]
		public async Task<Result<int>> DeleteAsync(int id)
		{
			return await _parametresService.DeleteAsync(id);
		}
	}
}