using Infrastructure.Bydesign.OData.UniScan;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using RestSharp;
using Soap.Byd.QuerySiteLogisticsTaskIn;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uni.Scan.Infrastructure.ByDesign.Extentions;
using Uni.Scan.Infrastructure.ByDesign.Helper;
using Uni.Scan.Shared.Configurations;
using Uni.Scan.Shared.Extensions;
using Uni.Scan.Shared.Localizers;

namespace Uni.Scan.Infrastructure.ByDesign.Service
{
    public partial class BydLogisticTaskService : IBydService
    {
        private readonly BydesignConfiguration _config;
        private readonly IStringLocalizer<BackendLocalizer> _localizer;


        public BydLogisticTaskService(IOptions<BydesignConfiguration> config,
            IStringLocalizer<BackendLocalizer> localizer)
        {
            _config = config.Value;
            _localizer = localizer;
        }


        public async Task<SiteLogisticsTask> GetOdataTaskByOIDAsync(string objectID)
        {
            var oDataClient = BydServiceHelper.GetUniScanOdataClient(_config);

            var results = await oDataClient.SiteLogisticsTaskCollection
                .AddQueryOption("$expand", "ResponsibleEmployee,LogisticsRequest,LogisticsTaskFolder")
                .AddQueryOption("$filter", $"ObjectID eq '{objectID}'")
                .ExecuteAsync();
            return results.Item2.FirstOrDefault();
        }

        public async Task<SiteLogisticsTaskByElementsResponse_sync> GetSoapTaskByIDAsync(string taskID)
        {
            var client = new QuerySiteLogisticsTaskInClient(BydServiceHelper.BasicBinding,
                BydServiceHelper.QuerySiteLogisticsTaskInAdresse(_config.TenantId));

            client.ClientCredentials.UserName.UserName = _config.SoapUser;
            client.ClientCredentials.UserName.Password = _config.SoapPassword;


            var conditions = new QueryProcessingConditions()
                { QueryHitsUnlimitedIndicator = false, QueryHitsMaximumNumberValue = 10 };
            var query = new SiteLogisticsTaskByElementsQueryMessage
            {
                ProcessingConditions = conditions,
                SiteLogisticsTaskSelectionByElements = new SiteLogisticsTaskSelectionByElements
                {
                    SelectionBySiteLogisticsTaskID = new SiteLogisticsTaskSelectionByTaskId[]
                    {
                        new SiteLogisticsTaskSelectionByTaskId
                        {
                            IntervalBoundaryTypeCode = "1",
                            InclusionExclusionCode = "I",
                            LowerBoundarySiteLogisticsTaskID =
                                new Soap.Byd.QuerySiteLogisticsTaskIn.BusinessTransactionDocumentID
                                    { Value = taskID }
                        }
                    }
                }
            };

            var TaskResponse = await client.FindByElementsAsync(query);

            if (TaskResponse.SiteLogistcsTaskByElementsResponse_sync.SiteLogisticsTask != null)
            {
                return TaskResponse.SiteLogistcsTaskByElementsResponse_sync.SiteLogisticsTask.FirstOrDefault();
            }

            return null;
        }


        public async Task<bool> SetLogisticsTaskResponsibleAsync(string taskObjectID, string employeeID)
        {
            var oRestClient = BydServiceHelper.GetOdataRestClient(_config, _config.OdataServiceName);

            var ressource =
                $"SiteLogisticsTaskSetResponsible?ObjectID='{taskObjectID}'&ResponsibleEmployeeID='{employeeID}'";
            var result = await oRestClient.BydPostAsync(ressource);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> RemoveTaskResponsibleAsync(string taskObjectID)
        {
            var oRestClient = BydServiceHelper.GetOdataRestClient(_config, _config.OdataServiceName);

            var ressource = $"SiteLogisticsTaskRemoveResponsible?ObjectID='{taskObjectID}'";
            var result = await oRestClient.BydPostAsync(ressource);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }

        public async Task<List<ResponsibleEmployee>> GetResponsibleEmployeesAsync()
        {
            var oDataClient = BydServiceHelper.GetUniScanOdataClient(_config);

            var results = await oDataClient.ResponsibleEmployeeCollection.ExecuteAsync();
            return results.Item2.ToList();
        }


        public async Task<List<LogisticsArea>> GetLogisticsAreasAsync(int nbrtoskip = 0)
        {
            var areas = new List<LogisticsArea>();
            var oDataClient = BydServiceHelper.GetUniScanOdataClient(_config);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            while (nbrtoskip >= 0)
            {
                var results = await oDataClient.LogisticsAreaCollection
                    .AddQueryOption("$skip", nbrtoskip.ToString())
                    .ExecuteAsync();
                var otmpList = results.Item2.ToList();
                if (otmpList.IsNullOrEmpty() == false)
                {
                    areas.AddRange(otmpList);
                    nbrtoskip += otmpList.Count;
                }
                else
                {
                    nbrtoskip = -1;
                    break;
                }
            }

            return areas;
        }


        public async Task<List<SiteLogisticsTask>> GetOdataTasksAsync()
        {
            var oDataClient = BydServiceHelper.GetUniScanOdataClient(_config);

            List<SiteLogisticsTask> tasks = new List<SiteLogisticsTask>();

            var skip = 0;

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            while (skip >= 0)
            {
                var results = await oDataClient.SiteLogisticsTaskCollection
                    .AddQueryOption("$expand", "ResponsibleEmployee,LogisticsRequest,LogisticsTaskFolder")
                    .AddQueryOption("$filter", "ProcessingStatusCode eq '1' or ProcessingStatusCode eq '2'")
                    .AddQueryOption("$skip", skip.ToString())
                    .ExecuteAsync();

                var otmpList = results.Item2.ToList();
                if (otmpList.IsNullOrEmpty() == false)
                {
                    tasks.AddRange(otmpList);
                    skip += otmpList.Count;
                }
                else
                {
                    skip = -1;
                    break;
                }
            }

            return tasks;
        }
    }
}