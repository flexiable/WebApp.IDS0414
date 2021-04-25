using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using IdentityServer4;
using IdentityModel;
using IdentityServer4.AspNetIdentity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WebApp.IDS0414.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using IdentityServer4.Services;

namespace WebApp.IDS0414
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
           // services.AddSameSiteCookiePolicy();
           
            string connectionString = Configuration.GetConnectionString("SqlServerStr");
            services.AddDbContext<ApplicationDbContext>(Option => { Option.UseSqlServer(connectionString); })
                .AddIdentity<ApplicationUser, ApplicationRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders()
                .AddUserManager<UserManager<ApplicationUser>>()
            .AddRoleManager<RoleManager<ApplicationRole>>();

            // 使用内存存储，密钥，客户端和资源来配置身份服务器。
            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.UserInteraction = new IdentityServer4.Configuration.UserInteractionOptions
                {
                    LoginUrl = "/oauth2/authorize",//登录地址
                    LogoutUrl = "/Account/Logout"
                };
                options.Caching = new IdentityServer4.Configuration.CachingOptions
                {
                    ClientStoreExpiration = new TimeSpan(1, 0, 0),//设置Client客户端存储加载的客户端配置的数据缓存的有效时间 
                    ResourceStoreExpiration = new TimeSpan(1, 0, 0),// 设置从资源存储加载的身份和API资源配置的缓存持续时间
                    CorsExpiration = new TimeSpan(1, 0, 0)  //设置从资源存储的跨域请求数据的缓存时间
                };
                //IdentityServer支持一些端点的CORS。底层CORS实现是从ASP.NET Core提供的，因此它会自动注册在依赖注入系统中
                options.Cors = new IdentityServer4.Configuration.CorsOptions
                {
                    CorsPaths = { "" }, //支持CORS的IdentityServer中的端点。默认为发现，用户信息，令牌和撤销终结点

                    CorsPolicyName = "default", //【必备】将CORS请求评估为IdentityServer的CORS策略的名称（默认为"IdentityServer4"）。处理这个问题的策略提供者是ICorsPolicyService在依赖注入系统中注册的。如果您想定制允许连接的一组CORS原点，则建议您提供一个自定义的实现ICorsPolicyService
                    PreflightCacheDuration = new TimeSpan(1, 0, 0)//可为空的<TimeSpan>，指示要在预检Access-Control-Max-Age响应标题中使用的值。默认为空，表示在响应中没有设置缓存头
                };
            })
                .AddAspNetIdentity<ApplicationUser>()
                .AddConfigurationStore(option =>
                {

                    string migrationsAssembly = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                    option.ConfigureDbContext = opt => opt.UseSqlServer(connectionString,
                              sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(option =>
                {

                    string migrationsAssembly = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                    option.ConfigureDbContext = opt => opt.UseSqlServer(connectionString,
                              sql => sql.MigrationsAssembly(migrationsAssembly));
                    // 自动清理 token ，可选
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
            //配置session的有效时间,单位秒
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });
            services.AddMvc();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.Requirements.Add(new ClaimRequirement("rolename", "Admin")));
                options.AddPolicy("SuperAdmin", policy => policy.Requirements.Add(new ClaimRequirement("rolename", "AdminTest")));
            });
            services.AddSingleton<IAuthorizationHandler, ClaimsRequirementHandler>();
            services.AddScoped<IProfileService, ProfileServices>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseForwardedHeaders();
            //  app.UseCookiePolicy();
            app.SeedConfigurationData();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseSession();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseIdentityServer();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
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
