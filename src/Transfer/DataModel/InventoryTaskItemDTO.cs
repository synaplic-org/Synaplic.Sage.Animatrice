using System.Text.Json.Serialization;

namespace Uni.Scan.Transfer.DataModel
{
    public partial class InventoryTaskItemDTO
    {
        public int Id { get; set; }
        public string ObjectID { get; set; }
        public string OperationObjectID { get; set; }

        public string QuantitObjectID { get; set; }
        public string TaskID { get; set; }
        public string ProductID { get; set; }
        public string LogisticPackageUUID { get; set; }
        public string IdentifiedStockID { get; set; }
        public string CountedQuantityUnitCode { get; set; }
        public int CountCounterValue { get; set; }
        public string DeviationReasonCode { get; set; }
        public bool ForceSkippedIndicator { get; set; }
        public bool IncludeIndicator { get; set; }
        public int InventoryItemNumberValue { get; set; }
        public string ApprovalResultStatusCode { get; set; }
        public string CountApprovalStatusCode { get; set; }
        public decimal ApprovalDiscrepancyPercent { get; set; }
        public string CountedQuantityTypeCode { get; set; }
        public bool ZeroCountedQuantityConfirmedIndicator { get; set; }
        public decimal CountedQuantity { get; set; }
        public decimal BookInventoryQuantity { get; set; }
        public bool IsStockIDError { get; }
        public bool StockIDErrorText { get; }
        public bool Disabled { get; }

        [JsonIgnore]
        public bool isNewItem
        {
            get { return string.IsNullOrWhiteSpace(ObjectID); }
        }
    }
}