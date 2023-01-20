using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Uni.Scan.Infrastructure.Services.v1;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.DataModel;

namespace Uni.Scan.Server.Controllers.Logistics
{
    public class MaterialController : BaseApiController<MaterialController>
    {
        private readonly IMaterialService _materialService;

        public MaterialController(IMaterialService materialService)
        {
            _materialService = materialService;
        }

        [HttpGet(nameof(GetMaterial) + "/{id}")]
        public async Task<IResult<MaterialDTO>> GetMaterial(string id)
        {
            return await _materialService.GetMaterial(id);
        }

        [HttpGet(nameof(GetMaterialsByMangementType) + "/{code}")]
        public async Task<IResult<List<MaterialDTO>>> GetMaterialsByMangementType(string code)
        {
            return await _materialService.GetMaterialsByMangementType(code);
        }
    }
}