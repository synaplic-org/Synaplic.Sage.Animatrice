using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Uni.Scan.Reports.Models
{
    public static class LogisticPrintType
    {
        public static string Task = "Task";
        public static string Label = "Label";
    }

    public class LogisticPrintRequest
    {
        public LogisticPrintRequest()
        {
            Labels = new List<LogisticPrintRequestLine>();
        }

        public List<LogisticPrintRequestLine> Labels { get; set; }
        public string Id { get; set; }
        public string ModelId { get; set; }
        public string ModelType { get; set; }
        public string TaskId { get; set; }
        public byte[] TaskIdBarcode { get; set; }
        public string TaskIdBarcodeType { get; set; }
        public string PriorityCode { get; set; }
        public string SiteId { get; set; }
        public string SiteName { get; set; }
        public string OperationType { get; set; }
        public string ProcessType { get; set; }
        public string RequestId { get; set; }
        public string ResponsibleName { get; set; }
        public string ResponsibleId { get; set; }
        public string Notes { get; set; }
    }

    public class LogisticPrintRequestLine
    {
        public int Id { get; set; }
        public string TaskId { get; set; }
        public string RequestId { get; set; }
        public string UniqueId { get; set; }
        public byte[] UniqueIdBarcode { get; set; }
        public string UniqueIdBarcodeType { get; set; }
        public string ProductId { get; set; }
        public byte[] ProductIdBarcode { get; set; }
        public string ProductIdBarcodeType { get; set; }
        public string ProductName { get; set; }
        public string PlanQuantity { get; set; }
        public string IdentifiedStock { get; set; }
        public byte[] IdentifiedStockBarcode { get; set; }
        public string IdentifiedStockBarcodeType { get; set; }
        public string SerialStock { get; set; }
        public string QuatityOnLabel { get; set; }
        public string QuatityUnite { get; set; }
        public string ProductionDate { get; set; }
        public string ExpirationDate { get; set; }
        public string TransferOrdre { get; set; }
        public string ProductionOrdre { get; set; }
        public string FabricationOrdre { get; set; }
        public string SupplierIdentifiedStock { get; set; }
        public string Tare { get; set; }
        public string Comment { get; set; }
        public int NbrEtiquettes { get; set; }
        public bool IsBatchManaged { get; set; }
    }
}