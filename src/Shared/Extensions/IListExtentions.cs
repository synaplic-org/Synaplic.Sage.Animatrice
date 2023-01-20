using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uni.Scan.Shared.Extensions
{
   public static class IListExtentions
    {
        public static bool IsNullOrEmpty(this IList list)
        {
            return (list?.Count ?? 0) == 0;
        }

        public static bool HasValues(this IList list)
        {
            return (list?.Count ?? 0) > 0;
        }
    }
}
