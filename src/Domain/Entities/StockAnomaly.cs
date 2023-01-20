using System;
using Uni.Scan.Domain.Contracts;
using Uni.Scan.Shared.Enums;

namespace Uni.Scan.Domain.Entities
{
    public class StockAnomaly : AuditableEntity<int>
    {
        public AnomalyType AnomalyType { get; set; }
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
        public string CorrectedIdentifiedStockID { get; set; }
        public decimal CorrectedQuantity { get; set; }
        public string AnomalyReason { get; set; }
        public AnomalyStatus AnomalyStatus { get; set; }
        public string DeclaredBy { get; set; }
        public string ClosedBy { get; set; }
        public DateTime? CloseOn { get; set; }
    }
}
