using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uni.Scan.Shared.Extensions
{
    public static class DecimalExtensions
    {
        public static string ToInputString(this decimal value, string format = "#.##")
        {
            return value.ToString(format).Replace(",", ".");
        }

        public static string ToValueString(this decimal value, string format = "N2", bool CleanDecimals = false)
        {
            if (value == 0) return "";
            var result = value.ToString(format).Replace(",", ".");
            return CleanDecimals ? result.Replace(".000", "") : result;
        }

        public static string ToCoefString(this decimal value, string format = "#.###")
        {
            if (value == 0) return "";

            return value.ToString(format).Replace(",", ".") + "%";
        }

        public static string ToCSString(this decimal value)
        {
            return value.ToString().Replace(",", ".");
        }
    }
}