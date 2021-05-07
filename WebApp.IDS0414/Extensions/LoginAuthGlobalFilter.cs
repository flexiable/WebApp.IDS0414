using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace WebApp.IDS0414.Extensions
{
    public class LoginAuthGlobalFilter : IAuthorizationFilter
    {
        public LoginAuthGlobalFilter()
        {

        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.HttpContext.Response.StatusCode == 401)
            {
                context.Result = new RedirectResult("/oauth2/authorize");
            }
            // throw new NotImplementedException();
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var a=context.HttpContext.Request.HttpContext.User.Identity.IsAuthenticated;
            if (!context.HttpContext.Request.Cookies.TryGetValue(".AspNetCoreIdentityCookie", out _))
            {
                context.Result = new RedirectResult("/oauth2/authorize");
            }
            if (context.HttpContext.Response.StatusCode == 401)
            {
                context.Result = new RedirectResult("/oauth2/authorize");
            }
            // throw new NotImplementedException();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
           var ca= context.ActionDescriptor.EndpointMetadata.FirstOrDefault(X => X.GetType().IsAssignableFrom(typeof(IAllowAnonymous))) == null;

                var aa= context.Filters.FirstOrDefault(X => X.GetType().IsAssignableFrom(typeof(IAllowAnonymous))) == null;
           var ia= context.HttpContext.Request.HttpContext.User.Identity.IsAuthenticated;
            context.HttpContext.Response.Redirect("/oauth2/authorize");
            //Unauthorized
            if (!context.HttpContext.Request.Cookies.TryGetValue(".AspNetCoreIdentityCookie", out _))
            {
               // context.Result = new RedirectResult("/oauth2/authorize");
            }
            
           // throw new NotImplementedException();
        }
    }
}
