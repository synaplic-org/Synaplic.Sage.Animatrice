using System;
using System.Collections.Generic;

namespace Uni.Scan.Transfer.DataModel
{
    public partial class InventoryTaskDTO
    {

        public InventoryTaskDTO()
        {
            InventoryTaskOperations = new List<InventoryTaskOperationDTO>();
           
        }

    
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



        public virtual List<InventoryTaskOperationDTO> InventoryTaskOperations { get; set; }
    }
}
