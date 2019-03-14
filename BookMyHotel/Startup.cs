using BookMyHotel.Tenants.Common.Interfaces;
using BookMyHotel.ViewModels;
using BookMyHotel_Tenants.Common.Interfaces;
using BookMyHotel_Tenants.Common.Repositories;
using BookMyHotel_Tenants.Common.Utilities;
using BookMyHotel_Tenants.EmailService;
using BookMyHotel_Tenants.UserApp.EF.CatalogDB;
using DnsClient;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace BookMyHotel
{
    public class Startup
    {
        private IUtilities _utilities;
        private ICatalogRepository _catalogRepository;
        private ITenantRepository _tenantRepository;
        private ILookupClient _client;

        #region Public Properties
        public static DatabaseConfig DatabaseConfig { get; set; }
        public static CatalogConfig CatalogConfig { get; set; }
        public static TenantServerConfig TenantServerConfig { get; set; }
        public IConfigurationRoot Configuration { get; }

        #endregion

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                 .SetBasePath(env.ContentRootPath)
                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                 .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                 .AddEnvironmentVariables();
            Configuration = builder.Build();

            //read config settigs from appsettings.json
            ReadAppConfig();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddDataAnnotationsLocalization();

            services.AddMvcCore()
                .AddDataAnnotations();

            // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();
            services.AddSession();

            //register catalog DB
            services.AddDbContext<CatalogDbContext>(options => options.UseSqlServer(GetCatalogConnectionString(CatalogConfig, DatabaseConfig)));

            //Add Application services
            services.AddTransient<ICatalogRepository, CatalogRepository>();
            services.AddTransient<ITenantRepository, TenantRepository>();
            services.AddSingleton<ITenantRepository>(p => new TenantRepository(GetBasicSqlConnectionString()));
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<ILookupClient>(p => new LookupClient());
            services.AddSingleton<IEmailService>(p => new EmailService());
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        options.LoginPath = "/Account/Login";
                        options.LogoutPath = "Account/Logout";
                    });

            //create instance of utilities class
            services.AddTransient<IUtilities, Utilities>();
            var provider = services.BuildServiceProvider();
            _utilities = provider.GetService<IUtilities>();
            _catalogRepository = provider.GetService<ICatalogRepository>();
            _tenantRepository = provider.GetService<ITenantRepository>();
            _client = provider.GetService<ILookupClient>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            #region Localisation settings

            //get the list of supported cultures from the appsettings.json
            var allSupportedCultures = Configuration.GetSection("SupportedCultures").Get<SupportedCultures>();
            var defaultCulture = Configuration["DefaultRequestCulture"];

            if (allSupportedCultures != null && defaultCulture != null)
            {
                List<CultureInfo> supportedCultures = allSupportedCultures.SupportedCulture.Select(t => new CultureInfo(t)).ToList();

                app.UseRequestLocalization(new RequestLocalizationOptions
                {
                    DefaultRequestCulture = new RequestCulture(defaultCulture),
                    //get the default culture from appsettings.json
                    SupportedCultures = supportedCultures, // UI strings that we have localized.
                    SupportedUICultures = supportedCultures,
                    RequestCultureProviders = new List<IRequestCultureProvider>()
                });
            }
            else
            {
                app.UseRequestLocalization(new RequestLocalizationOptions
                {
                    DefaultRequestCulture = new RequestCulture("en-US"),
                    RequestCultureProviders = new List<IRequestCultureProvider>()
                });
            }

            #endregion

            app.UseSession();
            app.UseCookiePolicy();           

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                   name: "default",
                   template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default_route",
                    template: "{tenant}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "TenantAccount",
                    template: "{tenant}/{controller=Account}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "FindSeats",
                    template: "{tenant}/{controller=FindHotels}/{action=Index}/{id?}");
               
            });

            //shard management
            InitialiseShardMapManager();
            _utilities.RegisterTenantShard(TenantServerConfig, DatabaseConfig, CatalogConfig, TenantServerConfig.ResetBookingDates);
        }

        /// <summary>
        ///  Gets the catalog connection string using the app settings
        /// </summary>
        /// <param name="catalogConfig">The catalog configuration.</param>
        /// <param name="databaseConfig">The database configuration.</param>
        /// <returns></returns>
        private string GetCatalogConnectionString(CatalogConfig catalogConfig, DatabaseConfig databaseConfig)
        {
            return
                $"Server=tcp:{catalogConfig.CatalogServer},1433;Database={catalogConfig.CatalogDatabase};User ID={databaseConfig.DatabaseUser};Password={databaseConfig.DatabasePassword};Trusted_Connection=False;Encrypt=True;";
        }

        /// <summary>
        /// Reads the application settings from appsettings.json
        /// </summary>
        private void ReadAppConfig()
        {
            DatabaseConfig = new DatabaseConfig
            {
                DatabasePassword = Configuration["DatabasePassword"],
                DatabaseUser = Configuration["DatabaseUser"],
                DatabaseServerPort = Convert.ToInt32(Configuration["DatabaseServerPort"]),
                SqlProtocol = SqlProtocol.Tcp,
                ConnectionTimeOut = Convert.ToInt32(Configuration["ConnectionTimeOut"]),
                LearnHowFooterUrl = Configuration["LearnHowFooterUrl"]
            };

            CatalogConfig = new CatalogConfig
            {
                ServicePlan = Configuration["ServicePlan"],
                CatalogDatabase = Configuration["CatalogDatabase"],
                CatalogServer = Configuration["CatalogServer"] + ".database.windows.net",
                CatalogLocation = Configuration["APP_REGION"]
            };

            TenantServerConfig = new TenantServerConfig
            {
                TenantServer = Configuration["TenantServer"] + ".database.windows.net"
            };

            bool isResetBookingDatesEnabled = false;
            if (bool.TryParse(Configuration["ResetBookingDates"], out isResetBookingDatesEnabled))
            {
                TenantServerConfig.ResetBookingDates = isResetBookingDatesEnabled;
            }
        }

        /// <summary>
        /// Initialises the shard map manager and shard map 
        /// <para>Also does all tasks related to sharding</para>
        /// </summary>
        private void InitialiseShardMapManager()
        {
            var basicConnectionString = GetBasicSqlConnectionString();
            SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder(basicConnectionString)
            {
                DataSource = DatabaseConfig.SqlProtocol + ":" + CatalogConfig.CatalogServer + "," + DatabaseConfig.DatabaseServerPort,
                InitialCatalog = CatalogConfig.CatalogDatabase
            };

            var sharding = new Sharding(CatalogConfig.CatalogDatabase, connectionString.ConnectionString, _catalogRepository, _tenantRepository, _utilities);
        }

        /// <summary>
        /// Gets the basic SQL connection string.
        /// </summary>
        /// <returns></returns>
        private string GetBasicSqlConnectionString()
        {
            var connStrBldr = new SqlConnectionStringBuilder
            {
                UserID = DatabaseConfig.DatabaseUser,
                Password = DatabaseConfig.DatabasePassword,
                ApplicationName = "EntityFramework",
                ConnectTimeout = DatabaseConfig.ConnectionTimeOut,
                LoadBalanceTimeout = 15
            };

            return connStrBldr.ConnectionString;
        }
    }
}
