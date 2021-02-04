using ASC.Business.Interfaces;
using ASC.DataAccess;
using ASC.Web.Configuration;
using ASC.Web.Data.Cache;
using ASC.Web.Data.Interfaces;
using ASC.Web.Extensions;
using ASC.Web.Logger;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ASC.Web
{
    public class Startup
    {

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Settings.ConnectionString = Configuration.GetSection("AppMongoSettings:ConnectionString").Value;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            // MongoDB Identity Configuration
            services.AddMongoDbIdentityConfiguration(Configuration);

            // Options pattern
            // Adicionando no container de servi�o da inje��o de depend�ncia. 
            services.Configure<ApplicationSettings>(Configuration.GetSection(ApplicationSettings.ApplicationSettingName));
            services.Configure<AppMongoSettingsOptions>(Configuration.GetSection(AppMongoSettingsOptions.ApplicationSettingName));

            // dependency injection
            services.AddDependencyInjectionServices();
            services.AddInitRoleUserSetup();

            //services.AddDistributedMemoryCache();
            services.AddRedis(Configuration);

            // Adicionando Menu dinamico na View Razor - EmbeddedFileProvider�used to access files inside the assemblies
            services.AddEmbeddedFileProvider();

            services.AddSession();

            services.AddMvcOptions();

            services.AddAutoMapper(typeof(Startup));

            // Configurar a autentica��o do Google
            services.AddGoogleAuthentication(Configuration);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IWebHostEnvironment env,
            IMasterDataCacheOperations masterDataCacheOperations,
            ILoggerFactory loggerFactory,
            ILogDataOperations logDataOperations,
            INavigationCacheOperations navigationCacheOperations)
        {

            // Configure MongoDb Logger to log all events except the ones that are generated by
            // default by ASP.NET Core.
            loggerFactory.AddAzureTableStorageMongoDbLog(logDataOperations,
                (categoryName, logLevel) => !categoryName.Contains("Microsoft") &&
                logLevel >= LogLevel.Information);

            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                // app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePagesWithRedirects("/Home/Error/{0}");

            app.UseHttpsRedirection();
            app.UseSession();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            // Redis Cache
            masterDataCacheOperations.CreateMasterDataCacheAsync();

            // Menu to Cache
            navigationCacheOperations.CreateNavigationCacheAsync();
        }

        ////This method creates the standard roles and master user
        //public async Task InitRoleUserSetup(IServiceCollection services)
        //{

        //    IServiceProvider serviceProvider = services.BuildServiceProvider();

        //    var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
        //    var roleManager = serviceProvider.GetService<RoleManager<ApplicationRole>>();
        //    var options = serviceProvider.GetService<IOptions<ApplicationSettings>>();
        //    var identitySeed = serviceProvider.GetService<IIdentitySeed>();

        //    await identitySeed.Seed(userManager, roleManager, options);           

        //}
    }
}
