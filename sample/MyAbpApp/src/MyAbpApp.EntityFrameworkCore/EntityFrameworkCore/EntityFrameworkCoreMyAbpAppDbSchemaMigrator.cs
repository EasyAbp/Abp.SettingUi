using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyAbpApp.Data;
using Volo.Abp.DependencyInjection;

namespace MyAbpApp.EntityFrameworkCore
{
    public class EntityFrameworkCoreMyAbpAppDbSchemaMigrator
        : IMyAbpAppDbSchemaMigrator, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public EntityFrameworkCoreMyAbpAppDbSchemaMigrator(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task MigrateAsync()
        {
            /* We intentionally resolving the MyAbpAppDbContext
             * from IServiceProvider (instead of directly injecting it)
             * to properly get the connection string of the current tenant in the
             * current scope.
             */

            await _serviceProvider
                .GetRequiredService<MyAbpAppDbContext>()
                .Database
                .MigrateAsync();
        }
    }
}
