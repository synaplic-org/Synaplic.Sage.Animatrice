using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Uni.Scan.Infrastructure.ByDesign.Service;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.DataModel;
using Uni.Scan.Transfer.Requests;

namespace Uni.Scan.Server.Controllers.Stock
{
    public class StockController : BaseApiController<StockController>
    {
        private readonly BydStockService _bydStockService;

        public StockController(BydStockService bydStockService)
        {
            _bydStockService = bydStockService;
        }

        [HttpPost(nameof(MoveItemStock))]
        public async Task<IResult<bool>> MoveItemStock(InventoryItemMovementRequest request)
        {
            return await _bydStockService.MoveItemStock(request);
        }


        [HttpGet(nameof(GetStockOverViewListAsync))]
        public async Task<IResult<List<StockOverViewDTO>>> GetStockOverViewListAsync(string siteId,
            string logisticsArea, string productId, string identifiedStockId)
        {
            return await _bydStockService.GetStockOverViewAsync(siteId, logisticsArea, productId, identifiedStockId);
        }
    }
}