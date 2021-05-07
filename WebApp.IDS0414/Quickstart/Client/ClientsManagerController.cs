using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDS.Infrastructure;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.IDS0414.Extensions;
using WebApp.IDS0414.Quickstart.Common;
using static IdentityModel.OidcConstants;

namespace WebApp.IDS0414.Controllers.Client
{
    [Route("[controller]/[action]")]
    [ApiController]
    
    public class ClientsManagerController : Controller
    {
        
        private readonly ConfigurationDbContext _configurationDbContext;

        public ClientsManagerController(ConfigurationDbContext configurationDbContext)
        {
            
            _configurationDbContext = configurationDbContext;
        }
       
        /// <summary>
        /// 数据列表页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {


            return View(await _configurationDbContext.Clients
                .Include(d => d.AllowedGrantTypes)
                .Include(d => d.AllowedScopes)
                .Include(d => d.AllowedCorsOrigins)
                .Include(d => d.RedirectUris)
                .Include(d => d.PostLogoutRedirectUris)
                .ToListAsync());
        }
        [HttpPost]
        [Authorize]
        public async Task<ResponsePaging<ClientDto>> GetAllClientPaging(Paging<ClientQueryParam> paging)
        {
            
            var model = (await _configurationDbContext.Clients.Include(d => d.AllowedGrantTypes).ToListAsync()).Skip((paging.PageNum - 1)*paging.PageSize).Take(paging.PageSize).Where(X=>X.ClientId.Contains(paging.Filter.clientIdOrname)||X.ClientName.Contains(paging.Filter.clientIdOrname));


            return new ResponsePaging<ClientDto>()
            {
                total = model.Count(),
                rows = model.Select(X => new ClientDto
                {
                    id = X.Id,
                    ClientId = X.ClientId,
                    Enabled = X.Enabled,
                    Description = X.Description,
                    AllowedGrantTypes = X.AllowedGrantTypes?.FirstOrDefault()?.GrantType??""

                }).ToList()
            };
        }
        /// <summary>
        /// 数据列表页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index2Scope()
        {
            return View(await _configurationDbContext.ApiScopes
                .ToListAsync());
        }
        /// <summary>
        /// 数据修改页
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = "SuperAdmin")]
        public IActionResult CreateOrEdit2Scope(int id)
        {
            ViewBag.ScopeId = id;
            return View();
        }
        /// <summary>
        /// 数据修改页
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = "SuperAdmin")]
        public IActionResult CreateOrEdit(int id)
        {
            ViewBag.ClientId = id;
            var StandardScopes= typeof(StandardScopes).GetFields();
           // ViewBag.GrantType =Newtonsoft.Json.JsonConvert.SerializeObject( gt.GetFields().Select(X => new { name = X.Name, value = X.GetValue(gt).ToString() }).ToList());
            var StandardScopes2Api = new List<IdentityServer4.EntityFramework.Entities.ApiScope>();
            foreach (var StandardScope in StandardScopes)
            {
                StandardScopes2Api.Add(new IdentityServer4.EntityFramework.Entities.ApiScope() { Name  = StandardScope.GetValue(StandardScopes).ToString(), DisplayName = StandardScope.Name });
            }
            StandardScopes2Api.AddRange(_configurationDbContext.ApiScopes);
            ViewBag.Scopes =   ((StandardScopes2Api));
            return View();
        }

        [HttpGet]
        [Authorize(Policy = "SuperAdmin")]
        public async Task<MessageModel<ApiScopeDto>> GetDataById2Scope(int id = 0)
        {
            var model = (await _configurationDbContext.ApiScopes.FindAsync( id))?.ToModel();


            return new MessageModel<ApiScopeDto>()
            {
                success = true,
                msg = "获取成功",
                response = new ApiScopeDto()
                {
                    id = id, Description=model.Description,name=model.Name,displayName=model.DisplayName

                }
            };
        }

        [HttpPost]
        [Authorize(Policy = "SuperAdmin")]
        public async Task<MessageModel<string>> SaveData2Scope(ApiScopeDto request)
        {
            try
            {

         
            if (request.id==0)
            {
                await _configurationDbContext.ApiScopes.AddAsync(new IdentityServer4.EntityFramework.Entities.ApiScope()
                {
                    Name = request.name, DisplayName=request.displayName, Description=request.displayName
                }) ;
                await _configurationDbContext.SaveChangesAsync();
                

            }
            else
            {
            var model = (await _configurationDbContext.ApiScopes.FindAsync(request.id));
                model.Name = request.name; model.DisplayName = request.displayName; model.Description = request.displayName;
                 _configurationDbContext.ApiScopes.Update(model);
                await _configurationDbContext.SaveChangesAsync();
            }


            return new MessageModel<string>()
            {
                success = true,
                msg = "添加成功",
            };
            }
            catch (Exception)
            {
                return new MessageModel<string>()
                {
                    success = false,
                    msg = "添加失败",
                };
            }
        }
        [HttpGet]
        [Authorize(Policy = "SuperAdmin")]
        public  MessageModel<ClientDto> GetDataById(int id = 0)
        {
            var model = ( _configurationDbContext.Clients
                .Include(d => d.AllowedGrantTypes)
                .Include(d => d.AllowedScopes)
                .Include(d => d.AllowedCorsOrigins)
                .Include(d => d.RedirectUris)
                .Include(d => d.PostLogoutRedirectUris)
                .Include(d => d.ClientSecrets)//.AsSplitQuery()
                .ToList()).FirstOrDefault(d => d.Id == id).ToModel();

            var clientDto = new ClientDto();
            if (model != null)
            {
                clientDto = new ClientDto()
                {
                    ClientId = model?.ClientId,
                    ClientName = model?.ClientName,
                    Description = model?.Description,
                    AllowedCorsOrigins = string.Join(",", model?.AllowedCorsOrigins),
                    AllowedGrantTypes = string.Join(",", model?.AllowedGrantTypes),
                    AllowedScopes = string.Join(",", model?.AllowedScopes),
                    PostLogoutRedirectUris = string.Join(",", model?.PostLogoutRedirectUris),
                    RedirectUris = string.Join(",", model?.RedirectUris),
                    ClientSecrets = string.Join(",", model?.ClientSecrets.Select(d => d.Value)),
                    AccessTokenLifetime=model?.AccessTokenLifetime
                };
            }

            return new MessageModel<ClientDto>()
            {
                success = true,
                msg = "获取成功",
                response = clientDto
            };
        }

        [HttpPost]
        [Authorize(Policy = "SuperAdmin")]
        public async Task<MessageModel<string>> SaveData(ClientDto request)
        {
            if (request != null && request.id == 0)
            {
                IdentityServer4.Models.Client client = new IdentityServer4.Models.Client()
                {
                    ClientId = request?.ClientId,
                    ClientName = request?.ClientName,
                    AllowOfflineAccess= request.AllowedScopes.Contains(",")? request.AllowedScopes.Split(",").Contains(StandardScopes.OfflineAccess):false,
                    Description = request?.Description,
                    AccessTokenLifetime=request.AccessTokenLifetime.Value,
                    AllowedCorsOrigins = request?.AllowedCorsOrigins?.Split(","),
                    AllowedGrantTypes = request?.AllowedGrantTypes?.Split(","),
                    AllowedScopes = request?.AllowedScopes?.Split(","),
                    PostLogoutRedirectUris = request?.PostLogoutRedirectUris?.Split(","),
                    RedirectUris = request?.RedirectUris?.Split(","),
                };

                if (!string.IsNullOrEmpty(request.ClientSecrets))
                {
                    client.ClientSecrets = new List<Secret>() { new Secret(request.ClientSecrets.Sha256()) };
                }
                if (_configurationDbContext.Clients.FirstOrDefault(X=>X.ClientId==client.ClientId)!=null)
                {
                    throw new Exception("已经存在相同客户端Id");
               // ModelState.AddModelError("", "已经存在相同客户端Id");
                }
                var result = (await _configurationDbContext.Clients.AddAsync(client.ToEntity()));
                await _configurationDbContext.SaveChangesAsync();
            }

            if (request != null && request.id > 0)
            {

                var modelEF = (await _configurationDbContext.Clients
                    .Include(d => d.AllowedGrantTypes)
                    .Include(d => d.AllowedScopes)
                    .Include(d => d.AllowedCorsOrigins)
                    .Include(d => d.RedirectUris)
                    .Include(d => d.PostLogoutRedirectUris)
                    .ToListAsync()).FirstOrDefault(d => d.Id == request.id);
                modelEF.AccessTokenLifetime = request.AccessTokenLifetime.Value;
                modelEF.ClientId = request?.ClientId;
                modelEF.ClientName = request?.ClientName;
                modelEF.Description = request?.Description;
                modelEF.AllowOfflineAccess = request.AllowedScopes.Contains(",") ? request.AllowedScopes.Split(",").Contains(StandardScopes.OfflineAccess) : false;
                var AllowedCorsOrigins = new List<IdentityServer4.EntityFramework.Entities.ClientCorsOrigin>();
                if (!string.IsNullOrEmpty(request?.AllowedCorsOrigins))
                {
                    request?.AllowedCorsOrigins.Split(",").Where(s => s != "" && s != null).ToList().ForEach(s =>
                               {
                                   AllowedCorsOrigins.Add(new IdentityServer4.EntityFramework.Entities.ClientCorsOrigin()
                                   {
                                       Client = modelEF,
                                       ClientId = modelEF.Id,
                                       Origin = s
                                   });
                               });
                    modelEF.AllowedCorsOrigins = AllowedCorsOrigins;
                }



                var AllowedGrantTypes = new List<IdentityServer4.EntityFramework.Entities.ClientGrantType>();
                if (!string.IsNullOrEmpty(request?.AllowedGrantTypes))
                {
                    request?.AllowedGrantTypes.Split(",").Where(s => s != "" && s != null).ToList().ForEach(s =>
                    {
                        AllowedGrantTypes.Add(new IdentityServer4.EntityFramework.Entities.ClientGrantType()
                        {
                            Client = modelEF,
                            ClientId = modelEF.Id,
                            GrantType = s
                        });
                    });
                    modelEF.AllowedGrantTypes = AllowedGrantTypes;
                }



                var AllowedScopes = new List<IdentityServer4.EntityFramework.Entities.ClientScope>();
                if (!string.IsNullOrEmpty(request?.AllowedScopes))
                {
                    request?.AllowedScopes.Split(",").Where(s => s != "" && s != null).ToList().ForEach(s =>
                    {
                        AllowedScopes.Add(new IdentityServer4.EntityFramework.Entities.ClientScope()
                        {
                            Client = modelEF,
                            ClientId = modelEF.Id,
                            Scope = s
                        });
                    });
                    modelEF.AllowedScopes = AllowedScopes;
                }


                var PostLogoutRedirectUris = new List<IdentityServer4.EntityFramework.Entities.ClientPostLogoutRedirectUri>();
                if (!string.IsNullOrEmpty(request?.PostLogoutRedirectUris))
                {
                    request?.PostLogoutRedirectUris.Split(",").Where(s => s != "" && s != null).ToList().ForEach(s =>
                    {
                        PostLogoutRedirectUris.Add(new IdentityServer4.EntityFramework.Entities.ClientPostLogoutRedirectUri()
                        {
                            Client = modelEF,
                            ClientId = modelEF.Id,
                            PostLogoutRedirectUri = s
                        });
                    });
                    modelEF.PostLogoutRedirectUris = PostLogoutRedirectUris;
                }


                var RedirectUris = new List<IdentityServer4.EntityFramework.Entities.ClientRedirectUri>();
                if (!string.IsNullOrEmpty(request?.RedirectUris))
                {
                    request?.RedirectUris.Split(",").Where(s => s != "" && s != null).ToList().ForEach(s =>
                    {
                        RedirectUris.Add(new IdentityServer4.EntityFramework.Entities.ClientRedirectUri()
                        {
                            Client = modelEF,
                            ClientId = modelEF.Id,
                            RedirectUri = s
                        });
                    });
                    modelEF.RedirectUris = RedirectUris;
                }

                var result = (_configurationDbContext.Clients.Update(modelEF));
                await _configurationDbContext.SaveChangesAsync();
            }


            return new MessageModel<string>()
            {
                success = true,
                msg = "添加成功",
            };
        }
    }

    internal class SelectModel
    {
        public string name { get; set; }
        public string value { get; set; }
    }
}