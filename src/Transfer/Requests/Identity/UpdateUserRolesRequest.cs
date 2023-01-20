using System.Collections.Generic;
using Uni.Scan.Transfer.Responses.Identity;

namespace Uni.Scan.Transfer.Requests.Identity
{
    public class UpdateUserRolesRequest
    {
        public string UserId { get; set; }
        public IList<UserRoleModel> UserRoles { get; set; }
    }
}