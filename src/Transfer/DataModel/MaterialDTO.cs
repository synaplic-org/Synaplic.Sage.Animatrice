using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Uni.Scan.Transfer.DataModel
{
    public class MaterialDTO
    {
        public string ObjectID { get; set; }
        public string ProductID { get; set; }
        public string Description_KUT { get; set; }
        public string ProductValuationLevelTypeCode { get; set; }
        public string ProductValuationLevelTypeCodeText { get; set; }
        public string SerialIdentifierAssignmentProfileCode { get; set; }
        public string SerialIdentifierAssignmentProfileCodeText { get; set; }
        public string InternalID { get; set; }
        public string ProductIdentifierTypeCode { get; set; }
        public string ProductIdentifierTypeCodeText { get; set; }
        public string ProductTypeCode { get; set; }
        public string ProductTypeCodeText { get; set; }
        public string BaseMeasureUnitCode { get; set; }


        [JsonIgnore]
        public bool IsBatchManaged => ProductIdentifierTypeCode == "01"
                                      || ProductIdentifierTypeCode == "02"
                                      || ProductIdentifierTypeCode == "04";

        [JsonIgnore] public bool IsSerialManaged => SerialIdentifierAssignmentProfileCode == "1003";
    }
}