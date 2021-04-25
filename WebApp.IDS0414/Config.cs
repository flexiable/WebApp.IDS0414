using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace WebApp.IDS0414
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>()
            {
                new ApiResource("api1","this is api",new List<string>{ClaimTypes.Role })
                  {
                    ApiSecrets = { new Secret("secret".Sha256()) }
                }
            };
        }
        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser>{
                new TestUser()
                {
                    SubjectId="123",
                    Username="Mr.wen",
                    Password="123465",
                    Claims=new Claim[]
                    {
                        new Claim(ClaimTypes.Role,"管理员")
                    }

                },
                new TestUser()
                {
                    SubjectId="456",
                    Username="123",
                    Password="123456",
                    Claims=new Claim[]
                    {
                        new Claim(ClaimTypes.Role,"阅览者")
                    }

                }

            };
        }
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>()
            {
             new Client()
             {
                  SlidingRefreshTokenLifetime=10, 
                 //AllowedGrantTypes = new List<string>(){ GrantTypes.ClientCredentials.FirstOrDefault() }, 
                 AllowedGrantTypes = GrantTypes.ClientCredentials,
                   ClientId="ClientID", ClientName="Mvc_Name",
                 ClientSecrets ={   new Secret("secret".Sha256()) },AllowedScopes= {"api1" },
                   AccessTokenType= AccessTokenType.Reference
             }
            
            };
        }
    }
}