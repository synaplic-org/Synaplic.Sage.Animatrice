namespace Uni.Scan.Transfer.DataModel
{
    public class LogisticParametresDTO
    {
        public int Id { get; set; }
        public string ParametreID { get; set; }
        public string Owner { get; set; }
        public string ValueString { get; set; }
        public byte[] ValueBin { get; set; }
    }
}
