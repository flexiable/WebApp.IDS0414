using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.IDS0414
{
    public class Program
    {
        public static async Task  Main(string[] args)
        {
            // 配置 Serilog 
            Log.Logger = new LoggerConfiguration()
                // 最小的日志输出级别
                .MinimumLevel.Information()
                // 日志调用类命名空间如果以 Microsoft 开头，覆盖日志输出最小级别为 Information
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                // 配置日志输出到控制台
                .WriteTo.Console()
                // 配置日志输出到文件，文件输出到当前项目的 logs 目录下
                // 日记的生成周期为每天
                .WriteTo.File(Path.Combine("logs", @"log.txt"), rollingInterval: RollingInterval.Day).CreateLogger();
            // 创建 logger



            try
            {
                Log.Information("Starting WebApp.");

                await CreateHostBuilder(args).UseSerilog().UseDefaultServiceProvider(options =>
                {
                    options.ValidateScopes = false;
                }).Build().RunAsync();
             
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "WebApp terminated unexpectedly!");
            }
            finally
            {
                Log.CloseAndFlush();
            }
           // CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().UseUrls("http://192.168.0.136:1919");
                });
    }
}
