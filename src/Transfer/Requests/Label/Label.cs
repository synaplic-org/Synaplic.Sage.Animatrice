namespace Uni.Scan.Transfer.Requests.Label
{
    public class Label
    {
        public string Title { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string IdentifiedStock { get; set; }
        public string SerialStock { get; set; }
        public decimal QuatityOnLabel { get; set; }
        public string QuatityUnite { get; set; }
        public string ProductionDate { get; set; }
        public string ExpirationDate { get; set; }
        public string ExternalID { get; set; }
        public string ProductSpecification { get; set; }
        public string GTIN { get; set; }
        public string PackageID { get; set; }
        public string TransferOrdre { get; set; }
        public string ProductionOrdre { get; set; }
        public string FabricationOrdre { get; set; }
        public string SupplierIdentifiedStock { get; set; }
        public string Tare { get; set; }
        public string Comment { get; set; }
        public int NbrEtiquettes { get; set; }
        public int Duplicata { get; set; }
        public int Supplement { get; set; }

    }
}