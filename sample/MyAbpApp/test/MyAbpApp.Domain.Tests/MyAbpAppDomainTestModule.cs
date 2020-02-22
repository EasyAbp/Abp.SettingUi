using MyAbpApp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace MyAbpApp
{
    [DependsOn(
        typeof(MyAbpAppEntityFrameworkCoreTestModule)
        )]
    public class MyAbpAppDomainTestModule : AbpModule
    {

    }
}