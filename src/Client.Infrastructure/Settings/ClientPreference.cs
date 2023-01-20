using System.Collections.Generic;
using System.Linq;
using Uni.Scan.Shared.Constants.Localization;
using Uni.Scan.Shared.Settings;

namespace Uni.Scan.Client.Infrastructure.Settings
{
    public record ClientPreference : IPreference
    {
        public bool IsDarkMode { get; set; }
        public bool IsRtl => LanguageCode.StartsWith("ar-");
        public bool IsDrawerOpen { get; set; }
        public string PrimaryColor { get; set; }
        public string LanguageCode { get; set; } = LocalizationConstants.SupportedLanguages.FirstOrDefault()?.Code ??  LocalizationConstants.DefaultLanguageCode;
        public List<string> MyTasklist { get; set; }

    }
}