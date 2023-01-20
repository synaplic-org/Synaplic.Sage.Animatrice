using System.Linq;
using Uni.Scan.Shared.Constants.Localization;
using Uni.Scan.Shared.Settings;

namespace Uni.Scan.Server.Settings
{
    public record ServerPreference : IPreference
    {
        public string LanguageCode { get; set; } = LocalizationConstants.SupportedLanguages.FirstOrDefault()?.Code ?? LocalizationConstants.DefaultLanguageCode; 

        //TODO - add server preferences
    }
}