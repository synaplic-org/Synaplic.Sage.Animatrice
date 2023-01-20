using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Uni.Scan.Reports.Models
{
    public class TaskLine
    {
        public int LineItemID { get; set; }
        public string ProductID { get; set; }
        public byte[] ProductIDBarcode { get; set; }
        public string ProductDescription { get; set; }
        public decimal PlanQuantity { get; set; }
        public string PlanQuantityUnitCode { get; set; }
        public string IdentifiedStockID { get; set; }


    }
}