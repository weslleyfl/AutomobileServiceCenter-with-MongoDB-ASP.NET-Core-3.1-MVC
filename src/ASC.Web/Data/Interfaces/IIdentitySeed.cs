using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASC.Web.Models;
using Microsoft.Extensions.Options;
using ASC.Web.Configuration;


namespace ASC.Web.Data.Interfaces
{
    public interface IIdentitySeed
    {
        Task Seed(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
           IOptions<ApplicationSettings> options);
        
    }
}
