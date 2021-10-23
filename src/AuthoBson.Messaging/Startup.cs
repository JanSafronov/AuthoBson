using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi;
using AuthoBson.Messaging.Services;
using AuthoBson.Messaging.Data.Models.Templates;
using AuthoBson.Shared.Data.Models;

namespace AuthoBson.Messaging
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure options for the service
            services.Configure<StoreDatabaseSettings>(Configuration.GetSection(nameof(StoreDatabaseSettings)));
            services.AddSingleton<IStoreDatabaseSettings>(sp => sp.GetRequiredService<IOptions<StoreDatabaseSettings>>().Value);

            services.Configure<MessageTemplate>(Configuration.GetSection(nameof(MessageTemplate)));
            services.AddSingleton<IMessageTemplate>(sp => sp.GetRequiredService<IOptions<MessageTemplate>>().Value);

            services.AddSingleton<MessageService>();

            services.AddHealthChecks().AddCheck("AuthoBson check", () => HealthCheckResult.Healthy());
            services.AddMvc(ops =>
            {
                ops.EnableEndpointRouting = false;
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthoBson", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // Swagger UI
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthoBson v1"));
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseRouting();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {

                endpoints.MapControllers();
            });
            
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}