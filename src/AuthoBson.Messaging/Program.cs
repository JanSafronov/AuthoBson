using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AuthoBson.Shared;

namespace AuthoBson.Messaging
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).ConfigureAppConfiguration((hostContext, hostBuilder) => {
                if (hostContext.HostingEnvironment.IsDevelopment())
                    hostBuilder.AddUserSecrets<Program>();
                if (hostContext.HostingEnvironment.IsProduction())
                {
                    Parser parser = new Parser();
                    parser.ParseConfig(Parser.ParseasCommands(args), hostBuilder);
                }
            }).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
