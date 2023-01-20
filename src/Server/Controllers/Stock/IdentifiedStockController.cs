using Infrastructure.Bydesign.OData.UniScan;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Mapster;
using Uni.Scan.Infrastructure.ByDesign.Service;
using Uni.Scan.Transfer.DataModel;

namespace Uni.Scan.Server.Controllers.Stock
{
    public class IdentifiedStockController : BaseApiController<IdentifiedStockController>
    {
        private readonly BydIdentifiedStockService _byDService;

        public IdentifiedStockController(BydIdentifiedStockService byDService)
        {
            _byDService = byDService;
        }
        //[HttpPost(nameof(CreateIdentifiedStock))]
        //public async Task<bool> CreateStockID(string  ProductID, string ID)
        //{ 
        //    return await _byDService.CreateStockID(ProductID, ID);
        //}

        [HttpPost(nameof(CreateIdentifiedStock))]
        public async Task<bool> CreateIdentifiedStock(IdentifiedStockDTO request)
        {
            return await _byDService.CreateStock(request);
        }
        [HttpGet(nameof(GetStockIDAsync))]
        public async Task<IdentifiedStockDTO> GetStockIDAsync(string stockID, string ProductID)
        {
            var result = await _byDService.GetStockIDAsync(stockID, ProductID);
            return result.Adapt<IdentifiedStockDTO>();
        }
    }
}
