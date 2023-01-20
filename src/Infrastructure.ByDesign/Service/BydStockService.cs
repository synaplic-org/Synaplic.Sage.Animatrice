using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Soap.Byd.InventoryProcessingGood2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Uni.Scan.Infrastructure.ByDesign.BydResponses;
using Uni.Scan.Infrastructure.ByDesign.Extentions;
using Uni.Scan.Infrastructure.ByDesign.Helper;
using Uni.Scan.Shared.Configurations;
using Uni.Scan.Shared.Extensions;
using Uni.Scan.Shared.Localizers;
using Uni.Scan.Shared.Wrapper;
using Uni.Scan.Transfer.DataModel;
using Uni.Scan.Transfer.Requests;

namespace Uni.Scan.Infrastructure.ByDesign.Service
{
    public class BydStockService : IBydService
    {
        private readonly BydesignConfiguration _config;
        private readonly IStringLocalizer<BackendLocalizer> _localizer;


        public BydStockService(IOptions<BydesignConfiguration> config,
            IStringLocalizer<BackendLocalizer> localizer)
        {
            _config = config.Value;
            _localizer = localizer;
        }


        public async Task<IResult<List<StockOverViewDTO>>> GetStockOverViewAsync(string SiteID,
            string LogisticAreaID, string ProductID, string IdentifiedStockID)
        {
            var oQuery = $"$format=json";
            var ofilter = string.Empty;
            var oAnd = " and ";

            if (!string.IsNullOrWhiteSpace(SiteID) && SiteID != "*")
            {
                if (ofilter.Length > 0) ofilter += oAnd;
                ofilter += $"(CSITE_UUID eq  '{SiteID}')";
            }

            if (!string.IsNullOrWhiteSpace(LogisticAreaID))
            {
                if (ofilter.Length > 0) ofilter += oAnd;
                ofilter += $"(CLOG_AREA_UUID eq '{SiteID}/{LogisticAreaID}')";
            }

            if (!string.IsNullOrWhiteSpace(ProductID))
            {
                if (ofilter.Length > 0) ofilter += oAnd;
                ofilter += $"(CMATERIAL_UUID eq '{ProductID}')";

                if (!string.IsNullOrWhiteSpace(IdentifiedStockID))
                {
                    if (ofilter.Length > 0) ofilter += oAnd;
                    ofilter += $"(CISTOCK_UUID eq '{ProductID}/{IdentifiedStockID}')";
                }
            }

            if (ofilter.Length > 0) ofilter = "&$filter=" + ofilter;

            var oDataClient = BydServiceHelper.GetAnaliticsRestClient(_config, _config.StockOverViewReportID);

            var ressource = $"{_config.StockOverViewReportID}QueryResults?" + oQuery + ofilter;

            var response = await oDataClient.BydGetAsync(ressource);
            if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrWhiteSpace(response.Content))
            {
                var result = BydStockOverViewResponse.FromJson(response.Content);
                var adaptedDtos = result.D.Results.Select(o => new StockOverViewDTO()
                {
                    CompanyID = o.CcoUuid,
                    SiteID = o.CsiteUuid,
                    ProductID = o.CmaterialUuid,
                    IdentifiedStockID = o.CistockUuid.Replace($"{o.CmaterialUuid}/", ""),
                    Quantity = o.FconHandStock.Replace(".", "").ToDecimal(),
                    QuantityUniteCode = o.ConHandStockUom,
                    IdentifiedStockType = o.TistockType,
                    IdentifiedStockTypeCode = o.CistockType,
                    InventoryRestrictedUseIndicator = (o.CrestrictedInd == true),
                    InventoryInInspectionIndicator = (o.CinspectionInd == true),
                    InventoryStockStatusCode = o.CinvStockStatusCode,
                    LogisticsAreaID = o.ClogAreaUuid.Replace($"{o.CsiteUuid}/", ""),
                    LogisticsArea = o.ClogAreaUuid,
                    OwnerPartyID = o.CownerUuid,
                    ExprirationDate = o.CexpDate ?? ""
                }).ToList();
                return await Result<List<StockOverViewDTO>>.SuccessAsync(adaptedDtos);
            }
            else
            {
                return await Result<List<StockOverViewDTO>>.FailAsync(response.ErrorMessage);
            }
        }


