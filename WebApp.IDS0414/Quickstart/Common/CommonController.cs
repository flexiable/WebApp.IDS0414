using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.IDS0414.Quickstart.Common
{
    public class CommonController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [ResponseCache(Duration =36000)]
        public IActionResult GetIdentityServerConstants()
        {
            var gt = typeof(GrantType); 
            var GrantTypes=gt.GetFields().Select(X => new { name = X.Name, value = X.GetValue(gt).ToString() }).ToList();

            return Json(GrantTypes);
        }
    }
}
