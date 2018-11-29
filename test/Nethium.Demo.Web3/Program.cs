using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nethium.Core;

namespace Nethium.Demo.Web3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseNethium(args, CreateLoggerFactory())
                        .UseStartup<Startup>();

                });

        private static ILoggerFactory CreateLoggerFactory()
        {
            return new ServiceCollection()
                .AddLogging(logging =>
                {
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.AddEventSourceLogger();
                })
                .BuildServiceProvider()
                .GetRequiredService<ILoggerFactory>();
        }
    }
}