using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uni.Scan.Transfer.DataModel
{
    public class LogisticsLineDTO
    {
        public int LineItemID { get; set; }
        public string ProductID { get; set; }
        public string ProductDescription { get; set; }
        public Decimal TotalOpenQuantity => Items?.Sum(o => o.OpenQuantity) ?? 0;
        public Decimal TotalConfirmQuantity => Items?.Sum(o => o.ConfirmQuantity + o.TotalConfirmedQuantity) ?? 0;
        public List<LogisticTaskDetailDTO> Items { get; set; }
        public decimal PlanQuantity { get; set; }
        public string PlanQuantityUnitCode { get; set; }
        public MaterialDTO? Matrial { get; set; }

        public bool ShowStockID => (Matrial != null && (Matrial.ProductIdentifierTypeCode == "01" || Matrial.ProductIdentifierTypeCode == "02" || Matrial.ProductIdentifierTypeCode == "04"));
        public bool ShowSerials => (Matrial != null && (Matrial.SerialIdentifierAssignmentProfileCode == "1003"));
    }
}
