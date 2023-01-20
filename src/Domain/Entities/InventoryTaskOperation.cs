using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uni.Scan.Domain.Entities
{
    public partial class InventoryTaskOperation
    {
        public InventoryTaskOperation()
        {
            InventoryTaskItems = new HashSet<InventoryTaskItem>();
        }

        [Key]
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

        public virtual InventoryTask Task { get; set; }
        public virtual ICollection<InventoryTaskItem> InventoryTaskItems { get; set; }
    }

}
