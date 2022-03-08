using System;

using AspNetCoreRateLimit;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

using YoutubeWithFriends.Api.Services;
using YoutubeWithFriends.Db;

namespace YoutubeWithFriends.Api {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            /* Start IP Rate limit config */

            services.AddOptions()
                .AddMemoryCache();

            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"))
                .Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));

            services.AddInMemoryRateLimiting()
                .AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            /* End IP Rate limit config */

            services.AddControllers();
            services.AddSwaggerGen(options =>
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "YoutubeWithFriends.Api", Version = "v1" }));

            var dbInstanceId = Guid.NewGuid();
            services.AddSingleton<ISimpleDbContextFactory>(new SimpleDbContextFactory(options =>
                options.UseInMemoryDatabase($"MemoryDatabase-{dbInstanceId}")));

            services.AddSingleton<IIpAddressResolver, IpAddressResolver>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "YoutubeWithFriends.Api v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
                endpoints.MapControllers());

            app.UseIpRateLimiting();
        }
    }
}