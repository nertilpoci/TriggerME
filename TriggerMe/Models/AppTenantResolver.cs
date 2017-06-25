using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TriggerMe.Model;

namespace TriggerMe.Models
{
    public class AppTenantResolver : ITenantResolver<AppTenant>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public AppTenantResolver(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<TenantContext<AppTenant>> ResolveAsync(HttpContext context)
        {
            TenantContext<AppTenant> tenantContext = null;

            var user = await _userManager.GetUserAsync(context.User);

            if (null!=user)
            {
                tenantContext = new TenantContext<AppTenant>(new AppTenant { Id=user.Id});
            }

            return tenantContext;
        }
    }
}
