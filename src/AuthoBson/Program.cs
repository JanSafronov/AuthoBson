using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AuthoBson.Shared;

namespace AuthoBson
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IDictionary<int, string> parsed = new Dictionary<int, string>();
            CreateHostBuilder(args).ConfigureAppConfiguration((hostContext, hostBuilder) =>
            {
                if (hostContext.HostingEnvironment.IsDevelopment())
                    hostBuilder.AddUserSecrets<Program>();
                if (hostContext.HostingEnvironment.IsProduction())
                {
                    Parser parser = new();
                    parsed = Parser.ParseasCommands(args);
                    parser.ParseConfig(parsed, hostBuilder);
                }
            }).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
