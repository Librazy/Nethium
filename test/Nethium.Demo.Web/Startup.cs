using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nethium.Demo.Abstraction;
using Nethium.Demo.Service;
using Nethium.Demo.Service.CalcToRoman;
using Nethium.Demo.Stub;

namespace Nethium.Demo.Web
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
            services.RegisterService<IStoreService, StoreController>("/api/store", "/api/store/health");
            services.RegisterService<ICalcToRomanService, CalcToRomanController>("/api/calcToRoman",
                "/api/calcToRoman/health");

            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddNethiumControllers();

            services.AddNethiumServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

            app.UseAuthentication();

            // app.UseHttpsRedirection();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUi3();
        }
    }
}