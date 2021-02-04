using ASC.Utilities.Model;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Linq;

namespace ASC.Utilities
{
    public static class ClaimsPrincipalExtensions
    {
        public static CurrentUser GetCurrentUserDetails(this ClaimsPrincipal principal)
        {
            if (principal.Claims.Any() == false)
            {
                return null;
            }

            return new CurrentUser()
            {
                Name = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                Email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                Roles = principal.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray(),
                IsActive = Convert.ToBoolean(principal.Claims.SingleOrDefault(c => c.Type.Equals("IsActive"))?.Value)
            };
        }
    }
}
