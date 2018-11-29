using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nethium.Demo.Abstraction;
using Nethium.Demo.Service.Aggregate;
using Nethium.Demo.Service.ToRoman;
using Nethium.Demo.Stub;

namespace Nethium.Demo.Web3
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddStub(Assembly.GetAssembly(typeof(SwaggerException)));
            services.RegisterInterface<IStoreService>("store");
            services.RegisterInterface<IAggregateService>("aggregate");
            services.RegisterInterface<IToRomanService>("toRoman");
            services.RegisterInterface<ICalcToRomanService>("calcToRoman");
            services.RegisterInterface<ICalcService>("calc");
            services.RegisterService<IAggregateService, AggregateController>("/api/aggregate", "/api/aggregate/health");
            services.RegisterService<IToRomanService, ToRomanController>("/api/toRoman", "/api/toRoman/health");

            services.AddControllers()
                .AddNethiumControllers();

            services.AddNethiumServices();
            services.AddOpenApiDocument();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseSwagger();
            app.UseSwaggerUi3();
        }
    }
}