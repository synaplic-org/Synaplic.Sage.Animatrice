using System.Collections.Generic;
using Uni.Scan.Application.Interfaces.Common;

namespace Uni.Scan.Application.Interfaces.Services
{
    public interface ICurrentUserService : IService
    {
        string UserId { get; }
        public string SiteId { get; }

        public string Name { get; }

        public string EmployeeID { get; }

        public List<KeyValuePair<string, string>> Claims { get; set; }
    }
}