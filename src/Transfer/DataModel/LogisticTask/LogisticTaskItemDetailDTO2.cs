using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Uni.Scan.Transfer.DataModel.LogisticTask
{
    public class LogisticTaskItemDetailDTO2
    {
        public LogisticTaskItemDetailDTO2()
        {
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

        private string _IdentifiedStockIDNew = "<>";

        public string IdentifiedStockIDNew
        {
            get => (_IdentifiedStockIDNew == "<>") ? IdentifiedStockID : _IdentifiedStockIDNew;
            set
            {
                if (value.Contains("/"))
                {
                    _IdentifiedStockIDNew = value[..value.IndexOf("/", StringComparison.OrdinalIgnoreCase)];
                }
                else if (value.Contains(">"))
                {
                    var ar = value.Split(">");
                    _IdentifiedStockIDNew = ar.Length >= 2 ? ar[1] : value;
                }
                else
                {
                    _IdentifiedStockIDNew = value;
                }

                _IdentifiedStockIDNew = IdentifiedStockIDNew.Trim();
            }
        }

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

        public bool IsNew => string.IsNullOrWhiteSpace(OutputUUID);

        [JsonIgnore] public bool Disabled => (OpenQuantity == 0) && !IsNew;

        [JsonIgnore] public bool LogisticSourceAreaError { get; set; }
        [JsonIgnore] public bool LogisticTargetAreaError { get; set; }
        [JsonIgnore] public bool StockIDError { get; set; }
        [JsonIgnore] public bool ConfirmQuantityError { get; set; }

        public LogisticTaskItemDetailDTO2 CreateNew()
        {
            var dt = new LogisticTaskItemDetailDTO2()
            {
                //UUID = this.UUID,
                TaskId = this.TaskId,
                LineItemID = this.LineItemID,
                ProductID = this.ProductID,
                SourceLogisticsAreaID = this.SourceLogisticsAreaID,
                TargetLogisticsAreaID = this.TargetLogisticsAreaID,
                ProductDescription = this.ProductDescription,
                PlanQuantity = this.PlanQuantity,
                PlanQuantityUnitCode = this.PlanQuantityUnitCode,
                //OpenQuantity = this.OpenQuantity,
                OpenQuantityUnitCode = this.OpenQuantityUnitCode,
                //ConfirmQuantity = this.ConfirmQuantity,
                ConfirmQuantityUnitCode = this.ConfirmQuantityUnitCode,
                TotalConfirmedQuantity = this.TotalConfirmedQuantity,
                TotalConfirmedQuantityUnitCode = this.TotalConfirmedQuantityUnitCode,
                IdentifiedStockID = this.IdentifiedStockID,
                PackageLogisticUnitUUID = this.PackageLogisticUnitUUID,
                PackageLogisticUnitTotalConfirmedQuantity = this.PackageLogisticUnitTotalConfirmedQuantity,
                PackageLogisticUnitOpenQuantity = this.PackageLogisticUnitOpenQuantity,
                PackageLogisticUnitTotalConfirmedQuantityUnitCode =
                    this.PackageLogisticUnitTotalConfirmedQuantityUnitCode,
                PackageLogisticUnitPlanQuantityUnitCode = this.PackageLogisticUnitPlanQuantityUnitCode,
                //MaterialDeviationStatusCode = this.MaterialDeviationStatusCode,
                MaterialInspectionID = this.MaterialInspectionID,
                PackageLogisticUnitPlanQuantity = this.PackageLogisticUnitPlanQuantity,
                PackageLogisticUnitOpenQuantityUnitCode = this.PackageLogisticUnitOpenQuantityUnitCode,
                //MaterialDeviationStatusCodeSpecified = this.MaterialDeviationStatusCodeSpecified,
                LogisticsDeviationReasonCode = this.LogisticsDeviationReasonCode,
                LineItemIDSpecified = this.LineItemIDSpecified,
                ProductSpecificationID = this.ProductSpecificationID,
                RestrictedIndicator = this.RestrictedIndicator,
                ProductRequirementSpecificationDescription = this.ProductRequirementSpecificationDescription,
                SerialNumberAssignments = this.SerialNumberAssignments
            };
            return dt;
        }
    }
}