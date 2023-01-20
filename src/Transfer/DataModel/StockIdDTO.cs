using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uni.Scan.Transfer.DataModel
{
    public class StockIdDTO
    {
        public string ObjectID { get; set; }
        public string ID { get; set; }
        public string IdentifiedStockTypeCode { get; set; }
        public string IdentifiedStockTypeCodeText { get; set; }
        public string IdentifiedStockPartyID { get; set; }
        public string ProductTypeCode { get; set; }
        public string ProductTypeCodeText { get; set; }
        public string ProductID { get; set; }
        public string MaterialUUID { get; set; }
        public string UUID { get; set; }
        public string LifeCycleStatusCode { get; set; }
        public string LifeCycleStatusCodeText { get; set; }
        public string PartyTypeCode { get; set; }
        public string PartyTypeCodeText { get; set; }
    }
}
