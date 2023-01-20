using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Uni.Scan.Reports.Models
{
    public class ZoneLogistic
    {
        public string zoneLogistic1 { get; set; }
        public string zoneLogistic2 { get; set; }
        public string zoneLogistic3 { get; set; } 
        public byte[] zoneBarcode1 { get; set; }
        public byte[] zoneBarcode2 { get; set; }
        public byte[] zoneBarcode3 { get; set; }
    }

    public class ZoneLogisticObject
    {
        public List<string> zoneLogistic { get; set; }
        public string ModelName { get; set; }
    }
}