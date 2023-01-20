using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Uni.Scan.Transfer.DataModel.LogisticTask
{
    public class LogisticTaskItemDTO2
    {
        public LogisticTaskItemDTO2()
        {
            Details = new();
            Lables = new();
        }

        public int LineItemID { get; set; }
        public string ProductID { get; set; }
        public string ProductDescription { get; set; }
        public decimal PlanQuantity { get; set; }
        public string PlanQuantityUnitCode { get; set; }
        public decimal TotalQuantity => Details.Sum(o => o.ConfirmQuantity + o.TotalConfirmedQuantity);

        public decimal SumOpenQuantity => Details.Sum(o => o.OpenQuantity);
        public decimal SumConfirmQuantity => Details.Sum(o => o.ConfirmQuantity);

        public bool DeviationFound { get; set; }
        public MaterialDTO Material { get; set; }
        public List<LogisticTaskItemDetailDTO2> Details { get; set; }


        [JsonIgnore] public bool IsBatchManaged => Material is { IsBatchManaged: true };

        [JsonIgnore] public bool IsSerialManaged => Material is { IsSerialManaged: true };

        public List<string> Errors { get; set; }
        public List<LogisticTaskLabelDTO2> Lables { get; set; }
    }
}