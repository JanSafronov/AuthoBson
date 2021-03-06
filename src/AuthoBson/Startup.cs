using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security;
using System.Security.Cryptography;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Cors;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using AuthoBson.Services;
using AuthoBson.Network;
using AuthoBson.Shared.Data.Models;
using AuthoBson.Models;
using AuthoBson.Shared.Services;

namespace AuthoBson
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
            
            services.Configure<StoreDatabaseSettings>(Configuration.GetSection(nameof(StoreDatabaseSettings)));
            services.AddSingleton<IStoreDatabaseSettings>(sp => sp.GetRequiredService<IOptions<StoreDatabaseSettings>>().Value);

            services.Configure<UserTemplate>(Configuration.GetSection(nameof(UserTemplate)));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<UserTemplate>>().Value);

            services.Configure<DomainSettings>(Configuration.GetSection(nameof(DomainSettings)));
            services.AddSingleton<IDomainSettings>(sp => sp.GetRequiredService<IOptions<DomainSettings>>().Value);
            
            services.AddSingleton<UserService>();

            services.AddAntiforgery();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireModeration",
                     policy => policy.RequireRole("Moderator", "Administrator"));
            });

            services.AddControllers();
            services.AddHealthChecks().AddCheck("AuthoBsoncheck", () => HealthCheckResult.Healthy());
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthoBson", Version = "v1.1.2" });
            });

            services.AddControllers().AddNewtonsoftJson(options => options.UseMemberCasing());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthoBson v1.1.2"));
            }
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {

                endpoints.MapControllers();
            });
        }
    }

    public class AuthoBsonPasswordValidator : IPasswordValidator<User>
    {
        //Implement later
        private SecurityMechanism<User, SHA256> Mechanism { get => new(); set => Mechanism = value; }

        public IdentityError identityError { get => new() { Code = "0001", Description = "User cannot be validated" }; }

        public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user, string password)
        {
            user = manager.Users.FirstOrDefault(User => User.Password == password);

            if (user == null)
                return Task.FromResult(IdentityResult.Failed(identityError));

            return Task.FromResult(IdentityResult.Success);
        }
    }

    public class AuthoBsonRoleValidator : IRoleValidator<User>
    {

        public IdentityError identityError { get => new() { Code = "0002", Description = "Role cannot be validated" }; }

        public Task<IdentityResult> ValidateAsync(RoleManager<User> manager, User role)
        {
            if (manager.Roles.Any(User => User == role))
                return Task.FromResult(IdentityResult.Success);

            return Task.FromResult(IdentityResult.Failed(identityError));
        }
    }
}
