using System.Collections.Generic;

namespace Uni.Scan.Transfer.Requests.Label
{
    public class LabelPrintRequest
    {
        public LabelPrintRequest()
        {
            Lables = new List<Label>();
        }

        public string Responsible { get; set; }
        public string ModelName { get; set; }
        public List<Label> Lables { get; set; }
    }
}