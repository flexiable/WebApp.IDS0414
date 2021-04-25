using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.IDP0415
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration; WebHostEnvironment = webHostEnvironment;
        }
        public IWebHostEnvironment WebHostEnvironment { get; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region ¿çÓò
            services.AddCors(options =>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy("AllowSpecificOrigins", policy =>
                {
                    policy. AllowAnyOrigin()
                    
                    //WithOrigins("http://192.168.0.136:422").AllowCredentials()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            #endregion
            services.AddMvcCore().AddAuthorization(
                option =>
                     option.AddPolicy("Weatherpolicy",builder=>builder.RequireScope("ZTApiResource.scope"))
                );
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
          .AddIdentityServerAuthentication(options =>
          {
              options.Authority = "http://192.168.0.136:422";
              options.RequireHttpsMetadata = false;
              
              options.ApiName = "ZTApiResource1";
              options.ApiSecret = "secret";
          });
     

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //¿çÓò·ÃÎÊ
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("AllowSpecificOrigins");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
