using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uni.Scan.Transfer.DataModel
{
    public class LogisticTaskDTO
    {
        public LogisticTaskDTO()
        {
                Labels = new ();
                Martials = new ();
                LogisticTaskDetails = new ();
                LogisticAreas = new ();
        }
        public string Id { get; set; }
        public string OperationType { get; set; }
        public string OperationTypeCode { get; set; }
        public string Status { get; set; }
        public string ResponsibleId { get; set; }
        public string ResponsibleName { get; set; }
        public string TaskFolderId { get; set; }
        public string Priority { get; set; }
        public string RequestId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string SiteId { get; set; }
        public string Notes { get; set; }
        public string ThirdPartyKey { get; set; }
        public string ThirdpartyName { get; set; }
        public string BusinessTransactionDocumentReferenceID { get; set; }
        public string TaskUuid { get; set; }
        public string ObjectID { get; set; }
        public string SiteName { get; set; }
        public string ProcessTypeCode { get; set; }
        public string ProcessType { get; set; }
        public int ItemsNumberValue { get; set; }
        public List<LogisticTaskDetailDTO> LogisticTaskDetails { get; set; }
        public List<MaterialDTO> Martials { get; set; }
        public List<LogisticAreaDTO> LogisticAreas { get; set; }
        public List<LogisticTaskLabelDTO> Labels { get; set; }
    }
}
