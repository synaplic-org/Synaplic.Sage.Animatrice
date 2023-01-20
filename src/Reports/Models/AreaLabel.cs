using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Uni.Scan.Reports.Models
{
    public class AreaLabel
    {
        public string LogisticAreaID { get; set; }
        public byte[] LogisticAreaIDBarcode { get; set; }
        public string ModelName { get; set; }

    }
}