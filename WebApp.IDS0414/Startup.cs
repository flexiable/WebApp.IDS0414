using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WebApp.IDS0414.Extensions;
using Microsoft.AspNetCore.Authorization;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;

using Microsoft.AspNetCore.Server.Kestrel.Core;
using IDS.Infrastructure;

namespace WebApp.IDS0414
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }
    
        public Startup(IConfiguration configuration, IWebHostEnvironment environment )
        {
           
            Configuration = configuration;
            Environment = environment;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
           // services.AddSameSiteCookiePolicy();
           
          var IsMySql=  Configuration.GetValue<bool>("IsMySql");
            string migrationsAssembly = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            string connectionString = Configuration.GetConnectionString("ServerStr");
            services.AddDbContext<ApplicationDbContext>(Option => {

               var a= !IsMySql ?Option.UseSqlServer(connectionString):Option.UseMySQL(connectionString); })
                .AddIdentity<ApplicationUser, ApplicationRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders()
                .AddUserManager<UserManager<ApplicationUser>>()
            .AddRoleManager<RoleManager<ApplicationRole>>();
            //BaseDbContext
            services.AddDbContext<IDS.Data.BaseDbContext>(Option => {
                var a = !IsMySql ? Option.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)) : Option.UseMySQL(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
            }).AddUnitOfWork<IDS.Data.BaseDbContext>();
            
                // services.AddDbContext<ConfigurationDbContext>().AddUnitOfWork<ConfigurationDbContext>();
                // ??????????????????????????????????????????????????
                var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                string customUrl = null;
                options.UserInteraction = new IdentityServer4.Configuration.UserInteractionOptions
                {
                   
                    LoginUrl = "/oauth2/authorize",//????????
                    LogoutUrl = "/Account/Logout",
                    ConsentUrl = customUrl + "/Consent/Index",//????????????????????????????
                    ErrorUrl = customUrl + "/Error/Index", //????????????????????
                    LoginReturnUrlParameter = "returnUrl",//????????????????????????????????URL??????????????????returnUrl 
                    LogoutIdParameter = "logoutId", //????????????????????????????????????ID??????????????????logoutId 
                    ConsentReturnUrlParameter = "returnUrl", //????????????????????????????????URL??????????????????returnUrl
                    ErrorIdParameter = "errorId", //????????????????????????????????????ID??????????????????errorId
                    CustomRedirectReturnUrlParameter = "returnUrl", //??????????????????????????????????????????????URL??????????????????returnUrl
                    CookieMessageThreshold = 5 //????????????????????Cookie??????????????????Cookies????????????????????????????????????????????????????????Cookies??????????????????Cookies??
                };
                options.Caching = new IdentityServer4.Configuration.CachingOptions
                {
                    ClientStoreExpiration = new TimeSpan(1, 0, 0),//????Client?????????????????????????????????????????????? 
                    ResourceStoreExpiration = new TimeSpan(1, 0, 0),// ??????????????????????????API??????????????????????
                    CorsExpiration = new TimeSpan(1, 0, 0)  //??????????????????????????????????????
                };
                //IdentityServer??????????????CORS??????CORS????????ASP.NET Core????????????????????????????????????????
                options.Cors = new IdentityServer4.Configuration.CorsOptions
                {
                    
                    CorsPaths = new List<PathString>() {
                           new PathString("/")
                       }, //????CORS??IdentityServer????????????????????????????????????????????????

                    CorsPolicyName = "default-CorsPolicyName", //??????????CORS??????????IdentityServer??CORS??????????????????"IdentityServer4"??????????????????????????????ICorsPolicyService??????????????????????????????????????????????????CORS??????????????????????????????????ICorsPolicyService
                    PreflightCacheDuration = new TimeSpan(1, 0, 0)//????????<TimeSpan>??????????????Access-Control-Max-Age????????????????????????????????????????????????????????
                };
               // options.Csp = null;
            })
                .AddAspNetIdentity<ApplicationUser>()
                .AddConfigurationStore(option =>
                {

                    
                    if (!IsMySql)
                    {

                    option.ConfigureDbContext = opt => opt.UseSqlServer(connectionString,
                              sql => sql.MigrationsAssembly(migrationsAssembly));
                    }
                    else
                    {
                        option.ConfigureDbContext = opt => opt.UseMySQL(connectionString,
                             sql => sql.MigrationsAssembly(migrationsAssembly));
                    }
                })
                .AddOperationalStore(option =>
                {

                   
                    if (!IsMySql)
                    {

                        option.ConfigureDbContext = opt => opt.UseSqlServer(connectionString,
                                  sql => sql.MigrationsAssembly(migrationsAssembly));
                    }
                    else
                    {
                        option.ConfigureDbContext = opt => opt.UseMySQL(connectionString,
                             sql => sql.MigrationsAssembly(migrationsAssembly));
                    }
                    // ???????? token ??????
                    option.EnableTokenCleanup = true;
                });

            /*// in-memory, code config
            .AddTestUsers(InMemoryConfig.Users().ToList())
            .AddInMemoryApiResources(InMemoryConfig.GetApiResources())
            .AddInMemoryApiScopes(InMemoryConfig.GetApiScopes())
           // .AddInMemoryIdentityResources(InMemoryConfig.GetApiResources())
            .AddInMemoryClients(InMemoryConfig.GetClients());
*/
         /*   services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
        {
            o.LoginPath = new PathString("/oauth2/authorize");
            o.AccessDeniedPath = new PathString("/Error/Forbidden");
        });*/
            builder.AddDeveloperSigningCredential();

            /*if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                throw new Exception("need to configure key material");
            }*/
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = ".AspNetCoreIdentityCookie";
                options.Cookie.HttpOnly = true;options.Cookie.SameSite = SameSiteMode.Unspecified;
                options.ExpireTimeSpan = TimeSpan.FromDays(3);
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
                options.LoginPath = new PathString("/oauth2/authorize");
                
           /*     options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.Headers["Location"] = context.RedirectUri;
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.Headers["Location"] = context.RedirectUri;
                    context.Response.StatusCode = 403;
                    return Task.CompletedTask;
                };*/
            });
            //????session??????????,??????
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });
            services.AddControllersWithViews(option=>
            {
                option.Filters.Add(typeof(HttpGlobalExceptionFilter));
                //option.Filters.Add(typeof(LoginAuthGlobalFilter));
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.Requirements.Add(new ClaimRequirement("rolename", "Admin")));
                options.AddPolicy("SuperAdmin", policy => policy.Requirements.Add(new ClaimRequirement("rolename", "AdminTest")));
            });
            services.Configure<KestrelServerOptions>(x => x.AllowSynchronousIO = true)
                .Configure<IISServerOptions>(x => x.AllowSynchronousIO = true);

            services.AddSingleton<IAuthorizationHandler, ClaimsRequirementHandler>();
            services.AddScoped<IProfileService, ProfileServices>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
          
            //app.UseForwardedHeaders();
            //  app.UseCookiePolicy();
        //    app.SeedConfigurationData();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
            defaultFilesOptions.DefaultFileNames.Clear();
            defaultFilesOptions.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(defaultFilesOptions);
            app.UseStaticFiles(); 

            app.UseSession();
            app.UseStaticFiles();
            app.UseRouting(); app.UseLogger();
            app.UseIdentityServer();

            app.UseAuthentication();
            app.UseAuthorization();
            app.Use(async (ctx, _next) =>
            { 
                logger.LogInformation("Content-Security-Policy"+ctx.Response.Headers.Count());
                if (ctx.Response.Headers.ContainsKey("Content-Security-Policy"))
                {

                    ctx.Response.Headers.Remove("Content-Security-Policy");
                }

                await _next.Invoke();
            });
            
           
            app.UseEndpoints(endpoints =>
            {
                
                //endpoints.Filters.Add<HttpGlobalExceptionFilter>();//????????
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            /* app.UseEndpoints(endpoints =>
             {
                 endpoints.MapGet("/", async context =>
                 {
                     await context.Response.WriteAsync("Hello World!");
                 });
             });*/
        }
    }
}
