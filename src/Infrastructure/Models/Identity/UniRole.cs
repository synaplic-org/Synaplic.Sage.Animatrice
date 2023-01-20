using System;
using System.Collections.Generic;
using Uni.Scan.Domain.Contracts;
using Microsoft.AspNetCore.Identity;

namespace Uni.Scan.Infrastructure.Models.Identity
{
    public class UniRole : IdentityRole, IAuditableEntity<string>
    {
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public virtual ICollection<UniRoleClaim> RoleClaims { get; set; }

        public UniRole() : base()
        {
            RoleClaims = new HashSet<UniRoleClaim>();
        }

        public UniRole(string roleName, string roleDescription = null) : base(roleName)
        {
            RoleClaims = new HashSet<UniRoleClaim>();
            Description = roleDescription;
        }
    }
}