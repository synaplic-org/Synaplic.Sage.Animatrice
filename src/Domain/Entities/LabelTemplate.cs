using Uni.Scan.Domain.Contracts;
using Uni.Scan.Shared.Enums;

namespace Uni.Scan.Domain.Entities
{
    public class LabelTemplate : AuditableEntity<int>
    {
        public string ModelName { get; set; }
        public string ModelID { get; set; }
        public PrintType Type { get; set; }

    }
}
