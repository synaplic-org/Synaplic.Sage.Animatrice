using System.Collections.Generic;

namespace Uni.Scan.Transfer.Responses.Identity
{
    public class GetAllUsersResponse
    {
        public IEnumerable<UserResponse> Users { get; set; }
    }
}