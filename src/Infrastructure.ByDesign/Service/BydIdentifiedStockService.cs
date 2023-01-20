using Infrastructure.Bydesign.OData.UniScan;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Uni.Scan.Infrastructure.ByDesign.Extentions;
using Uni.Scan.Infrastructure.ByDesign.Helper;
using Uni.Scan.Shared.Configurations;
using Uni.Scan.Shared.Localizers;
using Uni.Scan.Transfer.DataModel;

namespace Uni.Scan.Infrastructure.ByDesign.Service
{
    public class BydIdentifiedStockService : IBydService
    {
        private readonly BydesignConfiguration _config;
        private readonly IStringLocalizer<BackendLocalizer> _localizer;

        public BydIdentifiedStockService(IOptions<BydesignConfiguration> config, IStringLocalizer<BackendLocalizer> localizer)
        {
            _config = config.Value;
            _localizer = localizer;
        }

        public async Task<IdentifiedStock> GetStockIDAsync(string stockID, string ProductID)
        {
            var oDataClient = BydServiceHelper.GetUniScanOdataClient(_config);

            var results = await oDataClient.IdentifiedStockCollection
                .AddQueryOption("$filter", $"ID eq '{stockID}' and ProductID eq '{ProductID}'")
                .ExecuteAsync();

            return results.Item2.FirstOrDefault();
        }

        public async Task<bool> CreateStockID(string identifiedStock, string productId, string supplierIdentifiedStock, DateTime? expirationDate, DateTime? productionDate)
        {
            var oRestClient = BydServiceHelper.GetOdataRestClient(_config, _config.OdataServiceName);

            var ressource = $"IdentifiedStockCollection";
            var obj = new
            {
                ID = identifiedStock,
                ProductID = productId,
                LifeCycleStatusCode = "2",
                IdentifiedStockPartyID = supplierIdentifiedStock,
                ExpirationDateTime = expirationDate,
                ProductionDateTime = productionDate
            };


            var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver(),
            });

            //var str = $"{{\"ID\": \"{identifiedStock}\",\"ProductID\": \"{productId}\",\"LifeCycleStatusCode\": \"2\"}}";
            var result = await oRestClient.BydPostJsonAsync(ressource, json);
            if (result.StatusCode == System.Net.HttpStatusCode.Created)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateStockID(string identifiedStockUiid, string supplierIdentifiedStock, DateTime? expirationDate, DateTime? productionDate)
        {
            var oRestClient = BydServiceHelper.GetOdataRestClient(_config, _config.OdataServiceName);

            var ressource = $"IdentifiedStockCollection('{identifiedStockUiid}')";
            var obj = new
            {
                IdentifiedStockPartyID = supplierIdentifiedStock,
                ExpirationDateTime = expirationDate,
                ProductionDateTime = productionDate
            };


            var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver(),
            });
            var result = await oRestClient.BydPatchJsonAsync(ressource, json);
            if (result.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return true;
            }
            else
            {
                Console.WriteLine(result.Content);
            }

            return false;
        }

        public async Task<bool> CreateStock(IdentifiedStockDTO request)
        {
            var oRestClient = BydServiceHelper.GetOdataRestClient(_config, _config.OdataServiceName);

            var ressource = $"IdentifiedStockCollection";

            var str = $"{{\"ID\": \"{request.ID}\",\"ProductID\": \"{request.ProductID}\",\"LifeCycleStatusCode\": \"2\"}}";
            var result = await oRestClient.BydPostJsonAsync(ressource, str);
            if (result.StatusCode == System.Net.HttpStatusCode.Created)
            {
                return true;
            }

            return false;
        }

        private class StockIDRequest
        {
            [JsonProperty("ID")] public string ID { get; set; }

            [JsonProperty("ProductID")] public string ProductID { get; set; }
        }
    }
}