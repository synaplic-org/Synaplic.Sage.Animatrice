using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Nodes;
using Newtonsoft.Json;
using Uni.Scan.Shared.Extensions;

namespace Uni.Scan.Transfer.DataModel
{
    public partial class InventoryTaskOperationDTO
    {
        public InventoryTaskOperationDTO()
        {
            InventoryTaskItems = new List<InventoryTaskItemDTO>();
        }

        public string ObjectID { get; set; }
        public string TaskObjectID { get; set; }
        public string ParentObjectID { get; set; }
        public string SiteID { get; set; }
        public string LogisticsAreaUUID { get; set; }
        public string ApprovalProcessingStatusCode { get; set; }
        public string CountLifeCycleStatusCode { get; set; }
        public string LogisticsAreaID { get; set; }
        public string InventoryManagedLocationID { get; set; }
        public string InventoryManagedLocationUUID { get; set; }
        public string LogisticsAreaTypeCode { get; set; }
        public string LogisticsAreaLifeCycleStatusCode { get; set; }

        [JsonIgnore]
        public int Progress
        {
            get
            {
                if (InventoryTaskItems.IsNullOrEmpty()) return 0;
                return (int)(InventoryTaskItems.Count(o =>
                    o.ZeroCountedQuantityConfirmedIndicator || o.CountedQuantity > 0) * 100 / InventoryTaskItems.Count);
            }
        }

        public List<InventoryTaskItemDTO> InventoryTaskItems { get; set; }
    }
}