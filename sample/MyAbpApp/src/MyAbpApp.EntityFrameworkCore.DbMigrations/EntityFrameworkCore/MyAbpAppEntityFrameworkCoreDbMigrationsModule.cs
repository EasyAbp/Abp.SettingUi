using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace MyAbpApp.EntityFrameworkCore
{
    [DependsOn(
        typeof(MyAbpAppEntityFrameworkCoreModule)
        )]
    public class MyAbpAppEntityFrameworkCoreDbMigrationsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<MyAbpAppMigrationsDbContext>();
        }
    }
}
