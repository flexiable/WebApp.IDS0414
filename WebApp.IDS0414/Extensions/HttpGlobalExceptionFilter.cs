using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WebApp.IDS0414.Extensions
{
    public class HttpGlobalExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<HttpGlobalExceptionFilter> _logger;
        public HttpGlobalExceptionFilter(ILogger<HttpGlobalExceptionFilter> logger)
         {
             _logger = logger;
         }
    public override void OnException(ExceptionContext context)
         {
           
             var actionName = context.HttpContext.Request.RouteValues["controller"] + "/" + context.HttpContext.Request.RouteValues["action"];
             _logger.LogError($"--------{actionName} Error Begin--------");
             _logger.LogError($"  Error Detail:{ context.Exception.Message}\r {context.Exception.StackTrace}");
             //拦截处理
             if (!context.ExceptionHandled)
             {
                 context.Result = new JsonResult(new
                 {
                     status = false,
                     msg = context.Exception.Message
                 })
                 { StatusCode =500 }; 
                 context.ExceptionHandled = true;
             }
             _logger.LogError($"--------{actionName} Error End--------");
         }
     }
}
