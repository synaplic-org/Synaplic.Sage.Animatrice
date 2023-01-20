using System;

namespace Uni.Scan.Transfer.Requests;

public class InventoryItemMovementRequest
{
    public string ExternalID { get; set; }
    public string SiteID { get; set; }
    public DateTime TransactionDateTime { get; set; }
    public string ExternalItemID { get; set; }
    public string MaterialInternalID { get; set; }
    public string OwnerPartyInternalID { get; set; }
    public bool InventoryRestrictedUseIndicator { get; set; }
    public string InventoryStockStatusCode { get; set; }
    public string IdentifiedStockID { get; set; }
    public string SourceLogisticsAreaID { get; set; }
    public string TargetLogisticsAreaID { get; set; }
    public decimal Quantity { get; set; }
    public string QuantityTypeCode { get; set; }
}