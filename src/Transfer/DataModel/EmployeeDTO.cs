using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uni.Scan.Transfer.DataModel
{
    public class EmployeeDTO
    {    
        public string EmployeeID { get; set; }
        public string ObjectID { get; set; }
        public string InternalID { get; set; }
        public Guid UUID { get; set; }
        public string BusinessPartnerFormattedName { get; set; }
    }
}
