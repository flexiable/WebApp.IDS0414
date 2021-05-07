using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace WebApp.IDS0414.Extensions
{

    public class GrantwithApp : IdentityServer4.Validation.IExtensionGrantValidator
    {
        public string GrantType => throw new NotImplementedException();

        public Task ValidateAsync(ExtensionGrantValidationContext context)
        {

            throw new NotImplementedException();
        }
    }
    public static class SeedDataExtensions
    {

        public static void SeedConfigurationData(this IApplicationBuilder builder)
        {
            using (var scope = builder.ApplicationServices.CreateScope())
            {
                var _ConfigurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                var _ApplicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var _PersistedGrantDbContext = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
                _ConfigurationDbContext.Database.EnsureCreated(); _ApplicationDbContext.Database.EnsureCreated();
                if (_ApplicationDbContext.Database.GetPendingMigrations().Any())
                {
                    _ApplicationDbContext.Database.Migrate();
                }
                if (_PersistedGrantDbContext.Database.GetPendingMigrations().Any())
                {
                    _PersistedGrantDbContext.Database.Migrate();
                }
                _PersistedGrantDbContext.Database.EnsureCreated();



                /*
                                foreach (var client in InMemoryConfig.GetClients())
                                {
                                    await cdb.Clients.AddAsync(client.ToEntity());
                                }
                                foreach (var client in InMemoryConfig.GetApiScopes())
                                {
                                    await cdb.ApiScopes.AddAsync(client.ToEntity());
                                }
                                foreach (var client in InMemoryConfig.GetApiResources())
                                {
                                    await cdb.ApiResources.AddAsync(client.ToEntity());
                                }
                                  await  cdb.SaveChangesAsync();*/
            }

        }

    }


    /// <summary>
    /// 拦截器BlogLogAOP 继承IInterceptor接口
    /// </summary>
    public class BlogLogAOP : IInterceptor
    {

    }
}
