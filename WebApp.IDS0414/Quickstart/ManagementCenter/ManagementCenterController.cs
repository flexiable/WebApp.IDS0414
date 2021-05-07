using IDS.Infrastructure;
using IDS.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.IDS0414.Controllers;

namespace WebApp.IDS0414.Quickstart.ManagementCenter
{
    [Authorize]
    public class ManagementCenterController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public IActionResult Index()
        {
            return View();
        }
        public ManagementCenterController(IUnitOfWork unitOfWork)
        {
            _unitOfWork= unitOfWork ;
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponsePaging<ClientTokenLog>> GetAllClientPaging(Paging<ClientTokenLogQueryParam> paging)
        {
            var model =await _unitOfWork.GetRepository<ClientTokenLog>().GetPagedListAsync(X => new ClientTokenLog { }, pageIndex:paging.PageNum,pageSize:paging.PageSize);


           // var model = (await _configurationDbContext.Clients.Include(d => d.AllowedGrantTypes).ToListAsync()).Skip((paging.PageNum - 1) * paging.PageSize).Take(paging.PageSize).Where(X => X.ClientId.Contains(paging.Filter.clientIdOrname) || X.ClientName.Contains(paging.Filter.clientIdOrname));


            return new ResponsePaging<ClientTokenLog>()
            {
                total = model.TotalCount,
                rows = model.Items.ToList()
            };
        }
    }
}
