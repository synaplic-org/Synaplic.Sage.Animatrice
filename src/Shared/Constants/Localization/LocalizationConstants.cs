namespace Uni.Scan.Shared.Constants.Localization
{
    public static class LocalizationConstants
    {
        public static readonly string DefaultLanguageCode = "fr-FR";
        public static readonly LanguageCode[] SupportedLanguages = {

             new LanguageCode
            {
                Code = "fr-FR",
                DisplayName = "Français"
            },
            new LanguageCode
            {
                Code = "en-US",
                DisplayName= "English"
            }
            
           
        };
    }
}
