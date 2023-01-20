using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Bydesign.OData.UniScan;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Uni.Scan.Infrastructure.ByDesign.Helper;
using Uni.Scan.Shared.Configurations;
using Uni.Scan.Shared.Localizers;

namespace Uni.Scan.Infrastructure.ByDesign.Service
{
    public class BydMaterialService : IBydService
    {
        private readonly BydesignConfiguration _config;
        private readonly IStringLocalizer<BackendLocalizer> _localizer;


        public BydMaterialService(IOptions<BydesignConfiguration> config,
            IStringLocalizer<BackendLocalizer> localizer)
        {
            _config = config.Value;
            _localizer = localizer;
        }

        public async Task<MaterialIdentification> GetMaterialAsync(string productID)
        {
            var oDataClient = BydServiceHelper.GetUniScanOdataClient(_config);

            var results = await oDataClient.MaterialIdentificationCollection
                .AddQueryOption("$expand", "Material")
                .AddQueryOption("$filter", $"ProductID eq '{productID}'")
                .ExecuteAsync();
            return results.Item2?.FirstOrDefault();
        }
        
    }
}