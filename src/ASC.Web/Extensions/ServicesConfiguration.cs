using ASC.Business;
using ASC.Business.Interfaces;
using ASC.DataAccess;
using ASC.DataAccess.Interfaces;
using ASC.DataAccess.Repository;
using ASC.Web.Configuration;
using ASC.Web.Data;
using ASC.Web.Data.Cache;
using ASC.Web.Data.Interfaces;
using ASC.Web.Models;
using ASC.Web.Services.Email;
using ASC.Web.Services.SMS;
using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using System;
using StackExchange.Redis;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASC.Web.Filters;
using ASC.Web.Data.Menu;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;

namespace ASC.Web.Extensions
{
    public static class ServicesConfiguration
    {
        /// <summary>
        /// // Resolving dependency in Startup class
        /// </summary>
        /// <param name="services"></param>
        public static void AddDependencyInjectionServices(this IServiceCollection services)
        {
            services.AddSingleton<IIdentitySeed, IdentitySeed>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // email and sms
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthSmsSender>();

            // services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IMongoContext, MongoContext>();

            // Repository
            services.AddScoped<IMasterDataKeyRepository, MasterDataKeyRepository>();
            services.AddScoped<IMasterDataValueRepository, MasterDataValueRepository>();
            services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();

            // Business
            services.AddScoped<IMasterDataOperations, MasterDataOperations>();
            services.AddScoped<IServiceRequestOperations, ServiceRequestOperations>();

            // Redis Cache
            services.AddScoped<IMasterDataCacheOperations, MasterDataCacheOperations>();

            // Log and Exception
            services.AddScoped<ILogDataOperations, LogDataOperations>();

            // Filter
            services.AddScoped<UserActivityFilter>();
            services.AddScoped<CustomExceptionFilter>();

            // Menu
            services.AddSingleton<INavigationCacheOperations, NavigationCacheOperations>();
        }

        public static void AddInitRoleUserSetup(this IServiceCollection services)
        {
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetService<RoleManager<ApplicationRole>>();
            var options = serviceProvider.GetService<IOptions<ApplicationSettings>>();
            var identitySeed = serviceProvider.GetService<IIdentitySeed>();

            identitySeed.Seed(userManager, roleManager, options);
        }

        public static void AddMongoDbIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // Este lambda determina se o consentimento do usuário para cookies não essenciais é necessário para uma determinada solicitação (request).
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;

            });

            //services.ConfigureApplicationCookie(options =>
            //{
            //    options.Events.OnRedirectToLogin = context =>
            //    {
            //        context.Response.StatusCode = 401;

            //        return Task.CompletedTask;
            //    };
            //});

            services.Configure<PasswordHasherOptions>(options =>
                 options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV2);

            var mongoDbIdentityConfiguration = new MongoDbIdentityConfiguration()
            {
                MongoDbSettings = new MongoDbSettings
                {
                    ConnectionString = configuration.GetSection("AppMongoSettings:ConnectionString").Value,
                    DatabaseName = configuration.GetSection("AppMongoSettings:Database").Value
                },
                IdentityOptionsAction = options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 4;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;

                    // Lockout settings
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.AllowedForNewUsers = true;


                    // ApplicationUser settings - Correçao para validation failed: InvalidUserName
                    options.User.RequireUniqueEmail = true;
                    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/ ã~";

                    options.SignIn.RequireConfirmedAccount = true;

                    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultProvider;
                    options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;

                }
            };

            services.ConfigureMongoDbIdentity<ApplicationUser, ApplicationRole, Guid>(mongoDbIdentityConfiguration);

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings - Change email and activity timeout - The default inactivity timeout is 14 days. 
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                options.SlidingExpiration = true;

                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";

            });

            // O código a seguir altera todos os tokens de proteção de dados período de tempo limite para 3 horas
            // What we want for our password reset token is to be valid for a limited time,
            services.Configure<DataProtectionTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromMinutes(10));

        }

        public static void AddGoogleAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    IConfigurationSection googleAuthNSection =
                        configuration.GetSection("Google:Identity");

                    options.ClientId = googleAuthNSection["ClientId"];
                    options.ClientSecret = googleAuthNSection["ClientSecret"];
                });
        }

        public static void AddMvcOptions(this IServiceCollection services)
        {
            services.AddControllersWithViews(options =>
            {
                // Aplicando a proteção contra o CSRF de forma global no ASP.NET Core
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                // Filter error global
                options.Filters.Add(typeof(CustomExceptionFilter));
            })
            .AddJsonOptions(opt =>
            {
                var serializerOptions = opt.JsonSerializerOptions;
                serializerOptions.IgnoreNullValues = true;
                serializerOptions.IgnoreReadOnlyProperties = false;
                serializerOptions.WriteIndented = true;
            })
            .AddNewtonsoftJson(options =>
            {
                // By default, ASP.NET Core MVC serializes JSON data by using camelCaseing casing. 
                // To prevent that, we need to add the following configuration to MVC
                // Use the default property (Pascal) casing               
                //options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                // Use the default property (Pascal) casing
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            services.AddRazorPages()
                .AddRazorRuntimeCompilation();
            //services.AddRazorPages(options =>
            // {
            //     options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
            //     options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
            // });
        }

        public static void AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetSection("CacheSettings:CacheConnectionString").Value;

                options.InstanceName = Assembly.GetExecutingAssembly().GetName().Name;
                //options.InstanceName = Configuration.GetSection("CacheSettings:CacheInstance").Value;
            });
        }


        public static void AddEmbeddedFileProvider(this IServiceCollection services)
        {
            //IFileProvider embeddedProvider = new EmbeddedFileProvider(Assembly.GetEntryAssembly());
            //services.AddSingleton<IFileProvider>(embeddedProvider);

            // Add support to embedded views from ASC.Utilities project.
            var assembly = typeof(ASC.Utilities.Navigation.LeftNavigationViewComponent)
                .GetTypeInfo().Assembly;

            // EmbeddedFileProvider used to access files inside the assemblies
            var embeddedFileProvider = new EmbeddedFileProvider(assembly, "ASC.Utilities");

            services.Configure<MvcRazorRuntimeCompilationOptions>(options =>
            {
                options.FileProviders.Add(embeddedFileProvider);
            });
        }

    }
}
