using System.Collections.Generic;

namespace Uni.Scan.Transfer.Responses.Identity
{
    public class GetAllRolesResponse
    {
        public IEnumerable<RoleResponse> Roles { get; set; }
    }
}