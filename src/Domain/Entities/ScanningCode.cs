using Uni.Scan.Domain.Contracts;

namespace Uni.Scan.Domain.Entities
{
    public partial class ScanningCode :AuditableEntity<int>
    {
        public string BarCodeType { get; set; }
        public string BarCodePrefix { get; set; }
        public string BarCodeSuffix { get; set; }
    }
}
