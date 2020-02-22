using MyAbpApp.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace MyAbpApp.DbMigrator
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(MyAbpAppEntityFrameworkCoreDbMigrationsModule),
        typeof(MyAbpAppApplicationContractsModule)
        )]
    public class MyAbpAppDbMigratorModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
        }
    }
}
