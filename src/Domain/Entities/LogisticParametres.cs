using System.Reflection;
using Uni.Scan.Domain.Contracts;

namespace Uni.Scan.Domain.Entities
{
    public class LogisticParametres : AuditableEntity<int>
    {
        public string ParametreID { get; set; }
        public string Owner { get; set; }
        public string ValueString { get; set; }
        public byte[] ValueBin { get; set; }
    }
}
