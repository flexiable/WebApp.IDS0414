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
            // ���� Serilog 
            Log.Logger = new LoggerConfiguration()
                // ��С����־�������
                .MinimumLevel.Information()
                // ��־�����������ռ������ Microsoft ��ͷ��������־�����С����Ϊ Information
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                // ������־���������̨
                .WriteTo.Console()
                // ������־������ļ����ļ��������ǰ��Ŀ�� logs Ŀ¼��
                // �ռǵ���������Ϊÿ��
                .WriteTo.File(Path.Combine("logs", @"log.txt"), rollingInterval: RollingInterval.Day).CreateLogger();
            // ���� logger



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
