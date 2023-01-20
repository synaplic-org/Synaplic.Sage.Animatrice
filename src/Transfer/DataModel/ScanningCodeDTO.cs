namespace Uni.Scan.Transfer.DataModel
{
    public class ScanningCodeDTO
    {
        public int Id { get; set; }
        public string BarCodeType { get; set; }
        public string BarCodePrefix { get; set; }
        public string BarCodeSuffix { get; set; }
    }
}
