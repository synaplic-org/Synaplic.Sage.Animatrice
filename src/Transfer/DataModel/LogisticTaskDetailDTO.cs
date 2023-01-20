using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Uni.Scan.Transfer.DataModel
{
    public class LogisticTaskDetailDTO
    {
        public LogisticTaskDetailDTO()
        {
            Lables = new();
        }

        public string OutputUUID { get; set; }
        public string InputUUID { get; set; }
        public string TaskId { get; set; }
        public int LineItemID { get; set; }
        public string ProductID { get; set; }
        public string SourceLogisticsAreaID { get; set; }
        public string TargetLogisticsAreaID { get; set; }
        public string ProductDescription { get; set; }
        public decimal PlanQuantity { get; set; }
        public string PlanQuantityUnitCode { get; set; }
        public decimal OpenQuantity { get; set; }
        public string OpenQuantityUnitCode { get; set; }
        public decimal ConfirmQuantity { get; set; }
        public string ConfirmQuantityUnitCode { get; set; }
        public decimal TotalConfirmedQuantity { get; set; }
        public string TotalConfirmedQuantityUnitCode { get; set; }
        public string IdentifiedStockID { get; set; }
        public string PackageLogisticUnitUUID { get; set; }
        public decimal PackageLogisticUnitTotalConfirmedQuantity { get; set; }
        public decimal PackageLogisticUnitOpenQuantity { get; set; }
        public string PackageLogisticUnitTotalConfirmedQuantityUnitCode { get; set; }
        public string PackageLogisticUnitPlanQuantityUnitCode { get; set; }
        public string MaterialDeviationStatusCode { get; set; }
        public string MaterialInspectionID { get; set; }

        //  public string MaterialInspectionSkippingStatusCode { get; set; }
        public decimal PackageLogisticUnitPlanQuantity { get; set; }
        public string PackageLogisticUnitOpenQuantityUnitCode { get; set; }
        public bool MaterialDeviationStatusCodeSpecified { get; set; }
        public string LogisticsDeviationReasonCode { get; set; }
        public bool LineItemIDSpecified { get; set; }
        public string ProductSpecificationID { get; set; }
        public bool RestrictedIndicator { get; set; }
        public string ProductRequirementSpecificationDescription { get; set; }
        public string SerialNumberAssignments { get; set; }

        [JsonIgnore] public bool Disabled => (OpenQuantity == 0);

        [JsonIgnore] public bool LogisticAreaError { get; set; }
        [JsonIgnore] public bool StockIDError => string.IsNullOrWhiteSpace(IdentifiedStockID);
        public bool ShowDetails { get; set; } = true;
        public List<LogisticTaskLabelDTO> Lables { get; set; }
    }
}