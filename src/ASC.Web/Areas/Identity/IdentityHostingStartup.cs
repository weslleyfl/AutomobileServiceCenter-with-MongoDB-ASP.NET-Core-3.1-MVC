using System;
using ASC.Web.Data;
using ASC.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(ASC.Web.Areas.Identity.IdentityHostingStartup))]
namespace ASC.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            //builder.ConfigureServices((context, services) =>
            //{

            //    services.AddIdentity<ApplicationUser, ApplicationRole>()
            //       .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>
            //       (

            //        connectionString: context.Configuration.GetSection("AppMongoSettings:ConnectionString").Value,
            //        databaseName: context.Configuration.GetSection("AppMongoSettings:Database").Value

            //       )
            //       .AddDefaultTokenProviders();
            //});
                       
            //builder.ConfigureServices((context, services) =>
            //{
            //    services.AddDbContext<ASCWebContext>(options =>
            //        options.UseSqlServer(
            //            context.Configuration.GetConnectionString("ASCWebContextConnection")));

            //    services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //        .AddEntityFrameworkStores<ASCWebContext>();
            //});
        }
    }
}