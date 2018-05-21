﻿using System;
using Common.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Silverback.Domain;
using Silverback.Messaging;
using Silverback.Messaging.Adapters;
using Silverback.Messaging.Broker;
using Silverback.Messaging.Configuration;
using SilverbackShop.Catalog.Domain;
using SilverbackShop.Catalog.Domain.Repositories;
using SilverbackShop.Catalog.Infrastructure;
using SilverbackShop.Common.Data;
using Swashbuckle.AspNetCore.Swagger;

namespace SilverbackShop.Catalog.Service
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
            services.AddDbContext<CatalogContext>(o => o.UseSqlite($"Data Source={Configuration["DB:Path"]}Catalog.db"));

            services.AddMvc();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "SilverbackShop - Catalog API",
                    Version = "v1"
                });
            });

            services.AddScoped<ICatalogUnitOfWork, CatalogUnitOfWork>();
            services.AddTransient(s => s.GetService<ICatalogUnitOfWork>().Products);

            // TODO: Can get rid of this?
            services.AddSingleton<SimpleOutboundAdapter>();

            // TODO: Create extension method services.AddBus() in Silverback.AspNetCore
            var bus = new Bus();
            services.AddSingleton<IBus>(bus);
            services.AddSingleton(bus.GetEventPublisher<IDomainEvent<IDomainEntity>>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.ReturnExceptionsAsJson();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Baskets API V1");
            });

            // TODO: Create extension method app.UseBus() in Silverback.AspNetCore
            var bus = app.ApplicationServices.GetService<IBus>();
            bus.Config()
                .ConfigureBroker<FileSystemBroker>(c => c.OnPath(Configuration["Broker:Path"]))
                .WithFactory(t => app.ApplicationServices.GetService(t), t => app.ApplicationServices.GetServices(t))
                .ConfigureUsing<CatalogDomainMessagingConfigurator>();

            InitializeDatabase(app);
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<CatalogContext>().Database.Migrate();
            }
        }
    }
}
