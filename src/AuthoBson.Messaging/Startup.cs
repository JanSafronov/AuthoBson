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

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RoutedDatabaseSettings>(Configuration.GetSection(nameof(RoutedDatabaseSettings)));
            services.AddSingleton<IRoutedDatabaseSettings>(sp => sp.GetRequiredService<IOptions<RoutedDatabaseSettings>>().Value);

            services.Configure<MessageTemplate>(Configuration.GetSection(nameof(MessageTemplate)));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<MessageTemplate>>().Value);

            services.AddSingleton<MessageService>();
            services.AddSingleton(sp => sp.GetRequiredService<Shared>)

            services.AddHealthChecks().AddCheck("AuthoBsoncheck", () => HealthCheckResult.Healthy());
            services.AddMvc(ops =>
            {
                ops.EnableEndpointRouting = false;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthoBson", Version = "v1.1" });
            });

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // Swagger UI
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthoBson v1.1"));
            }
            else
            {
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
