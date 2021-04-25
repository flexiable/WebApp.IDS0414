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

            // ʹ���ڴ�洢����Կ���ͻ��˺���Դ��������ݷ�������
            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.UserInteraction = new IdentityServer4.Configuration.UserInteractionOptions
                {
                    LoginUrl = "/oauth2/authorize",//��¼��ַ
                    LogoutUrl = "/Account/Logout"
                };
                options.Caching = new IdentityServer4.Configuration.CachingOptions
                {
                    ClientStoreExpiration = new TimeSpan(1, 0, 0),//����Client�ͻ��˴洢���صĿͻ������õ����ݻ������Чʱ�� 
                    ResourceStoreExpiration = new TimeSpan(1, 0, 0),// ���ô���Դ�洢���ص���ݺ�API��Դ���õĻ������ʱ��
                    CorsExpiration = new TimeSpan(1, 0, 0)  //���ô���Դ�洢�Ŀ����������ݵĻ���ʱ��
                };
                //IdentityServer֧��һЩ�˵��CORS���ײ�CORSʵ���Ǵ�ASP.NET Core�ṩ�ģ���������Զ�ע��������ע��ϵͳ��
                options.Cors = new IdentityServer4.Configuration.CorsOptions
                {
                    CorsPaths = { "" }, //֧��CORS��IdentityServer�еĶ˵㡣Ĭ��Ϊ���֣��û���Ϣ�����ƺͳ����ս��

                    CorsPolicyName = "default", //���ر�����CORS��������ΪIdentityServer��CORS���Ե����ƣ�Ĭ��Ϊ"IdentityServer4"���������������Ĳ����ṩ����ICorsPolicyService������ע��ϵͳ��ע��ġ�������붨���������ӵ�һ��CORSԭ�㣬�������ṩһ���Զ����ʵ��ICorsPolicyService
                    PreflightCacheDuration = new TimeSpan(1, 0, 0)//��Ϊ�յ�<TimeSpan>��ָʾҪ��Ԥ��Access-Control-Max-Age��Ӧ������ʹ�õ�ֵ��Ĭ��Ϊ�գ���ʾ����Ӧ��û�����û���ͷ
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
                    // �Զ����� token ����ѡ
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
            //����session����Чʱ��,��λ��
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
