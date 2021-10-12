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
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using AuthoBson.Services;
using AuthoBson.Models.Templates;
using AuthoBson.Protocols;
using AuthoBson.Protocols.Settings;
using AuthoBson.Shared.Data.Models;

namespace AuthoBson
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            //BsonClassMap.RegisterClassMap<BsonUser>();
            //BsonClassMap.RegisterClassMap<User>();
                /*cm => {
                cm.AutoMap();
                cm.SetIsRootClass(true);

                var featureType = typeof(User);
                featureType.Assembly.GetTypes()
                    .Where(type => featureType.IsAssignableFrom(type)).ToList()
                    .ForEach(type => cm.AddKnownType(type));
            });*/

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<UserstoreDatabaseSettings>(Configuration.GetSection(nameof(UserstoreDatabaseSettings)));
            services.AddSingleton<IUserstoreDatabaseSettings>(sp => sp.GetRequiredService<IOptions<UserstoreDatabaseSettings>>().Value);

            services.Configure<UserTemplate>(Configuration.GetSection(nameof(UserTemplate)));
            services.AddSingleton<IUserTemplate>(sp => sp.GetRequiredService<IOptions<UserTemplate>>().Value);

            services.Configure<DomainSettings>(Configuration.GetSection(nameof(DomainSettings)));
            services.AddSingleton<IDomainSettings>(sp => sp.GetRequiredService<IOptions<DomainSettings>>().Value);

            services.AddSingleton<UserService>();
            //services.AddScoped<IMailSender, SMTPMail>(new Func<IServiceProvider, SMTPMail>(arg => arg.GetService<SMTPMail>()));

            services.AddControllers();
            services.AddHealthChecks().AddCheck("AuthoBson check", () => HealthCheckResult.Healthy());
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthoBson", Version = "v1" });
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
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthoBson v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapHealthChecks("/healthwb").RequireAuthorization("/admin");

                endpoints.MapControllers();
            });
        }
    }
}