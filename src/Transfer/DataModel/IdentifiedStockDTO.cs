using System;

namespace Uni.Scan.Transfer.DataModel
{
    public partial class IdentifiedStockDTO
    {
        public string ID { get; set; }
        public string ProductID { get; set; }
        public string ObjectID { get; set; }
        public string IdentifiedStockID { get; set; }
        public string IdentifiedStockTypeCode { get; set; }
        public string IdentifiedStockTypeCodeText { get; set; }
        public string IdentifiedStockPartyID { get; set; }
        public DateTime ExpirationDateTime { get; set; }
        public DateTime ProductionDateTime { get; set; }
        public string MaterialUUID { get; set; }
        public string UUID { get; set; } 
        public string LifeCycleStatusCode { get; set; }
        public string LifeCycleStatusCodeText { get; set; }
        public string PartyTypeCode { get; set; }
        public string PartyTypeCodeText { get; set; }
        public string ProductRequirementSpecificationVersionUUID { get; set; }
        public string ProductTypeCode { get; set; }
        public string ProductTypeCodeText { get; set; }
        public string ProductValuationLevelTypeCode { get; set; }
        public string ProductValuationLevelTypeCodeText { get; set; }
        public string ProductValuationLevelUUID { get; set; }
    }
}
