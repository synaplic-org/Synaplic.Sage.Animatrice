using System;
using Uni.Scan.Shared.Enums;

namespace Uni.Scan.Transfer.DataModel
{
    public class LabelTemplateDTO
    {
        public int Id { get; set; }
        public string ModelName { get; set; }
        public string ModelID { get; set; }
        public PrintType Type { get; set; }  
       

    }
}
