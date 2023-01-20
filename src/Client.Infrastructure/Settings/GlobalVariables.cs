using MudBlazor;
using System;
using System.Globalization;

namespace Uni.Scan.Client.Infrastructure.Settings
{
    public class GlobalVariables
    {
        public string IndexDbName = "UniScan";
        public String CurrentTitle { get; set; }
        public bool IsMobileView { get; set; }
        public bool IsLoading { get; set; }
        public bool AllowInventoryAddItem { get; set; } = true;
        public string QteFormat { get; set; } = "N3";
        public Variant TestFiledVariant { get; set; } = Variant.Text;

        public string CurrentVersion = "v1.2.5";
        private CultureInfo _customCulture;

        public CultureInfo CustomCulture
        {
            get
            {
                if (_customCulture == null)
                {
                    _customCulture = (CultureInfo)CultureInfo.GetCultureInfo("en-US").Clone();
                    _customCulture.NumberFormat.NumberGroupSeparator = " ";
                }

                return _customCulture;
            }
        }

        public bool ShowScanButton { get; set; }
    }
}