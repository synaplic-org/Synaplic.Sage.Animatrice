using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uni.Scan.Shared.Configurations
{
    public class BydesignConfiguration
    {
        public string TenantId { get; set; }
        public string OdataUser { get; set; }
        public string OdataPassword { get; set; }
        public string SoapUser { get; set; }
        public string SoapPassword { get; set; }
        public string StockOverViewReportID { get; set; }
        public string LogisticsDeviationReasonCode { get; set; }

        public string OdataServiceName { get; set; }
        //public string InventoryServiceName { get; set; }
    }
}