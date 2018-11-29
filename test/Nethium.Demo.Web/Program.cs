using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nethium.Core;

namespace Nethium.Demo.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseNethium(args, CreateLoggerFactory())
                .UseStartup<Startup>();

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