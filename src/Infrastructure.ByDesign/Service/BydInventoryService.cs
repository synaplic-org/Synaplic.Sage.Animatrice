using Infrastructure.Bydesign.OData.UniScan;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using RestSharp;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Uni.Scan.Infrastructure.ByDesign.Extentions;
using Uni.Scan.Infrastructure.ByDesign.Helper;
using Uni.Scan.Infrastructure.ByDesign.Requests.PhysicalInventory;
using Uni.Scan.Shared.Configurations;
using Uni.Scan.Shared.Extensions;
using Uni.Scan.Shared.Localizers;

namespace Uni.Scan.Infrastructure.ByDesign.Service
{
    public class BydInventoryService : IBydService
    {
        private readonly BydesignConfiguration _config;
        private readonly IStringLocalizer<BackendLocalizer> _localizer;


        public BydInventoryService(IOptions<BydesignConfiguration> config,
            IStringLocalizer<BackendLocalizer> localizer)
        {
            _config = config.Value;
            _localizer = localizer;
        }


        public async Task<List<PhysicalInventoryTask>> GetPhysicalInventoryTasksAsync()
        {
            var oDataClient = BydServiceHelper.GetUniScanOdataClient(_config);
            var inventries = new List<PhysicalInventoryTask>();
            var nbrtoskip = 0;
            while (nbrtoskip >= 0)
            {
                var results = await oDataClient.PhysicalInventoryTaskCollection
                    .AddQueryOption("$filter", "ProcessingStatusCode eq '1' or ProcessingStatusCode eq '2'")
                    .AddQueryOption("$expand", "PhysicalInventoryTaskReferencedObject," +
                                               "PhysicalInventoryTaskReferencedObject/PhysicalInventoryCountPhysicalInventoryCountOperationActivity," +
                                               "PhysicalInventoryTaskReferencedObject/PhysicalInventoryCountPhysicalInventoryCountOperationActivity/PhysicalInventoryCountOperationActivityCountInventory," +
                                               "PhysicalInventoryTaskReferencedObject/PhysicalInventoryCountPhysicalInventoryCountOperationActivity/PhysicalInventoryCountOperationActivityCountInventory/PhysicalInventoryCountOperationActivityInventoryItem," +
                                               "PhysicalInventoryTaskReferencedObject/PhysicalInventoryCountPhysicalInventoryCountOperationActivity/PhysicalInventoryCountOperationActivityCountInventory/PhysicalInventoryCountOperationActivityInventoryItem/PhysicalInventoryCountOperationActivityInventoryItemQuantity")
                    .AddQueryOption("$skip", nbrtoskip.ToString())
                    .ExecuteAsync();

                var otmpList = results.Item2.ToList();
                if (otmpList.IsNullOrEmpty() == false)
                {
                    inventries.AddRange(otmpList);
                    nbrtoskip += otmpList.Count;
                }
                else
                {
                    nbrtoskip = -1;
                    break;
                }
            }

            return inventries;
        }


        public async Task<bool> SetInevntoryTaskResponsibleAsync(string taskObjectId, string employeeId)
        {
            var oRestClient = BydServiceHelper.GetOdataRestClient(_config, _config.OdataServiceName);
            var ressource = $"PhysicalInventoryTaskSetResponsible?ObjectID='{taskObjectId}'&ResponsibleEmployeeID='{employeeId}'";
            var result = await oRestClient.BydPostAsync(ressource);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> PhysicalInventoryTaskFinish(string taskObjectId)
        {
            var oRestClient = BydServiceHelper.GetOdataRestClient(_config, _config.OdataServiceName);
            var ressource = $"PhysicalInventoryTaskFinish?ObjectID='{taskObjectId}'";
            var result = await oRestClient.BydPostAsync(ressource);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> RemoveInevntoryTaskResponsibleAsync(string taskObjectId)
        {
            var oRestClient = BydServiceHelper.GetOdataRestClient(_config, _config.OdataServiceName);

            var ressource = $"PhysicalInventoryTaskSetResponsible?ObjectID='{taskObjectId}'";
            var result = await oRestClient.BydPostAsync(ressource);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }

        public async Task<PhysicalInventoryTask> GetPhysicalInventoryTaskAsync(string taskId)
        {
            var oDataClient = BydServiceHelper.GetUniScanOdataClient(_config);

            var results = await oDataClient.PhysicalInventoryTaskCollection
                .AddQueryOption($"$filter", $"ID eq '{taskId}'")
                .ExecuteAsync();
            return results.Item2.FirstOrDefault();
        }

        public async Task<PhysicalInventoryTask> GetPhysicalInventoryTaskByIDAsync(string taskObjectId)
        {
            var oDataClient = BydServiceHelper.GetUniScanOdataClient(_config);

            var results = await oDataClient.PhysicalInventoryTaskCollection
                .AddQueryOption($"$filter", $"ObjectID eq '{taskObjectId}'")
                .ExecuteAsync();
            return results.Item2.FirstOrDefault();
        }

        public async Task<bool> UpdateInventoryTaskItem(string itemObjectId, decimal itemCountedQuantity,
            bool itemZeroCountedQuantityConfirmedIndicator)
        {
            var oRestClient = BydServiceHelper.GetOdataRestClient(_config, _config.OdataServiceName);
            var ressource = $"PhysicalInventoryCountOperationActivityInventoryItemQuantityCollection('{itemObjectId}')";

            var json = (new UpdateQuantityRequest
            {
                ObjectID = itemObjectId,
                CountedQuantity = itemCountedQuantity.ToString("G", CultureInfo.InvariantCulture),
                ZeroCountedQuantityConfirmedIndicator = itemZeroCountedQuantityConfirmedIndicator
            }).ToJson();
            var result = await oRestClient.BydPatchJsonAsync(ressource, json);
            if (result.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return true;
            }

            return false;
        }
    }
}