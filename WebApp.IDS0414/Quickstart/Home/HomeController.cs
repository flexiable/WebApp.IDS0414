// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WebApp.IDS0414;

namespace IdentityServerHost.Quickstart.UI
{
   
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IIdentityServerInteractionService interaction, IWebHostEnvironment environment, ILogger<HomeController> logger)
        {
            _userManager = userManager;
               _signInManager = signInManager;
            _interaction = interaction;
            _environment = environment;
            _logger = logger;
        }
        public IActionResult Callback()
        {
            return View();
        }

        public IActionResult Index()
        {
            if (_environment.IsDevelopment())
            {
                // only show in development
            }
            if (_signInManager.GetExternalLoginInfoAsync().Result != null)
            {

                _signInManager.ExternalLoginSignInAsync(_signInManager.GetExternalLoginInfoAsync().Result.LoginProvider, _signInManager.GetExternalLoginInfoAsync().Result.ProviderKey, true);
            }
                return View();

           
        }
        [Route("/Home/Index/{name}/{sub}")]
        public async Task<IActionResult> Index(string name,string sub)
        {
            if (_environment.IsDevelopment())
            {
                // only show in development
            }

            var user = new ApplicationUser { UserName = name .Replace(" ",""), NormalizedUserName=name , Id=sub};
            if ( _userManager.FindByIdAsync(sub).Result==null)
            {

          var result=   _userManager.CreateAsync(user);
                if (result.Result.Succeeded)
                {
                     _signInManager.SignInAsync(user, true).Wait();
                }
            }
            else
            {
                await _signInManager.SignInAsync(user, true);
            }
             return Redirect("~/");


        }
        /// <summary>
        /// Shows the error page
        /// </summary>
        public async Task<IActionResult> Error(string errorId)
        {
            var vm = new ErrorViewModel();

            // retrieve error details from identityserver
            var message = await _interaction.GetErrorContextAsync(errorId);
            if (message != null)
            {
                vm.Error = message;

                if (!_environment.IsDevelopment())
                {
                    // only show in development
                    message.ErrorDescription = null;
                }
            }

            return View("Error", vm);
        }
    }
}