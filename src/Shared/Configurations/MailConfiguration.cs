namespace Uni.Scan.Shared.Configurations
{
    public class MailConfiguration
    {
        public string From { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public bool DevMode { get; set; }
        public string DevTo { get; set; }
    }
}