namespace Uni.Scan.Shared.Configurations
{
    public class AppConfiguration
    {
        public string Secret { get; set; }
        public bool StartCronOnStartup { get; set; }
        public string ReportUrl { get; set; }
    }
}