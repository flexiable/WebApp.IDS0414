using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.IDS0414
{
    public static class InMemoryConfig
    {
        // 这个 Authorization Server 保护了哪些 API （资源）
        public static IEnumerable<IdentityResource> GetIdentityResource()
        {
            return new[]
            {
                    new IdentityResources.OpenId(), new IdentityResource("roles", "角色", new List<string> { JwtClaimTypes.Role }),
                };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new[]
            { new ApiResource("api1")
                {
                    Scopes={ "api422", "ZTApiResource.scope" }
                },
                     new ApiResource("roles", "角色", new List<string> { JwtClaimTypes.Role }),
                };
        }
        // v4更新
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new ApiScope[] {
                 new ApiScope("api422"),
                 new ApiScope("api422.BlogModule"),
            };
        }

      
        // 哪些客户端 Client（应用） 可以使用这个 Authorization Server
        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                    new Client
                    {

                        ClientId = "blogvuejs",//定义客户端 Id
                        RefreshTokenExpiration= TokenExpiration.Sliding,

                        AlwaysIncludeUserClaimsInIdToken=true,
                        AllowOfflineAccess=true,

                        ClientSecrets = new [] { new Secret("secret".Sha256()) },//Client用来获取token
                        AllowedGrantTypes =GrantTypes.DeviceFlow, //这里使用的是通过用户名密码和ClientCredentials来换取token的方式. ClientCredentials允许Client只使用ClientSecrets来获取token. 这比较适合那种没有用户参与的api动作
                        AllowedScopes = new [] { "api422",   IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess, }// 允许访问的 API 资源
                    },
                    new Client{
       ClientId="mvc client", //客户端Id
       ClientName="测试客户端", //客户端名称 随便写
       AllowedGrantTypes=GrantTypes.Code,//验证模式
       ClientSecrets=
       {
           new Secret("mvc secret".Sha256()) //客户端验证密钥
       },
       // 登陆以后 我们重定向的地址(客户端地址)，
       // {客户端地址}/signin-oidc是系统默认的不用改，也可以改，这里就用默认的
       RedirectUris = { "http://localhost:5003/signin-oidc" },
       //注销重定向的url
       PostLogoutRedirectUris = { "http://localhost:5003/signout-callback-oidc" },
       //是否允许申请 Refresh Tokens
       //参考地址 https://identityserver4.readthedocs.io/en/latest/topics/refresh_tokens.html
       AllowOfflineAccess=true,
       //将用户claims 写人到IdToken,客户端可以直接访问
       AlwaysIncludeUserClaimsInIdToken=true,
       //客户端访问权限
       AllowedScopes =
       {
           "api422",
           IdentityServerConstants.StandardScopes.OpenId,
           IdentityServerConstants.StandardScopes.Email,
           IdentityServerConstants.StandardScopes.Address,
           IdentityServerConstants.StandardScopes.Phone,
           IdentityServerConstants.StandardScopes.Profile,
           IdentityServerConstants.StandardScopes.OfflineAccess
       }
   }
                };
        }
        // 指定可以使用 Authorization Server 授权的 Users（用户）
        public static IEnumerable<TestUser> Users()
        {
            return new[]
            {
                    new TestUser
                    {
                        SubjectId = "1",
                        Username = "laozhang",
                        Password = "laozhang",Claims={ new System.Security.Claims.Claim(JwtClaimTypes.Name,"laozhang")
                        , new System.Security.Claims.Claim(JwtClaimTypes.Role,"administrator")}
                    }
            };
        }
    }
}
