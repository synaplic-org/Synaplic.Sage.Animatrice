using System.Collections.Generic;

namespace Uni.Scan.Transfer.Requests.Task
{
    public class TaskPrintRequest
    {
        public TaskPrintRequest()
        {
            TaskLineDetails = new List<TaskLine>();
        }
        public string Id { get; set; }
        public string ModelName { get; set; }
        public string ResponsibleName { get; set; }
        public string Priority { get; set; }
        public string SiteId { get; set; }
        public string SiteName { get; set; }
        public string OperationType { get; set; }
        public string ProcessType { get; set; }
        public string RequestId { get; set; }
        public int ItemsNumberValue { get; set; }
        public List<TaskLine> TaskLineDetails { get; set; }

    }
}

