using IDS.Entity;
using IDS.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.IDS0414.Extensions
{
    public static class LoggerMiddlewareExtension
    {

        public static void UseLogger(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<LoggerMiddleware>();
        }
    }
    /// <summary>
    /// HTTP服务日志中间件
    /// <para>支持Request、Response信息输出</para>
    /// <para>支持请求处理耗时输出</para>
    /// </summary>
    public class LoggerMiddleware
    {
        const string _FileName = "请求日志";
        private readonly ILogger<LoggerMiddleware> _logger;
        private readonly RequestDelegate _Next;
        private readonly IUnitOfWork _unitOfWork;
        public IConfiguration Configuration { get; }
        /// <summary>
        /// 不记录日志的请求类型
        /// </summary>
        /// <summary>
        /// 日志中间件
        /// </summary>
        /// <param name="next"></param>
        public LoggerMiddleware(RequestDelegate next, IUnitOfWork unitOfWork, IConfiguration configuration, ILogger<LoggerMiddleware> logger)
        {
            Configuration = configuration;
            _unitOfWork = unitOfWork;
            _logger = logger;
              _Next = next;
        }

        /// <summary>
        /// 请求拦截处理
        /// </summary>
        /// <param name="context">HTTP请求</param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            string fPath = context.Request.Path.ToString().ToLower();
        
            if (context.Request.Method.ToLower()=="get")
            {
                await _Next(context);
                return;
            }
            context.Request.EnableBuffering();
           

            var requestReader = new StreamReader(context.Request.Body);
            var requestContent = requestReader.ReadToEnd();
            StringBuilder fLog = new StringBuilder();
            fLog.AppendLine($"Request:{context.Request.Method}:{context.Request.Path}{context.Request.QueryString}");
            fLog.AppendLine($"Request Body:{requestContent}");
            _logger.LogInformation(fLog.ToString());
            context.Request.Body.Position = 0;

            Stream originalBody = context.Response.Body;
            try
            {
              
                using var ms = new MemoryStream();
                context.Response.Body = ms;
                var fWatch = new Stopwatch();
                fWatch.Start();
             
                if (context.Request.Path.Value.Contains("token"))
                {
                    #region Token请求次数已超出限制

                  
                    var ClientId =requestContent.Split('&').FirstOrDefault().Split('=').LastOrDefault();
                    var repoTokenlog = _unitOfWork.GetRepository<ClientTokenLog>();
                    var AccessTokenPerDayLimit= Configuration.GetValue<int>("AccessTokenPerDayLimit");
 
                    Expression<Func<ClientTokenLog, bool>> expression = t => true;
                    expression = expression.And(X => X.ClientId == ClientId);
                    string dn = DateTime.Now.ToString("yyyy-MM-dd 00:00:59");
                    expression = expression.And(X => X.CreateTime > DateTime.Parse(dn));//.AddDays(1)
                    if (repoTokenlog.LongCount(expression) >AccessTokenPerDayLimit)
                    {
                        _logger.LogInformation($"今日Token请求次数已超出限制。ClientId:{ClientId}");
                       await ms.WriteAsync(Encoding.UTF8.GetBytes("今日Token请求次数已超出限制。"));
                        //throw new Exception("今日Token请求次数已超出限制。");
                    }
                    else
                    {
                        await _Next(context);
                        fWatch.Stop();
                        ms.Position = 0;
                        string responseBody = new StreamReader(ms).ReadToEnd();
                        _logger.LogInformation($"Response Body({fWatch.ElapsedMilliseconds}ms):\r\n{responseBody}");
                        await repoTokenlog.InsertAsync(new ClientTokenLog()
                        {
                            ClientId = ClientId,
                            ClientRequestParam = requestContent,
                            ClientResponseBody = responseBody,
                             RemoteIpAddress = context.Request.Host.ToString(),
                            LocalIpAddress = context.Connection.LocalIpAddress.ToString(),
                        UserId = context.User.Claims.FirstOrDefault(X => X.Type == "sub").Value
                        }); ;
                   await _unitOfWork.SaveChangesAsync();
                    }
                    #endregion
                }
                else
                {
                    await _Next(context);
                    fWatch.Stop();
                    ms.Position = 0;
                    string responseBody = new StreamReader(ms).ReadToEnd();
                    _logger.LogInformation($"Response Body({fWatch.ElapsedMilliseconds}ms):\r\n{responseBody}");
                }
                ms.Position = 0;
                await ms.CopyToAsync(originalBody);
            }
            finally
            {
                context.Response.Body = originalBody;
            }
        }
    }

}
