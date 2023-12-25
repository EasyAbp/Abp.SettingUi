﻿using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyAbpApp.Data;
using Serilog;
using Volo.Abp;
using Volo.Abp.Data;

namespace MyAbpApp.DbMigrator
{
    public class DbMigratorHostedService : IHostedService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public DbMigratorHostedService(IHostApplicationLifetime hostApplicationLifetime)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var application = await AbpApplicationFactory.CreateAsync<MyAbpAppDbMigratorModule>(options =>
            {
                options.UseAutofac();
                options.AddDataMigrationEnvironment();
                options.Services.AddLogging(c => c.AddSerilog());
            }))
            {
                await application.InitializeAsync();

                await application
                    .ServiceProvider
                    .GetRequiredService<MyAbpAppDbMigrationService>()
                    .MigrateAsync();

                await application.ShutdownAsync();

                _hostApplicationLifetime.StopApplication();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
