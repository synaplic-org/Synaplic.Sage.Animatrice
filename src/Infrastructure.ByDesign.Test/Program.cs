// See https://aka.ms/new-console-template for more information


using System.Diagnostics;
using Infrastructure.Bydesign.OData.UniScan;
using Uni.Scan.Infrastructure.ByDesign.Helper;
using Uni.Scan.Shared.Configurations;
using Uni.Scan.Shared.Extensions;
using Uni.Scan.Shared.Localizers;

Console.WriteLine("Hello, World!");

BydesignConfiguration _config = new BydesignConfiguration()
{
    TenantId = "my429899.businessbydesign.cloud.sap",
    OdataUser = "ahmed",
    OdataPassword = "Welcome1Welcome2",
    SoapUser = "_UNIVERSCAN",
    SoapPassword = "Azur@222324",
    StockOverViewReportID = "RPZD2DC8C8E3F747EF0E0B51B",
    OdataServiceName = "uniscan"
};


var wtch = new Stopwatch();
wtch.Start();

var areas = await LogisticsAreas(_config);
var tasks = await SiteLogisticsTasks(_config);
var ss = PhysicalInventoryTasks(_config);

wtch.Stop();
Console.WriteLine(wtch.ElapsedMilliseconds);


Console.ReadKey();


async Task<List<SiteLogisticsTask>> SiteLogisticsTasks(BydesignConfiguration config)
{
    var oDataClient = BydServiceHelper.GetUniScanOdataClient(config);

    List<SiteLogisticsTask> siteLogisticsTasks = new List<SiteLogisticsTask>();

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
            siteLogisticsTasks.AddRange(otmpList);
            skip += otmpList.Count;
        }
        else
        {
            skip = -1;
            break;
        }
    }

    return siteLogisticsTasks;
}

async Task<List<LogisticsArea>> LogisticsAreas(BydesignConfiguration bydesignConfiguration)
{
    var logisticsAreas = new List<LogisticsArea>();
    var oDataClient = BydServiceHelper.GetUniScanOdataClient(bydesignConfiguration);

// ReSharper disable once ConditionIsAlwaysTrueOrFalse
    int nbrtoskip = 0;
    while (nbrtoskip >= 0)
    {
        var results = await oDataClient.LogisticsAreaCollection
            .AddQueryOption("$skip", nbrtoskip.ToString())
            .ExecuteAsync();
        var otmpList = results.Item2.ToList();
        if (otmpList.IsNullOrEmpty() == false)
        {
            logisticsAreas.AddRange(otmpList);
            nbrtoskip += otmpList.Count;
        }
        else
        {
            nbrtoskip = -1;
            break;
        }
    }

    return logisticsAreas;
}

async Task<List<PhysicalInventoryTask>> PhysicalInventoryTasks(BydesignConfiguration config1)
{
    var oDataClient = BydServiceHelper.GetUniScanOdataClient(config1);
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