        public async Task<IResult<bool>> MoveItemStock(InventoryItemMovementRequest request)
        {
            var client =
                new InventoryProcessingGoodsAndActivityConfirmationGoodsMovementInClient(BydServiceHelper.BasicBinding, BydServiceHelper.InventoryProcessingGoodsAndAc2Adresse(_config.TenantId));

            client.ClientCredentials.UserName.UserName = _config.SoapUser;
            client.ClientCredentials.UserName.Password = _config.SoapPassword;

            try
            {
                request.ExternalID = "E" + DateTime.Now.ToString("yyMMdd");
                request.ExternalItemID = "EXT01";

                var mouvement = new InventoryChangeItemGoodsMovement()
                {
                    IdentifiedStockID = new IdentifiedStockID() { Value = request.IdentifiedStockID },
                    ExternalItemID = request.ExternalItemID,
                    TargetLogisticsAreaID = request.TargetLogisticsAreaID,
                    SourceLogisticsAreaID = request.SourceLogisticsAreaID,
                    InventoryRestrictedUseIndicator = request.InventoryRestrictedUseIndicator,
                    InventoryStockStatusCodeSpecified = false,
                    MaterialInternalID = new ProductInternalID() { Value = request.MaterialInternalID },
                    OwnerPartyInternalID = new PartyInternalID()
                        { Value = request.OwnerPartyInternalID },
                    InventoryItemChangeQuantity =
                        new
                            GoodsAndActivityConfirmationInventoryChangeInventoryChangeItemInventoryItemChangeQuantity()
                            {
                                Quantity = new Quantity()
                                    { unitCode = request.QuantityTypeCode, Value = request.Quantity },
                                QuantityTypeCode = new QuantityTypeCode()
                                    { Value = request.QuantityTypeCode }
                            }
                };

                if (!string.IsNullOrWhiteSpace(request.InventoryStockStatusCode))
                {
                    if (request.InventoryStockStatusCode.Equals("1"))
                    {
                        mouvement.InventoryStockStatusCode = InventoryStockStatusCode.Item1;
                        mouvement.InventoryStockStatusCodeSpecified = true;
                    }

                    if (request.InventoryStockStatusCode.Equals("2"))
                    {
                        mouvement.InventoryStockStatusCode = InventoryStockStatusCode.Item2;
                        mouvement.InventoryStockStatusCodeSpecified = true;
                    }
                }


                var result = await client.DoGoodsMovementGoodsAndActivityConfirmationAsync(
                    new GoodsAndActivityConfirmationGoodsMoveGAC[]
                    {
                        new GoodsAndActivityConfirmationGoodsMoveGAC()
                        {
                            ExternalID = new BusinessTransactionDocumentID() { Value = request.ExternalID },
                            SiteID = new LocationID() { Value = request.SiteID },
                            TransactionDateTimeSpecified = false,
                            InventoryChangeItemGoodsMovement = new InventoryChangeItemGoodsMovement[]
                            {
                                mouvement
                            }
                        }
                    });

                if (result.GoodsAndActivityConfoirmationGoodsMovementResponse?.GACDetails != null)
                {
                    var warnings =
                        result.GoodsAndActivityConfoirmationGoodsMovementResponse?.Log?.Item?.Select(o => o.Note) ??
                        Enumerable.Empty<string>();
                    return await Shared.Wrapper.Result<bool>.SuccessAsync(true, warnings.ToList());
                }
                else if (result.GoodsAndActivityConfoirmationGoodsMovementResponse?.Log?.Item?.Length > 0)
                {
                    var erros = result.GoodsAndActivityConfoirmationGoodsMovementResponse.Log.Item.Select(o => o.Note)
                        .ToList();
                    if (erros.IsNullOrEmpty() == false)
                    {
                        return await Shared.Wrapper.Result<bool>.FailAsync(erros);
                    }
                }
            }
            catch (Exception e)
            {
                return await Result<bool>.FailAsync(e.Message);
            }

            return await Shared.Wrapper.Result<bool>.SuccessAsync(true);
        }
    }
}