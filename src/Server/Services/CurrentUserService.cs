using Uni.Scan.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Uni.Scan.Infrastructure.Models.Identity;

namespace Uni.Scan.Server.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            Name = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name) + " " +
                   httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Surname);
            SiteId = httpContextAccessor.HttpContext?.User?.FindFirstValue(nameof(UniUser.SiteID)) ?? "*";
            EmployeeID = httpContextAccessor.HttpContext?.User?.FindFirstValue(nameof(UniUser.EmployeeID)) ?? "*";
            Claims = httpContextAccessor.HttpContext?.User?.Claims.AsEnumerable()
                .Select(item => new KeyValuePair<string, string>(item.Type, item.Value)).ToList();
        }

        public string UserId { get; }
        public string SiteId { get; }
        public string Name { get; }
        public string EmployeeID { get; }
        public List<KeyValuePair<string, string>> Claims { get; set; }
    }
}