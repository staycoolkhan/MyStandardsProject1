using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace MyStandardsProject1.Api.Models.Users
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
        public virtual ICollection<ApplicationRoleClaim> RoleClaims { get; set; }
    }
}
