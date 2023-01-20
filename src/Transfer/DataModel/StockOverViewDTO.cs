using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uni.Scan.Transfer.DataModel
{
    public class StockOverViewDTO
    {
        public string CompanyID { get; set; }
        public string SiteID { get; set; }
        public string OwnerPartyID { get; set; }
        public bool InventoryRestrictedUseIndicator { get; set; }
        public string InventoryStockStatusCode { get; set; }
        public string IdentifiedStockID { get; set; }
        public string LogisticsAreaID { get; set; }
        public decimal Quantity { get; set; }
        public string QuantityUniteCode { get; set; }

        public string IdentifiedStockType { get; set; }
        public string IdentifiedStockTypeCode { get; set; }
        public string LogisticsArea { get; set; }
        public string ProductID { get; set; }

        public string ProductDescription { get; set; }
        public bool InventoryInInspectionIndicator { get; set; }
        public string ExprirationDate { get; set; }
    }
}