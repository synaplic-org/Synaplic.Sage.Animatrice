using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Uni.Scan.Shared.Extensions
{
    public static class StringExtentions
    {
        public static string ToDateString(this string dateString, string format = "o", string culture = "fr-FR")
        {
            var dateTime = DateTime.Parse(dateString, new System.Globalization.CultureInfo(culture),
                System.Globalization.DateTimeStyles.NoCurrentDateDefault);
            return dateTime.ToString(format);
        }

        public static DateTimeOffset ToDateTimeOffset(this string dateString, string culture = "fr-FR")
        {
            var dateTime = DateTime.Parse(dateString, new System.Globalization.CultureInfo(culture),
                System.Globalization.DateTimeStyles.NoCurrentDateDefault);
            return dateTime;
        }

        public static DateTime ToDateTime(this string dateString, string culture = "fr-FR")
        {
            var dateTime = DateTime.Parse(dateString, new System.Globalization.CultureInfo(culture),
                System.Globalization.DateTimeStyles.NoCurrentDateDefault);
            return dateTime;
        }

        public static string ToDecimalString(this string dateString, string format = "N2")
        {
            decimal x = 0;
            Decimal.TryParse(dateString, out x);
            return x.ToValueString(format);
        }

        public static decimal ToDecimal(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return 0;

            if (decimal.TryParse(str, out decimal result)) return result;

            str = str.Replace(",", ".");

            return decimal.Parse(str, CultureInfo.InvariantCulture);
        }

        public static long ToLong(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return 0;

            return long.Parse(str);
        }
    }
}