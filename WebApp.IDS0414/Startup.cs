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
                // 使用内存存储，密钥，客户端和资源来配置身份服务器。
                var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                string customUrl = null;
                options.UserInteraction = new IdentityServer4.Configuration.UserInteractionOptions
                {
                   
                    LoginUrl = "/oauth2/authorize",//登录地址
                    LogoutUrl = "/Account/Logout",
                    ConsentUrl = customUrl + "/Consent/Index",//【必备】允许授权同意页面地址
                    ErrorUrl = customUrl + "/Error/Index", //【必备】错误页面地址
                    LoginReturnUrlParameter = "returnUrl",//【必备】设置传递给登录页面的返回URL参数的名称。默认为returnUrl 
                    LogoutIdParameter = "logoutId", //【必备】设置传递给注销页面的注销消息ID参数的名称。缺省为logoutId 
                    ConsentReturnUrlParameter = "returnUrl", //【必备】设置传递给同意页面的返回URL参数的名称。默认为returnUrl
                    ErrorIdParameter = "errorId", //【必备】设置传递给错误页面的错误消息ID参数的名称。缺省为errorId
                    CustomRedirectReturnUrlParameter = "returnUrl", //【必备】设置从授权端点传递给自定义重定向的返回URL参数的名称。默认为returnUrl
                    CookieMessageThreshold = 5 //【必备】由于浏览器对Cookie的大小有限制，设置Cookies数量的限制，有效的保证了浏览器打开多个选项卡，一旦超出了Cookies限制就会清除以前的Cookies值
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
                    
                    CorsPaths = new List<PathString>() {
                           new PathString("/")
                       }, //支持CORS的IdentityServer中的端点。默认为发现，用户信息，令牌和撤销终结点

                    CorsPolicyName = "default-CorsPolicyName", //【必备】将CORS请求评估为IdentityServer的CORS策略的名称（默认为"IdentityServer4"）。处理这个问题的策略提供者是ICorsPolicyService在依赖注入系统中注册的。如果您想定制允许连接的一组CORS原点，则建议您提供一个自定义的实现ICorsPolicyService
                    PreflightCacheDuration = new TimeSpan(1, 0, 0)//可为空的<TimeSpan>，指示要在预检Access-Control-Max-Age响应标题中使用的值。默认为空，表示在响应中没有设置缓存头
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
                
                //endpoints.Filters.Add<HttpGlobalExceptionFilter>();//全局注册
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
