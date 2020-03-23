using System;
using Autofac;
using ExternalPriceCollector.Azure;
using ExternalPriceCollector.Cache;
using ExternalPriceCollector.Configuration;
using ExternalPriceCollector.EntityFramework.Context;
using ExternalPriceCollector.EntityFramework.Repositories;
using ExternalPriceCollector.GrpcServices;
using ExternalPriceCollector.Models;
using ExternalPriceCollector.Rabbit.Subscribers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swisschain.Sdk.Server.Common;

namespace ExternalPriceCollector
{
    public sealed class Startup : SwisschainStartup<AppConfig>
    {
        public Startup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void RegisterEndpoints(IEndpointRouteBuilder endpoints)
        {
            base.RegisterEndpoints(endpoints);

            endpoints.MapGrpcService<MonitoringService>();
        }

        protected override void ConfigureContainerExt(ContainerBuilder builder)
        {
            builder.RegisterType<QuoteSubscriber>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance();

            builder.RegisterType<ConnectionFactory>()
                .AsSelf()
                .WithParameter(TypedParameter.From(Config.DbSettings.DataConnectionString))
                .SingleInstance();

            builder.RegisterType<QuoteRepository>()
                .SingleInstance();

            builder.RegisterType<QuoteMemoryCache<Quote>>()
                .SingleInstance();

            var azureBlobStorage = AzureBlobStorage.Create(Config.AzureStorage.ConnectionString, TimeSpan.FromMinutes(1));
            builder.RegisterInstance(azureBlobStorage)
                .As<IBlobStorage>()
                .SingleInstance();
        }

        protected override void ConfigureExt(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.ApplicationServices.GetRequiredService<ConnectionFactory>()
                .EnsureMigration();
        }
    }
}
