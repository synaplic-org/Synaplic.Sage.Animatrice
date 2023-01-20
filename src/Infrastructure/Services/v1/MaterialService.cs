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
    public interface IMaterialService
    {
        Task<IResult<MaterialDTO>> GetMaterial(string id);
        Task<IResult<List<MaterialDTO>>> GetMaterialsByMangementType(string code);
    }

    public class MaterialService : IMaterialService
    {
        private readonly UniContext _uniContext;
        public MaterialService(UniContext uniContext)
        {
            _uniContext = uniContext;
        }
        public async Task<IResult<MaterialDTO>> GetMaterial(string id)
        {
            var result = _uniContext.Materials.Where(m => m.ObjectID == id).Single();
            if(result != null)
            {
                var material = result.Adapt<MaterialDTO>();
                return await Result<MaterialDTO>.SuccessAsync(material);
            }
            else
            {
                return await Result<MaterialDTO>.FailAsync();
            }
        }

        public async Task<IResult<List<MaterialDTO>>> GetMaterialsByMangementType(string code)
        {
            var result = _uniContext.Materials.Where(m => m.ProductIdentifierTypeCode.Equals(code)).ToList();
            if (result != null)
            {
                var materials = result.Adapt<List<MaterialDTO>>();
                return await Result<List<MaterialDTO>>.SuccessAsync(materials);
            }
            else
            {
                return await Result<List<MaterialDTO>>.FailAsync();
            }
        }
    }
}
