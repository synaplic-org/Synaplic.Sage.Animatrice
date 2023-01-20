using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uni.Scan.Domain.Contracts;

namespace Uni.Scan.Domain.Entities
{
    public partial class InventoryTaskItem : AuditableEntity<int>
    {
        public string ObjectID { get; set; }
        public string OperationObjectID { get; set; }
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


        public virtual InventoryTaskOperation Operation { get; set; }
        public string QuantitObjectID { get; set; }
    }
}