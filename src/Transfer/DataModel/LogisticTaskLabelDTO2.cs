using System;
using Newtonsoft.Json;
using Uni.Scan.Shared.Enums;

namespace Uni.Scan.Transfer.DataModel;

public class LogisticTaskLabelDTO2
{
    public int Id { get; set; }
    public string TaskId { get; set; }
    public int LineItemID { get; set; }
    public string Title { get; set; }
    public LabelType Type { get; set; }
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal PlanQuantity { get; set; }
    public string IdentifiedStock { get; set; }
    public string SerialStock { get; set; }
    public int Status { get; set; }
    public decimal QuatityOnLabel { get; set; }
    public string QuatityUnite { get; set; }
    private DateTime? _ProductionDate { get; set; }

    public DateTime? ProductionDate
    {
        get
        {
            if (_ProductionDate != null &&
                (_ProductionDate.Value == DateTime.MinValue || _ProductionDate.Value == DateTime.MaxValue))
            {
                return null;
            }

            return _ProductionDate;
        }

        set => _ProductionDate = value;
    }

    private DateTime? _ExpirationDate { get; set; }

    public DateTime? ExpirationDate
    {
        get
        {
            if (_ExpirationDate != null &&
                (_ExpirationDate.Value == DateTime.MinValue || _ExpirationDate.Value == DateTime.MaxValue))
            {
                return null;
            }

            return _ExpirationDate;
        }
        set => _ExpirationDate = value;
    }

    public string ExternalID { get; set; }
    public string ProductSpecification { get; set; }
    public string GTIN { get; set; }
    public string PackageID { get; set; }
    public string TransferOrdre { get; set; }
    public string ProductionOrdre { get; set; }
    public string FabricationOrdre { get; set; }
    public string SupplierIdentifiedStock { get; set; }
    public string Tare { get; set; }

    public decimal? TareDecimal
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Tare)) return null;
            if (decimal.TryParse(Tare.Replace(" " + QuatityUnite ?? "", ""), out var o)) return o;
            return null;
        }
        set
        {
            if (value != null) Tare = value.Value.ToString("N3") + " " + QuatityUnite;
            else Tare = "";
        }
    }

    public string Comment { get; set; }
    public int NbrEtiquettes { get; set; }

    public int NbrEtiquettesCalc
    {
        get
        {
            if (QuatityOnLabel != 0) return (int)(PlanQuantity / QuatityOnLabel);
            else return 0;
        }
    }

    public bool IsBatchManaged { get; set; }

    [JsonIgnore] public string UniqueId => ProductId + ">" + IdentifiedStock + ">" + QuatityOnLabel + ">" + QuatityUnite;
}