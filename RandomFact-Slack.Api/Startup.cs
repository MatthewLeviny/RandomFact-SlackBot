using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using RandomFact_Slack.Core.Services;
using RandomFact_Slack.Infrastructure.Options;
using RandomFact_Slack.Infrastructure.Service;
using IHostingEnvironment = Microsoft.Extensions.Hosting.IHostingEnvironment;

namespace RandomFact_Slack.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });

            services.AddResponseCompression(options => { options.Providers.Add<GzipCompressionProvider>(); });

            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("v1", new OpenApiSecurityScheme()
                {
                    In = ParameterLocation.Header, // where to find apiKey, probably in a header
                    Name = "X-API-KEY", //header with api key
                    Type = SecuritySchemeType.ApiKey // this value is always "apiKey"

                });
            });

            services.AddHealthChecks().AddCheck<HealthCheckService>("HealthCheckService");

            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder.WithOrigins("https://localhost*",
                            "http://localhost*");
                    });
            });
            services.AddHttpClient();
            //http clients
            services.AddHttpClient("FactApi", client =>
            {
                client.BaseAddress = new Uri("https://uselessfacts.jsph.pl");
            });

            //Options Files
            services.AddSingleton<FactOptions, FactOptions>(serviceProvider => Configuration.GetSection(nameof(FactOptions)).Get<FactOptions>());

            //DI - Core
            services.AddTransient<IFactService, FactService>();

            //DI - Infrastructure
            services.AddScoped<IRandomFactApiService, RandomFactApiService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Random Fact Slack Bot");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors();
            app.UseResponseCompression();
            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}