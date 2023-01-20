using System.ComponentModel.DataAnnotations;

namespace Uni.Scan.Transfer.Requests.Logistics
{
    public class ProductSearchRequest
    {
        [Required]
        public string SiteID { get; set; }
        public string Product { get; set; }
        public string LogisticArea { get; set; }
        public string IdentifiedStock { get; set; }
    }
}
