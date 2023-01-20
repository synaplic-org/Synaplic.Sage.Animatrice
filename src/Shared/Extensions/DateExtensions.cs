using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uni.Scan.Shared.Extensions
{
    public static class DateExtensions
    {
        public static string ToValueString(this DateTime value, string format = "dd/MM/yy")
        {
            if (value == DateTime.MinValue) return "";
            if (value == DateTime.MaxValue) return "";

            return value.ToString(format);
        }

        public static string ToValueString(this DateTime? value, string format = "dd/MM/yy")
        {
            if (value == null) return "";

            return ToValueString(value.Value, format);
        }
    }
}