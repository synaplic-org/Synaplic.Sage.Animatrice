using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;

namespace Uni.Scan.Infrastructure.ByDesign.Requests.PhysicalInventory
{
    public class UpdateQuantityRequest
    {
        [JsonProperty("ObjectID", NullValueHandling = NullValueHandling.Ignore)]
        public string ObjectID { get; set; }

        [JsonProperty("CountedQuantity", NullValueHandling = NullValueHandling.Ignore)]
        public string CountedQuantity { get; set; }

        [JsonProperty("ZeroCountedQuantityConfirmedIndicator", NullValueHandling = NullValueHandling.Ignore)]
        public bool ZeroCountedQuantityConfirmedIndicator { get; set; }

        public string ToJson() => JsonConvert.SerializeObject(this);

        //public string ToJson() => JsonConvert.SerializeObject(this, JsonSettings);


        //private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        //{
        //    MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        //    DateParseHandling = DateParseHandling.None,
        //    Converters =
        //    {
        //        new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.None }
        //    },
        //};
    }
}