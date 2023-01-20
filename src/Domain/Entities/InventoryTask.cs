using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uni.Scan.Domain.Entities
{
    public partial class InventoryTask
    {

        public InventoryTask()
        {
            InventoryTaskOperations = new HashSet<InventoryTaskOperation>();
           
        }

        [Key]
        public string ObjectID { get; set; }
        public string Number { get; set; }
        public string ResponsibleId { get; set; }
        public string ProcessingStatus { get; set; }
        public string PriorityCode { get; set; }
        public string Notes { get; set; }
        public string OperationType { get; set; }
        public string OperationTypeCode { get; set; }
        public DateTime EndDateTime { get; set; }
        public DateTime CreationDateTime { get; set; }
        public string SiteId { get; set; }
        public string SiteName { get; set; }



        public virtual ICollection<InventoryTaskOperation> InventoryTaskOperations { get; set; }
    }
}
