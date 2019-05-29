using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Polly;
using Swashbuckle.AspNetCore.Swagger;

namespace Meetup.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            AddEventStore();
            services.AddScoped<IMeetupRepository, MeetupRepository>();
            services.AddScoped<MeetupApplicationService>();
            services.AddControllers().AddNewtonsoftJson();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "Meetup",
                        Version = "v1"
                    });
            });

            void AddEventStore() =>
                Retry(() => services.AddSingleton<IDocumentStore>(DocumentStore.For(_ =>
                {
                    _.Connection(Configuration["eventstore"] ?? "Host=localhost;Port=5432;Username=postgres;Password=changeit");
                    _.Events.DatabaseSchemaName = "meetup";
                })));

            void Retry(Action action, int retries = 3) =>
                Policy.Handle<Exception>()
                    .WaitAndRetry(retries, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                    .Execute(action);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Meetup v1"));
        }
    }
}
