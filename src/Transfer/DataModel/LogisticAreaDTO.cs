namespace Uni.Scan.Transfer.DataModel
{
    public class LogisticAreaDTO
    {
        public int Id { get; set; }
        public string ObjectID { get; set; }
        public string LogisticAreaUUID { get; set; }
        public string LogisticAreaID { get; set; }
        public string InventoryManagedLocationID { get; set; }
        public string InventoryManagedLocationUUID { get; set; }
        public string SiteID { get; set; }
        public string SiteUUID { get; set; }
        public string TypeCode { get; set; }
        public string TypeCodeText { get; set; }
        public bool InventoryManagedLocationIndicator { get; set; }
        public string SiteName { get; set; }
        public string LifeCycleStatusCode { get; set; }
        public string LifeCycleStatusCodeText { get; set; }
        public string ParentLocationUUID { get; set; }

    }
}
