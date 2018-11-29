using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Nethium.Core;

namespace Nethium.Demo.Web2
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
                        .UseNethium()
                        .UseStartup<Startup>();
                });
    }
}