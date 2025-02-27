﻿using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Cli;

namespace LINGYUN.Abp.Cli
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Volo.Abp", LogEventLevel.Warning)
                .MinimumLevel.Override("LINGYUN.Abp", LogEventLevel.Warning)
                .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
#if DEBUG
                .MinimumLevel.Override("Volo.Abp.Cli", LogEventLevel.Debug)
                .MinimumLevel.Override("LINGYUN.Abp.Cli", LogEventLevel.Debug)
#else
                .MinimumLevel.Override("Volo.Abp.Cli", LogEventLevel.Information)
                .MinimumLevel.Override("LINGYUN.Abp.Cli", LogEventLevel.Information)
#endif
                .Enrich.FromLogContext()
                .WriteTo.File(Path.Combine(CliPaths.Log, "lingyun-abp-cli-logs.txt"))
                .WriteTo.Console()
                .CreateLogger();

            using (var application = AbpApplicationFactory.Create<AbpCliModule>(
                options =>
                {
                    options.UseAutofac();
                    options.Services.AddLogging(c => c.AddSerilog());
                }))
            {
                application.Initialize();

                await application.ServiceProvider
                    .GetRequiredService<CliService>()
                    .RunAsync(args);

                application.Shutdown();

                Log.CloseAndFlush();
            }
        }
    }
}
