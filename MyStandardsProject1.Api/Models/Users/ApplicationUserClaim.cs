using Microsoft.AspNetCore.Identity;
using System;

namespace MyStandardsProject1.Api.Models.Users
{
    public class ApplicationUserClaim : IdentityUserClaim<Guid>
    {
        public virtual ApplicationUser User { get; set; }
    }
}
