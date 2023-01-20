namespace Uni.Scan.Transfer.Requests.Task
{
    public class TaskLine
    {
        public int LineItemID { get; set; }
        public string ProductID { get; set; }
        public string ProductDescription { get; set; }
        public decimal PlanQuantity { get; set; }
        public string PlanQuantityUnitCode { get; set; }
        public string IdentifiedStockID { get; set; }

    }
}