using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Abstractions;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi;
using MongoDB.Bson.Serialization;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Services;
using Models;

namespace RESTful_OnlineDish
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            BsonClassMap.RegisterClassMap<BsonUser>(cm => 
            {
                cm.MapMember(c => c.username);
                cm.MapMember(c => c.password);
                cm.MapMember(c => c.email);
                cm.MapMember(c => c.notification);
                cm.MapMember(c => c.role);
            });
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<UserstoreDatabaseSettings>(Configuration.GetSection(nameof(UserstoreDatabaseSettings)));
            services.AddSingleton<IUserstoreDatabaseSettings>(sp => sp.GetRequiredService<IOptions<UserstoreDatabaseSettings>>().Value);

            services.AddSingleton<UserService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RESTful_OnlineDish", Version = "v1" });
            });

            services.AddControllers().AddNewtonsoftJson(options => options.UseMemberCasing());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RESTful_OnlineDish v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/healthwb").RequireAuthorization("/admin");

                endpoints.MapControllers();
            });
        }
    }
}
